// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

#pragma warning disable UAL0010,UAL0011,UAL0012,UAL0013,UAL0014 // AutoStaticsCleanup: UIToolkitFramework not yet converted
namespace UnityEngine.UIElements.StyleSheets;

partial class InitialStyle
{
    // The generated s_InitialStyle is [NoAutoStaticsCleanup]: Release, registered here, owns the
    // teardown by dropping the ComputedStyle native refcount.
    static InitialStyle()
    {
        UnloadingUtility.SubscribeToUnloading(UnloadingSubscriber.InitialStyle, Release);
        Initialize();
    }
}
#pragma warning restore UAL0010,UAL0011,UAL0012,UAL0013,UAL0014
