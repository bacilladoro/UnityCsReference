#pragma warning disable UAL0010,UAL0011,UAL0012,UAL0013,UAL0014 // AutoStaticsCleanup: ScriptingRuntime not yet converted
using System.Reflection;
using System.Runtime.CompilerServices;
using PreserveAttribute = Unity.Private.Scripting.PreserveAttribute;

[assembly: InternalsVisibleTo("UnityEditor.BurstModule")]
namespace Unity.Scripting.LifecycleManagement
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal sealed class BurstInitializeAttribute : LifecycleAttributeBase
    {
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal sealed class BurstCleanupAttribute : LifecycleAttributeBase
    {
    }

    internal sealed class BurstScope : ImplicitLifecycleScope
    {
        public static BurstScope Instance { get; } = new();

        private BurstScope() : base(nameof(BurstScope))
        {
        }
        protected override void Enter(ScopeTransitionHelper scopeTransitionHelper)
        {
            scopeTransitionHelper.ExecuteMethodsInOrder<BurstInitializeAttribute>();
        }

        protected override void Exit(ScopeTransitionHelper scopeTransitionHelper)
        {
            scopeTransitionHelper.ExecuteMethodsInReverseOrder<BurstCleanupAttribute>();
        }
    }
}
#pragma warning restore UAL0010,UAL0011,UAL0012,UAL0013,UAL0014
