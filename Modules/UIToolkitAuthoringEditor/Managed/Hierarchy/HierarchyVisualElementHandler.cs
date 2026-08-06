// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Hierarchy;
using Unity.Hierarchy.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.IO;
using Unity.Scripting.LifecycleManagement;
using Unity.UIToolkit.Editor.Utilities;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;

namespace Unity.UIToolkit.Editor;

[UsedImplicitly]
internal sealed partial class HierarchyVisualElementHandler : VisualElementNodeTypeHandler
{
    public const string NodeTypeName = "VisualElementHierarchyHandler";

    [InitializeOnLoadMethod, UsedImplicitly]
    private static void RegisterStageHandlers()
    {
        if (StageUtility.GetCurrentStage() is MainStage)
            HierarchyWindow.RegisterNodeTypeHandler<HierarchyVisualElementHandler>();
        StageNavigationManager.instance.stageChanging += OnStageWillChange;
    }

    [OnCodeUnloading, UsedImplicitly]
    private static void UnregisterHierarchyHandlers()
    {
        HierarchyWindow.UnregisterNodeTypeHandler<HierarchyVisualElementHandler>();
        HierarchyWindow.UnregisterNodeTypeHandler<VisualElementEditingNodeHandler>();
    }

    [/*BeforeManagedObjectsDisabled,*/ UsedImplicitly]
    private static void UnregisterStageHandlers()
    {
        StageNavigationManager.instance.stageChanging -= OnStageWillChange;
    }

    private HierarchyGameObjectHandler m_GameObjectHandler;

    public HierarchyVisualElementHandler()
    {
        var registry = VisualElementSelectionRegistry.Instance;
        if (registry != null)
        {
            registry.PanelTracked += RegisterPanel;
            registry.PanelUntracked += UnregisterPanel;
        }
    }

    /// <inheritdoc cref="HierarchyNodeTypeHandlerBase.GetNodeTypeName"/>
    public override string GetNodeTypeName()
    {
        return NodeTypeName;
    }

