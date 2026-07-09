// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine.Assertions;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
    // Bindings for the layout/info exports added to Runtime/BaseClasses/EntityIdStore.{h,cpp}
    // and the GCHandle-offset helper in BaseObject.{h,cpp}. Kept private to this file so the
    // EntityIdStore type below is the only consumer.
    [NativeHeader("Runtime/BaseClasses/EntityIdStore.h")]
    [NativeHeader("Runtime/BaseClasses/BaseObject.h")]
    internal static class EntityIdStoreBindings
    {
        [NativeMethod(Name = "Object::GetOffsetOfGCHandleMember", IsFreeFunction = true, IsThreadSafe = true)]
        public static extern int GetOffsetOfGCHandleInCPlusPlusObject();

        [NativeMethod(Name = "GetEntityIdAllocatorStore", IsFreeFunction = true, IsThreadSafe = true)]
        public static extern unsafe void* GetEntityIdAllocatorStore();

        [NativeMethod(Name = "GetEntityIdStoreBlockShift", IsFreeFunction = true, IsThreadSafe = true)]
        public static extern uint GetEntityIdStoreBlockShift();

        [NativeMethod(Name = "GetEntityIdStoreBlockMask", IsFreeFunction = true, IsThreadSafe = true)]
        public static extern uint GetEntityIdStoreBlockMask();

        [NativeMethod(Name = "GetEntityIdStoreBlockCount", IsFreeFunction = true, IsThreadSafe = true)]
        public static extern uint GetEntityIdStoreBlockCount();

        [NativeMethod(Name = "GetEntityIdStoreWordsPerBlock", IsFreeFunction = true, IsThreadSafe = true)]
        public static extern uint GetEntityIdStoreWordsPerBlock();

        [NativeMethod(Name = "GetEntityIdStoreEntityCount", IsFreeFunction = true, IsThreadSafe = true)]
        public static extern unsafe void* GetEntityIdStoreEntityCount();

        // null on page-table path; null on non-editor builds for reserved.
        [NativeMethod(Name = "GetEntityIdStoreAllocatedBits", IsFreeFunction = true, IsThreadSafe = true)]
        public static extern unsafe void* GetEntityIdStoreAllocatedBits();

        [NativeMethod(Name = "GetEntityIdStoreReservedBits", IsFreeFunction = true, IsThreadSafe = true)]
        public static extern unsafe void* GetEntityIdStoreReservedBits();

        // null on page-table and on-demand-commit paths.
        [NativeMethod(Name = "GetEntityIdStoreBlockCommittedTable", IsFreeFunction = true, IsThreadSafe = true)]
        public static extern unsafe void* GetEntityIdStoreBlockCommittedTable();

        [NativeMethod(Name = "EntityIdStorePlatformSupportsVirtualMemory", IsFreeFunction = true, IsThreadSafe = true)]
        public static extern bool EntityIdStorePlatformSupportsVirtualMemory();

        // Commit a block's slot page if not already committed.
        // Caller must already hold the per-block lock (entityCount[blockIndex] == k_BlockBusy).
        [NativeMethod(Name = "EntityIdStore_EnsureBlockCommitted", IsFreeFunction = true, IsThreadSafe = true)]
        public static extern void EnsureBlockCommitted(uint blockIndex);

        // OS yield for the escalation branch of BlockSpinBackoff (defined in EntityComponentStoreEntities.cs).
        [NativeMethod(Name = "EntityIdStore_OsThreadYield", IsFreeFunction = true, IsThreadSafe = true)]
        public static extern void OsThreadYield();
    }

    // Managed view of the native EntityIdStore.
    //
    // Layout MUST stay in sync with Runtime/BaseClasses/EntityIdStore.cpp:
    //   - EntitySlot: 16 bytes, ulong versionAndChunk + IntPtr nativeObjectPtr.
    //   - versionAndChunk packing: [chunkIndex:32 | indexInChunk:8 | version:24].
    //   - EntityId packing: [Version:24 | TypeId:12 | Index:28]. See EntityID.h.
    //
    // Storage mode is decided natively and queried once at type init through
    // context.PlatformSupportsVirtualMemory, matching the native side. It cannot be a
    // C# compile-time #if: C# has no 64-bit define and the native
    // PLATFORM_SUPPORTS_ENTITYID_VIRTUAL_MEMORY define is forced off on 32-bit.
    // Either:
    //   Virtual memory mode: a flat EntitySlot array indexed directly by entity index.
    //   Page table mode:     a Block** indexed by (blockIndex, slotInBlock). slots is
    //                        the first member of each Block, so a block pointer is also
    //                        &slots[0] (see GetSlot).
    // The block geometry and committed-table base are queried once via
    // EntityIdStoreBindings and cached in the ContextData SharedStatic.
    internal unsafe partial class EntityIdStore
    {
        // Mirrors native EntitySlot in Runtime/BaseClasses/EntityIdStore.cpp.
        // Reading versionAndChunk and nativeObjectPtr as plain values is safe on
        // x86/arm64: the native side uses baselib::atomic only for memory ordering;
        // it has the same layout as the underlying type.
        [StructLayout(LayoutKind.Sequential, Size = 16)]
        internal struct EntitySlot
        {
            public ulong versionAndChunk;
            public IntPtr nativeObjectPtr;
        }

        // Version is the LOW 24 bits of versionAndChunk (mask-only extraction).
        internal const ulong k_SlotVersionMask = (1UL << 24) - 1; // 0x00FFFFFF

        // versionAndChunk field shifts — mirror C++ k_SlotIndexInChunkShift / k_SlotChunkIndexShift.
        internal const int k_IndexInChunkShift = 24;
        internal const int k_ChunkIndexShift   = 32;

        // versionAndChunk field helpers — mirror C++ SlotGetVersion / SlotPackVersionAndChunk / SlotSetVersion.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint  SlotGetVersion(ulong vac)      => (uint)(vac & k_SlotVersionMask);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int   SlotGetIndexInChunk(ulong vac) => (int)((vac >> k_IndexInChunkShift) & 0xFFUL);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int   SlotGetChunkIndex(ulong vac)   => (int)(uint)(vac >> k_ChunkIndexShift);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong SlotPackVersionAndChunk(uint version, byte indexInChunk, uint chunkIndex)
            => ((ulong)chunkIndex   << k_ChunkIndexShift)
             | ((ulong)indexInChunk << k_IndexInChunkShift)
             | (version & k_SlotVersionMask);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong SlotSetVersion(ulong vac, uint newVersion)
            => (vac & ~k_SlotVersionMask) | (newVersion & k_SlotVersionMask);

        // Per-block spinlock sentinel — mirrors C++ k_BlockBusy.
        internal const int k_BlockBusy = -1;

        // Bitmap word geometry — mirrors C++ k_WordShift / k_WordMask.
        internal const int  k_WordShift = 6;   // log2(64) — bits per UInt64
        internal const uint k_WordMask  = 63;  // 64 - 1

        // Layout fields that Burst-compiled code must be able to read are stored in a
        // SharedStatic so the Burst JIT never needs to call (or analyse) the class
        // initializer, which contains extern calls that Burst cannot compile.
        // Populated once by Initialize(), which is called eagerly via [OnCodeLoaded]
        // so Burst-compiled callers always see a valid context (Burst cannot execute
        // the P/Invoke calls inside Initialize() itself).
        internal struct ContextData
        {
            internal struct BurstIdentifier {}

            // Storage-mode selector — JIT folds branches on this after Initialize().
            public bool PlatformSupportsVirtualMemory;
            // Block geometry.
            public int   BlockShift;
            public uint  BlockMask;
            // Allocator tables.
            public int*  EntityCount;    // baselib::atomic<int>[], same layout as int[]
            public uint  BlockCount;
            public uint  WordsPerBlock;
            public ulong* AllocatedBits; // null on page-table path
            public ulong* ReservedBits;  // null on page-table path and in player builds
            public byte*  BlockCommitted;// null on page-table / on-demand-commit paths
            // Raw store pointer: EntitySlot* (VM path) or Block** (page-table path).
            public void*  NativeStore;
            // Zeroed sentinel slot: version 0 never matches any live entity.
            public EntitySlot NullSlot;
            // Byte offset of the GCHandle member inside a C++ Object — for GetManagedObject.
            public int OffsetOfGCHandleInObject;
            // Set to true by Initialize() so subsequent calls are no-ops.
            public bool IsInitialized;
        }

        internal static readonly SharedStatic<ContextData> s_Context =
            SharedStatic<ContextData>.GetOrCreate<ContextData.BurstIdentifier>();

        // Populates the SharedStatic layout from native once; subsequent calls are no-ops.
        // [OnCodeLoaded] runs this from managed code at domain load, before any Burst-compiled
        // caller can reach AllocateEntityIds. Burst cannot execute the P/Invoke calls here,
        // so eager managed-side initialization is required — same role as the C++ side's
        // RegisterRuntimeInitializeAndCleanup for InitializeEntityIdStore.
        [Unity.Scripting.LifecycleManagement.OnCodeLoaded]
        internal static void Initialize()
        {
            ref ContextData ctx = ref s_Context.Data;
            if (ctx.IsInitialized) return;
            ctx.PlatformSupportsVirtualMemory = EntityIdStoreBindings.EntityIdStorePlatformSupportsVirtualMemory();
            ctx.BlockShift    = (int)EntityIdStoreBindings.GetEntityIdStoreBlockShift();
            ctx.BlockMask     = EntityIdStoreBindings.GetEntityIdStoreBlockMask();
            ctx.EntityCount   = (int*)EntityIdStoreBindings.GetEntityIdStoreEntityCount();
            ctx.BlockCount    = EntityIdStoreBindings.GetEntityIdStoreBlockCount();
            ctx.WordsPerBlock = EntityIdStoreBindings.GetEntityIdStoreWordsPerBlock();
            ctx.AllocatedBits = (ulong*)EntityIdStoreBindings.GetEntityIdStoreAllocatedBits();
            ctx.ReservedBits  = (ulong*)EntityIdStoreBindings.GetEntityIdStoreReservedBits();
            ctx.BlockCommitted= (byte*)EntityIdStoreBindings.GetEntityIdStoreBlockCommittedTable();
            ctx.NativeStore              = EntityIdStoreBindings.GetEntityIdAllocatorStore();
            ctx.NullSlot                 = default;
            ctx.OffsetOfGCHandleInObject = EntityIdStoreBindings.GetOffsetOfGCHandleInCPlusPlusObject();
            ctx.IsInitialized = true;
        }



        // ----------------------------------------------------------------------
        // Native slot lookup
        // ----------------------------------------------------------------------

        // Returns a reference to the slot for the given entity index, or to a
        // process-wide zeroed sentinel if the slot is on an uncommitted page.
        // Mirrors native EntitySlot& GetSlot(UInt32) in EntityIdStore.cpp; the
        // sentinel keeps the call sites branch-free and lets the version check
        // act as the single validation step.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ref EntitySlot GetSlot(uint entityIndex)
        {
            ref ContextData ctx = ref s_Context.Data;
            if (ctx.PlatformSupportsVirtualMemory)
            {
                uint blockIndex = entityIndex >> ctx.BlockShift;
                if (Volatile.Read(ref ctx.BlockCommitted[blockIndex]) == 0)
                    return ref ctx.NullSlot;
                return ref ((EntitySlot*)ctx.NativeStore)[entityIndex];
            }
            else
            {
                EntitySlot** blockTable = (EntitySlot**)ctx.NativeStore;
                EntitySlot* slots = blockTable[entityIndex >> ctx.BlockShift];
                if (slots == null)
                    return ref ctx.NullSlot;
                return ref slots[entityIndex & ctx.BlockMask];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool BlockIsCommitted(uint blockIndex)
        {
            ref ContextData ctx = ref s_Context.Data;
            if (ctx.PlatformSupportsVirtualMemory)
                // BlockCommitted is null on the VM on-demand-commit path; null means all blocks
                // are committed on access (the page fault handler commits them).
                return ctx.BlockCommitted == null || Volatile.Read(ref ctx.BlockCommitted[blockIndex]) != 0;
            return Volatile.Read(ref ((IntPtr*)ctx.NativeStore)[blockIndex]) != IntPtr.Zero;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong* BlockAllocated(uint blockIndex)
        {
            ref ContextData ctx = ref s_Context.Data;
            if (ctx.PlatformSupportsVirtualMemory)
                return ctx.AllocatedBits + blockIndex * ctx.WordsPerBlock;
            return ((Block**)ctx.NativeStore)[blockIndex]->allocated;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong* BlockReserved(uint blockIndex)
        {
            ref ContextData ctx = ref s_Context.Data;
            if (ctx.PlatformSupportsVirtualMemory)
                return ctx.ReservedBits + blockIndex * ctx.WordsPerBlock;
            return ((Block**)ctx.NativeStore)[blockIndex]->reserved;
        }

        // Mirrors C++ struct Block in EntityIdStore.cpp (page-table path only).
        // slots[] is at offset 0 so (EntitySlot*)blockPtr == &block->slots[0].
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct Block
        {
            fixed byte slotsRaw[256 * 16];     // EntitySlot slots[256] at offset 0
            public fixed ulong allocated[4];    // UInt64 allocated[wordsPerBlock]
            public fixed ulong reserved[4];
        }

        // ----------------------------------------------------------------------
        // Public API
        // ----------------------------------------------------------------------

        // Pure C# existence check — mirrors C++ EntityExists in EntityIdStore.cpp.
        // Version 0 is the uninitialized-slot sentinel and is never assigned to a live entity.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool Exists(EntityId entity)
        {
            if (entity.Version == 0) return false;
            ref EntitySlot slot = ref GetSlot(entity.Index);
            return SlotGetVersion(Volatile.Read(ref slot.versionAndChunk)) == entity.Version;
        }

        // Overwrites the version stored in an entity's slot — used by deserialization.
        // Version 0 is the uninitialized-slot sentinel and must never be written to a live slot.
        internal static void SetEntityVersion(EntityId entity, int version)
        {
            uint maskedVersion = (uint)version & (uint)k_SlotVersionMask;
            Assert.AreNotEqual(0u, maskedVersion);
            ref EntitySlot slot = ref GetSlot(entity.Index);
            ulong vac = Volatile.Read(ref slot.versionAndChunk);
            Volatile.Write(ref slot.versionAndChunk, SlotSetVersion(vac, maskedVersion));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void* GetNativeObject(EntityId entity)
        {
            ref EntitySlot slot = ref GetSlot(entity.Index);
            uint expectedVersion = entity.Version;

            // Version seqlock matching native GetNativePtr. C# has no acquire
            // fence or relaxed atomic load, so all three are acquire loads
            // (Volatile.Read) instead of native's acquire/relaxed mix; the extra
            // ordering is free on x86 and a cheap ldar on ARM64, and far lighter
            // than the full StoreLoad fence Thread.MemoryBarrier() would emit.
            ulong v1 = Volatile.Read(ref slot.versionAndChunk);
            IntPtr ptr = Volatile.Read(ref slot.nativeObjectPtr);
            ulong v2 = Volatile.Read(ref slot.versionAndChunk);

            if ((uint)(v1 & k_SlotVersionMask) != expectedVersion ||
                (uint)(v2 & k_SlotVersionMask) != expectedVersion)
                return null;

            return (void*)ptr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetManagedObject<T>(EntityId entity) where T : UnityEngine.Object
        {
            void* objectPtr = GetNativeObject(entity);
            if (objectPtr == null)
                return null;

            GCHandle handle = *(GCHandle*)((byte*)objectPtr + s_Context.Data.OffsetOfGCHandleInObject);
            // Resident natively but not yet wrapped (e.g. a baked / deserialized ref on
            // first access): treat as a miss so the caller falls back to native resolution,
            // which materializes the wrapper. Reading Target on an empty handle would throw.
            if (!handle.IsAllocated)
                return null;
            return UnsafeUtility.As<T>(handle.Target);
        }

        // ----------------------------------------------------------------------
        // ── Integrity check — mirrors C++ IntegrityCheck in EntityIdStore.cpp ───

        // Counts allocated entity IDs, skipping blocks that are currently locked.
        // Thread-unsafe by design; call only from single-threaded diagnostic contexts.
        internal static int DebugOnlyThreadUnsafeEntityCount()
        {
            int count = 0;
            for (uint i = 0; i < s_Context.Data.BlockCount; i++)
            {
                int v = s_Context.Data.EntityCount[i];
                if (v != k_BlockBusy) count += v;
            }
            return count;
        }

        // Verifies that the allocated bitmap matches the slot parity invariant
        // (odd version = alive) for every entity in a block.
        internal static void IntegrityCheck(int blockIndex)
        {
            if (!BlockIsCommitted((uint)blockIndex))
            {
                Assert.AreEqual(0, s_Context.Data.EntityCount[blockIndex]);
                return;
            }

            ulong* allocated     = BlockAllocated((uint)blockIndex);
            ulong* reserved      = BlockReserved((uint)blockIndex);
            uint entitiesInBlock = s_Context.Data.BlockMask + 1;
            uint baseIndex       = (uint)blockIndex * entitiesInBlock;

            for (uint i = 0; i < entitiesInBlock; i++)
            {
                uint entityIndex = baseIndex + i;
                if (entityIndex >= (1u << 28)) break; // 28 = EntityId.kIndexBits

                var aliveA = (allocated[i >> k_WordShift] >> (int)(i & k_WordMask)) & 1UL;
                ref var slot = ref GetSlot(entityIndex);
                var aliveB = (ulong)(SlotGetVersion(Volatile.Read(ref slot.versionAndChunk)) & 1u);

                var isReserved = (reserved[i >> k_WordShift] >> (int)(i & k_WordMask)) & 1UL;
                if (isReserved != 0) continue;
                Assert.AreEqual(aliveA, aliveB);
            }
        }

        internal static void IntegrityCheck()
        {
            for (int i = 0; i < (int)s_Context.Data.BlockCount; i++)
                IntegrityCheck(i);
        }

        // ── Allocation infrastructure
        //
        // Mirrors C++ EntityIdStore.cpp: block spinlock, per-thread pool, global
        // block scanner, and deallocation.  All non-ECS code (e.g. future native
        // Object allocations) uses these directly; Unity.Entities wraps them with
        // thin adapters that add chunk-placement stamping.
        // ----------------------------------------------------------------------

        // Mirrors C++ BlockSpinBackoff. Exponential CPU pause escalating to an OS
        // yield after 7 iterations (matches k_BlockSpinYieldAfterIterations = 7).
        // Common.Pause() emits the hardware PAUSE/YIELD instruction in Burst; the
        // [BurstDiscard] managed fallback uses Thread.SpinWait for a richer OS spin.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void BlockSpinBackoff(ref int iter)
        {
            if (iter < 7)
            {
                int count = 1 << iter;
                ManagedSpinWait(count);              // Thread.SpinWait in managed, no-op in Burst
                for (int p = 0; p < count; ++p)
                    Common.Pause();                  // hardware PAUSE/YIELD in Burst (and managed)
            }
            else
                EntityIdStoreBindings.OsThreadYield();
            ++iter;
        }

        // Discarded by Burst; gives managed callers a single efficient OS-level spin
        // wait in place of the Common.Pause() loop below.
        [BurstDiscard]
        static void ManagedSpinWait(int count) => Thread.SpinWait(count);

        // Per-thread pre-allocated EntityId cache.  Mirrors C++ EntityIdThreadPool.
        // Populate() bulk-allocates via AllocateEntityIds(); TakeEntityIds() hands
        // IDs out one batch at a time with no global synchronisation.
        // Process-wide per-thread entity ID pool — the single SharedStatic instance.
        // Callers use EntityIdStore.Pool directly rather than going through EntityComponentStore.
        internal static readonly SharedStatic<EntityIdPool> Pool =
            SharedStatic<EntityIdPool>.GetOrCreate<EntityIdPool.BurstStaticIdentifier>();

        // Drain and free the pool when code is unloaded. This fires from CodeLoadedScope.Exit,
        // which runs both on an editor domain reload and at player/editor shutdown (see
        // CleanupAllObjects in Runtime/Misc/SaveAndLoadHelper.cpp) — in both cases before the
        // native store is torn down, so Drain (returns slots) and Dispose (frees the
        // Allocator.Persistent buffers) are safe.
        //   - Editor reload: Burst zeroes every SharedStatic at domainUnloadComplete
        //     (BurstCompilerService::ClearSharedMemory), but never frees the pool's Persistent
        //     buffers and never clears the native allocation bitmap. Without this, each reload
        //     would leak those buffers and strand the pool's reserved-but-unhanded-out slots.
        //   - Player shutdown: matches the old native EntityIdThreadPool, which was drained in
        //     DisposeEntityIdStore, so leak detection stays clean at exit.
        [Unity.Scripting.LifecycleManagement.OnCodeUnloading]
        static void OnCodeUnloading()
        {
            Pool.Data.Drain();
            Pool.Data.Dispose();
        }

        internal struct EntityIdPool
        {
            internal struct BurstStaticIdentifier { }

            // 64-byte cache-line padding keeps two threads' hot fields on separate lines.
            [StructLayout(LayoutKind.Sequential, Size = 64)]
            struct ArrayInfo
            {
                public int m_AvailableCount;
                public int m_NextIndex;
            }

            ArrayInfo* m_Arrays;          // [MaxJobThreadCount], aligned to 64 B
            EntityId*  m_EntityIds;       // flat buffer [k_PooledPerThread * MaxJobThreadCount]

            internal const int k_PooledPerThread = 128; // matches C++ gEntitiesInBlock / 2

            public int NumberAllocated;

            internal void Populate()
            {
                int total = k_PooledPerThread * JobsUtility.MaxJobThreadCount;
                NumberAllocated = total;
                if (m_EntityIds != null) return;

                m_EntityIds = (EntityId*)UnsafeUtility.Malloc(
                    (long)total * sizeof(EntityId), UnsafeUtility.AlignOf<EntityId>(), Allocator.Persistent);
                AllocateEntityIdsGlobal(m_EntityIds, total);

                m_Arrays = (ArrayInfo*)UnsafeUtility.Malloc(
                    (long)JobsUtility.MaxJobThreadCount * sizeof(ArrayInfo), 64, Allocator.Persistent);
                for (int i = 0; i < JobsUtility.MaxJobThreadCount; i++)
                {
                    m_Arrays[i].m_NextIndex    = 0;
                    m_Arrays[i].m_AvailableCount = k_PooledPerThread;
                }
            }

            // Returns un-consumed pool IDs back to the store so their slots can be
            // reallocated. Must be called before world teardown to avoid leaking slots.
            // After Drain the pool has 0 available per thread; the next TakeEntityIds
            // call will naturally trigger an AllocateEntityIdsGlobal refill.
            internal void Drain()
            {
                if (m_Arrays == null) return;
                for (int i = 0; i < JobsUtility.MaxJobThreadCount; i++)
                {
                    int available = m_Arrays[i].m_AvailableCount;
                    if (available > 0)
                    {
                        EntityId* threadBuf = m_EntityIds + k_PooledPerThread * i;
                        ReleaseEntityIds(threadBuf + m_Arrays[i].m_NextIndex, available);
                    }
                    m_Arrays[i].m_NextIndex    = 0;
                    m_Arrays[i].m_AvailableCount = 0;
                }
            }

            internal void Dispose()
            {
                if (m_EntityIds == null) return;
                UnsafeUtility.Free(m_Arrays,    Allocator.Persistent); m_Arrays    = null;
                UnsafeUtility.Free(m_EntityIds, Allocator.Persistent); m_EntityIds = null;
            }

            // Hot path: LIFO pop from this thread's pool slice.
            // Falls back to AllocateEntityIds for the remainder and then refills.
            internal void TakeEntityIds(EntityId* outIds, int count)
            {
                // Lazy populate — mirrors C++ GetOrCreateThreadPool(). DOTS pre-populates
                // explicitly for warm-path perf; all other callers get it on first use.
                if (m_EntityIds == null) Populate();

                int threadIdx        = JobsUtility.ThreadIndex;
                ArrayInfo* info      = m_Arrays + threadIdx;
                EntityId*  threadBuf = m_EntityIds + k_PooledPerThread * threadIdx;

                int available = info->m_AvailableCount;
                if (available >= count)
                {
                    UnsafeUtility.MemCpy(outIds, threadBuf + info->m_NextIndex, (long)count * sizeof(EntityId));
                    info->m_NextIndex    += count;
                    info->m_AvailableCount -= count;
                    return;
                }

                // Copy what we have, allocate the remainder and refill directly via the
                // global scanner so we don't recurse back through the pool routing.
                UnsafeUtility.MemCpy(outIds, threadBuf + info->m_NextIndex, (long)available * sizeof(EntityId));
                int remaining = count - available;
                AllocateEntityIdsGlobal(outIds + available, remaining);
                AllocateEntityIdsGlobal(threadBuf, k_PooledPerThread);
                info->m_NextIndex    = 0;
                info->m_AvailableCount = k_PooledPerThread;
            }
        }

        // Entry point: routes small no-chunk requests through the per-thread pool
        // (fast path, no locking) and everything else through the global block scanner.
        // Mirrors C++ AllocateEntityIds() which does the same pool/global split.
        internal static void AllocateEntityIds(EntityId* outIds, int count,
            uint chunkBits = 0, byte firstIndexInChunk = 0)
        {
            bool hasChunk = chunkBits != 0 || firstIndexInChunk != 0;
            if (count < EntityIdPool.k_PooledPerThread && !hasChunk)
                Pool.Data.TakeEntityIds(outIds, count);   // lazily populates on first call
            else
                AllocateEntityIdsGlobal(outIds, count, chunkBits, firstIndexInChunk);
        }

        // Global block scanner — mirrors C++ AllocateEntityIdsGlobal.
        // Called directly by the pool for refills and bulk allocations to avoid
        // recursing through the routing entry point above.
        static void AllocateEntityIdsGlobal(EntityId* outIds, int count,
            uint chunkBits = 0, byte firstIndexInChunk = 0)
        {
            if (count <= 0) return;

            ref ContextData ctx = ref s_Context.Data;
            bool hasChunk       = chunkBits != 0 || firstIndexInChunk != 0;
            int  allocatedCount = 0;
            int  rescanSpin     = 0;
            byte indexInChunk   = firstIndexInChunk;

            for (;;)
            {
                bool sawContention = false;
                for (uint i = 0; i < ctx.BlockCount && allocatedCount < count; i++)
                {
                    int  entitiesPerBlock = (int)(ctx.BlockMask + 1);
                    int  blockCount;
                    bool acquired = false;
                    int  spinIter = 0;
                    while (true)
                    {
                        blockCount = Volatile.Read(ref ctx.EntityCount[i]);
                        if (blockCount == entitiesPerBlock) break;  // full
                        if (blockCount == k_BlockBusy) { sawContention = true; break; }
                        if (Interlocked.CompareExchange(ref ctx.EntityCount[i], k_BlockBusy, blockCount) == blockCount)
                        { acquired = true; break; }
                        BlockSpinBackoff(ref spinIter);
                    }
                    if (!acquired) continue;

                    // Mirrors C++ CommitBlockIfNeeded — commit on first use for both
                    // the VM path (virtual-memory commit) and the page-table path
                    // (Block struct allocation). The caller already holds the block lock.
                    if (!BlockIsCommitted(i))
                        EntityIdStoreBindings.EnsureBlockCommitted(i);

                    ulong* allocated = BlockAllocated(i);
                    ulong* reserved  = BlockReserved(i);
                    uint baseIndex   = i * (uint)entitiesPerBlock;
                    int  allocInBlock = 0;

                    for (uint wordIdx = 0; wordIdx < ctx.WordsPerBlock && allocatedCount < count; wordIdx++)
                    {
                        ulong freeBits = ~(allocated[wordIdx] | reserved[wordIdx]);
                        while (freeBits != 0 && allocatedCount < count)
                        {
                            int bit    = math.tzcnt(freeBits);
                            freeBits  &= freeBits - 1;

                            uint entityIndex = baseIndex + (wordIdx << k_WordShift) + (uint)bit;
                            // EntityId index field is 28 bits — any index at or above this is out of range.
                            if (entityIndex >= (1u << 28)) goto blockDone;

                            ref EntitySlot slot = ref GetSlot(entityIndex);
                            uint oldVersion = SlotGetVersion(Volatile.Read(ref slot.versionAndChunk));
                            // Free slots carry an even version (parity invariant); next is always odd.
                            uint newVersion = (oldVersion + 1) & (uint)k_SlotVersionMask;

                            Volatile.Write(ref slot.nativeObjectPtr, IntPtr.Zero);
                            Volatile.Write(ref slot.versionAndChunk,
                                SlotPackVersionAndChunk(newVersion,
                                    hasChunk ? indexInChunk : (byte)0,
                                    hasChunk ? chunkBits   : 0u));
                            allocated[wordIdx] |= 1UL << bit;

                            // EntityId bit layout: [Version:24 | TypeId:12 | Index:28]
                            // Version at bits 40-63, Index at bits 0-27.
                            ulong rawId = ((ulong)newVersion << 40) | (entityIndex & 0x0FFFFFFFuL);
                            outIds[allocatedCount] = UnsafeUtility.As<ulong, EntityId>(ref rawId);

                            allocatedCount++;
                            allocInBlock++;
                            if (hasChunk) indexInChunk++;
                        }
                    }

                    blockDone:
                    int prev = Interlocked.CompareExchange(ref ctx.EntityCount[i], blockCount + allocInBlock, k_BlockBusy);
                    Assert.AreEqual(k_BlockBusy, prev);
                }

                if (allocatedCount == count) return;
                if (!sawContention) break;
                BlockSpinBackoff(ref rescanSpin);
            }

            throw new InvalidOperationException(
                $"EntityIdStore: ran out of entity IDs after allocating {allocatedCount} of {count} requested.");
        }

        // Mirrors C++ ReleaseEntityIds. Coalesces same-block IDs into runs so the
        // per-block CAS lock is taken once per run rather than once per entity.
        internal static void ReleaseEntityIds(EntityId* ids, int count)
        {
            if (count <= 0) return;

            ref ContextData ctx = ref s_Context.Data;
            int i = 0;
            while (i < count)
            {
                if (ids[i] == EntityId.None) { ++i; continue; }

                uint blockIndex = ids[i].Index >> ctx.BlockShift;
                Assert.IsTrue(blockIndex < ctx.BlockCount);

                if (!BlockIsCommitted(blockIndex)) { ++i; continue; }

                int runEnd = i + 1;
                while (runEnd < count
                    && ids[runEnd] != EntityId.None
                    && (ids[runEnd].Index >> ctx.BlockShift) == blockIndex)
                    ++runEnd;

                int spinIter = 0;
                for (;;)
                {
                    int blockCount = Volatile.Read(ref ctx.EntityCount[blockIndex]);
                    if (blockCount == k_BlockBusy) { BlockSpinBackoff(ref spinIter); continue; }
                    // blockCount == 0 means all entities in this block are already free; the
                    // input entities must be stale. Skip the run gracefully, same as C++.
                    if (blockCount == 0) break;

                    if (Interlocked.CompareExchange(ref ctx.EntityCount[blockIndex], k_BlockBusy, blockCount) == blockCount)
                    {
                        ulong* allocated = BlockAllocated(blockIndex);
                        ulong* reserved  = BlockReserved(blockIndex);
                        int freed = 0;
                        for (int j = i; j < runEnd; ++j)
                        {
                            EntityId entity    = ids[j];
                            uint entityIndex   = entity.Index;
                            uint indexInBlock  = entityIndex & ctx.BlockMask;
                            ulong mask         = 1UL << (int)(indexInBlock & k_WordMask);
                            uint  wordIdx      = indexInBlock >> k_WordShift;

                            ref EntitySlot slot = ref GetSlot(entityIndex);
                            uint slotVersion    = SlotGetVersion(Volatile.Read(ref slot.versionAndChunk));

                            // Graceful skip: if the bit is clear (double-free or stale id) or the
                            // version doesn't match, just continue — mirrors C++ ReleaseEntityIds
                            // which does the same after AssertFormatMsg (non-fatal in release builds).
                            if ((allocated[wordIdx] & mask) == 0 || slotVersion != entity.Version) continue;

                            bool isReserved = (reserved[wordIdx] & mask) != 0;
                            Volatile.Write(ref slot.nativeObjectPtr, IntPtr.Zero);

                            if (!isReserved)
                            {
                                ulong vac        = Volatile.Read(ref slot.versionAndChunk);
                                uint  nextVersion = (SlotGetVersion(vac) + 1) & (uint)k_SlotVersionMask;
                                Volatile.Write(ref slot.versionAndChunk, SlotSetVersion(vac, nextVersion));
                            }

                            allocated[wordIdx] &= ~mask;
                            ++freed;
                        }

                        int prevVal = Interlocked.CompareExchange(ref ctx.EntityCount[blockIndex], blockCount - freed, k_BlockBusy);
                        Assert.AreEqual(k_BlockBusy, prevVal);
                        break;
                    }
                }
                i = runEnd;
            }
        }
    }
}
