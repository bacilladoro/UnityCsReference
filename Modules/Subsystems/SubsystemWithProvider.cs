// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using Unity.Profiling;

namespace UnityEngine.SubsystemsImplementation
{
    public abstract class SubsystemWithProvider : ISubsystem
    {
        static readonly ProfilerMarker k_StartMarker = new ProfilerMarker("SubsystemWithProvider.Start");
        static readonly ProfilerMarker k_StopMarker = new ProfilerMarker("SubsystemWithProvider.Stop");
        static readonly ProfilerMarker k_DestroyMarker = new ProfilerMarker("SubsystemWithProvider.Destroy");

        public void Start()
        {
            using var _ = k_StartMarker.Auto();
            if (running)
                return;

            OnStart();
            providerBase.m_Running = true;
            running = true;
        }

        protected abstract void OnStart();

        public void Stop()
        {
            using var _ = k_StopMarker.Auto();
            if (!running)
                return;

            OnStop();
            providerBase.m_Running = false;
            running = false;
        }

        protected abstract void OnStop();

        public void Destroy()
        {
            using var _ = k_DestroyMarker.Auto();
            Stop();
            if (SubsystemManager.RemoveStandaloneSubsystem(this))
                OnDestroy();
        }

        protected abstract void OnDestroy();

        public bool running { get; private set; }
        internal SubsystemProvider providerBase { get; set; }

        internal abstract void Initialize(SubsystemDescriptorWithProvider descriptor, SubsystemProvider subsystemProvider);
        internal abstract SubsystemDescriptorWithProvider descriptor { get; }
    }

    public abstract class SubsystemWithProvider<TSubsystem, TSubsystemDescriptor, TProvider> : SubsystemWithProvider
        where TSubsystem : SubsystemWithProvider, new()
        where TSubsystemDescriptor : SubsystemDescriptorWithProvider
        where TProvider : SubsystemProvider<TSubsystem>
    {
        static readonly ProfilerMarker k_InitializeMarker = new ProfilerMarker("SubsystemWithProvider.Initialize");
        static readonly ProfilerMarker k_CreateMarker = new ProfilerMarker("SubsystemWithProvider.OnCreate");

        public TSubsystemDescriptor subsystemDescriptor { get; private set; }

        protected internal TProvider provider { get; private set; }

        protected virtual void OnCreate() {}
        protected override void OnStart() => provider.Start();
        protected override void OnStop() => provider.Stop();
        protected override void OnDestroy() => provider.Destroy();

        internal override sealed void Initialize(SubsystemDescriptorWithProvider descriptor, SubsystemProvider provider)
        {
            using var _ = k_InitializeMarker.Auto();
            providerBase = provider;
            this.provider = (TProvider)provider;
            subsystemDescriptor = (TSubsystemDescriptor)descriptor;
            using (k_CreateMarker.Auto())
                OnCreate();
        }

        internal override sealed SubsystemDescriptorWithProvider descriptor => subsystemDescriptor;
    }

    namespace Extensions
    {
        public static class SubsystemExtensions
        {
            public static TProvider GetProvider<TSubsystem, TDescriptor, TProvider>(
                this SubsystemWithProvider<TSubsystem, TDescriptor, TProvider> subsystem)
                where TSubsystem : SubsystemWithProvider, new()
                where TDescriptor : SubsystemDescriptorWithProvider<TSubsystem, TProvider>
                where TProvider : SubsystemProvider<TSubsystem>
            {
                return subsystem.provider;
            }
        }
    }
}
