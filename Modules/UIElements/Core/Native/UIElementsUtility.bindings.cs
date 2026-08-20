// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

#pragma warning disable UAL0010,UAL0011,UAL0012,UAL0013,UAL0014 // AutoStaticsCleanup: UIToolkitFramework not yet converted
using Unity.Scripting.LifecycleManagement;
using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.UIElements
{
    // This is the required interface to UIElementsUtility for Runtime game components.
    [NativeHeader("Modules/UIElements/Core/Native/UIElementsRuntimeUtilityNative.h")]
    internal static partial class UIElementsRuntimeUtilityNative
    {
        [AutoStaticsCleanupOnCodeReload]
        private static Action UpdatePanelsCallback;
        [AutoStaticsCleanupOnCodeReload]
        private static Action<bool> RepaintPanelsCallback;
        [AutoStaticsCleanupOnCodeReload]
        private static Action RenderOffscreenPanelsCallback;

        [RequiredByNativeCode]
        public static void UpdatePanels()
        {
            UpdatePanelsCallback?.Invoke();
        }

        [RequiredByNativeCode]
        public static void RepaintPanels(bool onlyOffscreen)
        {
            RepaintPanelsCallback?.Invoke(onlyOffscreen);
        }

        [RequiredByNativeCode]
        public static void RenderOffscreenPanels()
        {
            RenderOffscreenPanelsCallback?.Invoke();
        }

        public static void SetUpdateCallback(Action callback)
        {
            UpdatePanelsCallback = callback;
        }

        public static void SetRenderingCallbacks(Action<bool> repaintPanels, Action renderOffscreenPanels)
        {
            RepaintPanelsCallback = repaintPanels;
            RenderOffscreenPanelsCallback = renderOffscreenPanels;
            RegisterRenderingCallbacks();
        }

        public static void UnsetRenderingCallbacks()
        {
            RepaintPanelsCallback = null;
            RenderOffscreenPanelsCallback = null;
            UnregisterRenderingCallbacks();
        }

        private extern static void RegisterRenderingCallbacks();
        private extern static void UnregisterRenderingCallbacks();

        public extern static void VisualElementCreation();
    }
}
#pragma warning restore UAL0010,UAL0011,UAL0012,UAL0013,UAL0014