    protected override void Initialize()
    {
        base.Initialize();
        m_GameObjectHandler = Hierarchy.GetNodeTypeHandler<HierarchyGameObjectHandler>();

        var registry = VisualElementSelectionRegistry.Instance;
        if (registry != null)
        {
            // Register already tracked panels in case the registry was already initialized. That way, we won't
            // process the same panel more than once.
            var trackedPanels = registry.TrackedScenePanels;
            for (var i = 0; i < trackedPanels.Count; ++i)
                RegisterPanel(trackedPanels[i]);
            registry.EnsureInitialized();
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        var registry = VisualElementSelectionRegistry.Instance;
        if (registry != null)
        {
            registry.PanelTracked -= RegisterPanel;
            registry.PanelUntracked -= UnregisterPanel;
        }
    }

    protected override NodeCreationType ShouldCreateNode(VisualElement element)
    {
        return element is PanelRootElement
            ? NodeCreationType.CreateChildren
            : base.ShouldCreateNode(element);
    }

    protected override bool TryGetScopeKey(VisualElement element, out string scopeKey)
    {
        scopeKey = null;

        // Scene documents are scoped by their owning panel component: multiple documents in one
        // panel (or two instances of the same UXML) must not collide on identical in-document paths.
        var rootElement = element as IPanelComponentRootElement
                          ?? element?.GetFirstAncestorOfType<IPanelComponentRootElement>();
        if (rootElement?.panelComponent is not UnityEngine.Object componentObject || !componentObject)
            return false;

        scopeKey = GetGlobalObjectIdScopeKey(componentObject);
        return !string.IsNullOrEmpty(scopeKey);
    }

    protected override string GetDisplayName(HierarchyView view, in HierarchyNode node, VisualElement target)
    {
        if (target is IPanelComponentRootElement rootElement)
        {
            var vta = rootElement.panelComponent.visualTreeAsset;
            if (!vta)
            {
                return "<none>.uxml";
            }

            var path = AssetDatabase.GetAssetPath(vta);
            if (string.IsNullOrEmpty(path))
            {
                if (string.IsNullOrEmpty(vta.name))
                    return "<unsaved file>.uxml";

                return $"{vta.name}.uxml";
            }

            return Path.GetFileName(path);
        }

        return base.GetDisplayName(view, in node, target);
    }

    protected override void Bind(HierarchyViewItem item, VisualElement element)
    {
        if (element is IPanelComponentRootElement)
        {
            // We just want to display the name of the file.
            return;
        }

        base.Bind(item, element);
    }

    protected override void BindNavigation(HierarchyViewItem item, VisualElement container)
    {
        base.BindNavigation(item, container);
        SetStageNodeNavigation(item, container);
    }

    protected override void UnbindNavigation(HierarchyViewItem item, VisualElement container)
    {
        base.UnbindNavigation(item, container);
        UnsetStageNodeNavigation(item);
    }

    protected override void PopulateContextMenu(HierarchyView view, in HierarchyNode node, VisualElement element, DropdownMenu menu)
    {
        menu.AppendAction("Frame Selection", _ => RequestFramingCommand.Execute(CommandSources.Hierarchy, element, orientToFace: false));
        menu.AppendAction("Frame and Align to View", _ => RequestFramingCommand.Execute(CommandSources.Hierarchy, element, orientToFace: true));
        menu.AppendSeparator();

        IPanelComponent panelComponent;
        VisualTreeAsset vtaSource;
        VisualElementAsset vea;

        if (element is IPanelComponentRootElement rootElement)
        {
            panelComponent = rootElement.panelComponent;
            vtaSource = panelComponent.visualTreeAsset;
            vea = vtaSource?.visualTree;
        }
        else
        {
            panelComponent = element.GetFirstAncestorOfType<IPanelComponentRootElement>().panelComponent;
            vtaSource = panelComponent.visualTreeAsset;
            vea = element.visualElementAsset;

            if (vea == null)
            {
                vea = element.GetFirstAncestorWhere(ve => ve.visualElementAsset != null).visualElementAsset;
            }
        }

        var isPanelComponentRootElement = element is IPanelComponentRootElement;

        var ancestorInstances = new List<TemplateAsset>();

        element.GenerateSubDocumentPath(ancestorInstances);

        if (isPanelComponentRootElement)
        {
            menu.AppendAction(
                "Select VisualTreeAsset Asset",
                ma => { EditorGUIUtility.PingObject(ma.userData as VisualTreeAsset); },
                ma => (ma.userData as VisualTreeAsset) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled,
                vtaSource);
        }

        if (vtaSource != null)
        {
            // For a TemplateContainer, Open in UI Builder navigates to the template's own file, not the containing document.
            // The root element must be checked first: it is also a TemplateContainer but has no backing TemplateAsset.
            VisualTreeAsset openInBuilderVta;
            int openInBuilderSelectedId;
            if (isPanelComponentRootElement)
            {
                openInBuilderVta = vtaSource;
                openInBuilderSelectedId = vea?.id ?? -1;
            }
            else if (element is TemplateContainer templateContainer)
            {
                openInBuilderVta = (templateContainer.visualElementAsset as TemplateAsset)?.ResolveTemplate() ?? templateContainer.templateSource ?? vtaSource;
                openInBuilderSelectedId = -1;
            }
            else
            {
                openInBuilderVta = element.visualTreeAssetSource
                    ? element.visualTreeAssetSource
                    : element.GetFirstAncestorWhere(ve => ve.visualTreeAssetSource)?.visualTreeAssetSource ?? vtaSource;
                openInBuilderSelectedId = vea?.id ?? -1;
            }

            StageContextMenuUtility.PopulateOpenActions(menu, element, openInBuilderVta, openInBuilderSelectedId, ancestorInstances);

            if (ancestorInstances.Count == 0)
            {
                menu.AppendAction(
                    "Open Asset",
                    _ =>
                    {
                        VisualElementEditingStage.GoToStage(new VisualTreeAssetEditingContext(
                            vtaSource,
                            element.GetPanelSettings()
                        ), BreadcrumbBar.SeparatorStyle.Arrow);
                        UIToolkitStageUtility.RequestSelectionOnNextUpdate(new[] { vea });
                    });
            }

            var canBeReferenced = VisualElementReferenceTools.TryCreateReference(element, out var pr, out var authoringIdPath, false, true) && authoringIdPath.path.Length > 0;
            menu.AppendAction(
                "Find References In Scene",
                a =>
                {
                    var prId = pr.GetEntityId().GetHashCode();
                    var pathString = authoringIdPath.PathToCsvString(VisualElementReferenceSceneQueryEngineFilter.PathSeperatorToken);
                    var filter = $"ref={prId} {VisualElementReferenceSceneQueryEngineFilter.FilterId}=[{pathString}]";
                    SearchableEditorWindow.SetSearchText(filter, HierarchyType.GameObjects);
                },
                canBeReferenced ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
        }

        menu.AppendSeparator();
        StageContextMenuUtility.PopulateElementOperations(menu);
    }
    protected override bool TryGetParentNode(VisualElement element, out HierarchyNode parentNode)
    {
        if (element is not IPanelComponentRootElement rootElement)
            return base.TryGetParentNode(element, out parentNode);

        var panelComponentGameObject = rootElement.panelComponent.gameObject;
        parentNode = m_GameObjectHandler.GetOrCreateNode(panelComponentGameObject);
        return parentNode != HierarchyNode.Null;
    }

    private (List<VisualTreeAsset> vtAssets, List<TemplateAsset> instances) GetTemplateChain(VisualElement element)
    {
        var vtAssets = new List<VisualTreeAsset>();
        var instances = new List<TemplateAsset>();
        if (element is not TemplateContainer) return (vtAssets, instances);

        var elementTemplateSource = (element as TemplateContainer)?.templateSource;
        var elementVEATemplate = element.visualElementAsset as TemplateAsset;

        vtAssets.Add(elementTemplateSource);
        instances.Add(elementVEATemplate);

        var leafElement = element;
        while (leafElement?.GetFirstAncestorOfType<TemplateContainer>() != null)
        {
            var templateContainer = leafElement.GetFirstAncestorOfType<TemplateContainer>();
            if (templateContainer is IPanelComponentRootElement) break;

            if (templateContainer != null && templateContainer.templateSource != null)
                vtAssets.Add(templateContainer.templateSource);

            if (templateContainer?.visualElementAsset is TemplateAsset templateAsset)
                instances.Add(templateAsset);

            leafElement = templateContainer;
        }

        return (vtAssets, instances);
    }

    private static void OnStageWillChange(Stage previousStage, Stage nextStage)
    {
        switch (nextStage)
        {
            case MainStage:
                HierarchyWindow.RegisterNodeTypeHandler<HierarchyVisualElementHandler>();
                HierarchyWindow.UnregisterNodeTypeHandler<VisualElementEditingNodeHandler>();
                break;
            case VisualElementEditingStage:
                HierarchyWindow.RegisterNodeTypeHandler<VisualElementEditingNodeHandler>();
                HierarchyWindow.UnregisterNodeTypeHandler<HierarchyVisualElementHandler>();
                break;
            default:
                HierarchyWindow.UnregisterNodeTypeHandler<HierarchyVisualElementHandler>();
                HierarchyWindow.UnregisterNodeTypeHandler<VisualElementEditingNodeHandler>();
                break;
        }
        IntegratedAuthoringWorkflow.OnStageChanged(previousStage, nextStage);
    }
}
