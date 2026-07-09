// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using UnityEngine.Bindings;
using Object = UnityEngine.Object;

namespace UnityEngine.Serialization
{
    // Test-only harness for serialization performance benchmarks. Serializes a
    // UnityEngine.Object into a native scratch buffer and reads it back with the
    // caller's TransferInstructionFlags; the bytes stay native so write and read
    // are timed independently with no array marshalling. flags == 0 is the
    // PackEntityId (clone) arm; flags == 1 (kReadWriteFromSerializedFile) is the
    // resolver/remap arm. Internal — visible to the test assemblies via
    // InternalsVisibleTo (PlaymodeTests / Assembly-CSharp-testable). Not shipping code.
    [NativeHeader("Runtime/Serialize/SerializationPerfTestHarness.h")]
    internal static class SerializationPerfTestHarness
    {
        // kReadWriteFromSerializedFile (TransferInstructionFlags bit 0): set => the
        // remap arm (pack-off); clear => the inline PackEntityId arm (pack-on).
        public const int ReadWriteFromSerializedFile = 1;

        [FreeFunction("SerializationPerfTestHarness::SerializeToScratch")]
        public extern static int SerializeToScratch(Object obj, int flags);

        [FreeFunction("SerializationPerfTestHarness::DeserializeFromScratch")]
        public extern static void DeserializeFromScratch(Object target, int flags);
    }
}
