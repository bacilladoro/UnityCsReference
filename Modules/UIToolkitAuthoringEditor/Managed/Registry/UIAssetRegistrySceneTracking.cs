// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using Unity.Scripting.LifecycleManagement;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.UIToolkit.Editor;

/// <summary>
/// Bridges the scene panels tracked by <see cref="VisualElementSelectionRegistry"/> into
/// <see cref="UIAssetRegistry"/> as read-only open assets, so the registry knows about every
/// <see cref="VisualTreeAsset"/> / <see cref="StyleSheet"/> present in the loaded scenes (MainStage), not
/// just the top-level one. Editing tools upgrade the specific assets they can write to read-write.
/// </summary>
/// <remarks>
/// <see cref="VisualElementSelectionRegistry"/> raises <c>PanelTracked</c>/<c>PanelUntracked</c> only for
/// scene panels (gated by <see cref="UIToolkitAuthoringSettings.EnableInSceneUIAuthoring"/>); stage panels
/// manage their own registry tracking. The subscription is deferred until that registry has bootstrapped.
/// </remarks>
static partial class UIAssetRegistrySceneTracking
{
    [OnCodeLoaded]
    static void StartInit()
    {
        EditorApplication.delayCall += Init;
    }

    static void Init()
    {
        // Ensure the registry is alive so it reconciles external changes even before any tool opens an asset.
        _ = UIAssetRegistry.instance;

        var selection = VisualElementSelectionRegistry.Instance;
        if (selection == null)
        {
            // The selection registry has not bootstrapped yet; try again next tick.
            EditorApplication.delayCall += Init;
            return;
        }

        selection.PanelTracked += OnPanelTracked;
        selection.PanelUntracked += OnPanelUntracked;

        foreach (var panel in selection.TrackedScenePanels)
            OnPanelTracked(panel);
    }

    [OnCodeUnloading]
    static void Cleanup()
    {
        var selection = VisualElementSelectionRegistry.Instance;
        if (selection == null)
            return;
        selection.PanelTracked -= OnPanelTracked;
        selection.PanelUntracked -= OnPanelUntracked;
    }

    static void OnPanelTracked(Panel panel)
    {
        if (panel == null)
            return;

        UIAssetRegistry.instance.AttachPanel(
            panel,
            panel,
            roots => PanelDependencyTracker.CollectPanelComponentRoots(panel, roots));
    }

    static void OnPanelUntracked(Panel panel)
    {
        if (panel != null)
            UIAssetRegistry.instance.DetachPanel(panel);
    }
}
