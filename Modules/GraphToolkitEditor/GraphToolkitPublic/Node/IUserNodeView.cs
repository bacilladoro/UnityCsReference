// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using UnityEngine.UIElements;

namespace Unity.GraphToolkit.Editor;

interface IUserNodeView
{
    void Initialize(Node node, INodeView view);

    /// <summary>
    /// Called once, after the node's built-in UI is fully constructed and before its
    /// <see cref="VisualElement"/> is attached to the graph view.
    /// </summary>
    /// <remarks>
    /// This is the recommended entry point for allocating custom UI and adding it to
    /// <see cref="INodeView.Root"/>. Cache any allocated elements in fields; when the node returns from
    /// being culled, <see cref="INodeView.Root"/> is cleared and the cached elements must be re-added
    /// from <see cref="OnCullingChanged"/> (when <c>cullingEnabled</c> is <c>false</c>).
    /// </remarks>
    public void OnViewBuilt();

    /// <summary>
    /// Called when the <see cref="VisualElement"/> for your <see cref="Node"/> receives an
    /// <see cref="AttachToPanelEvent"/>.
    /// </summary>
    /// <remarks>
    /// This can fire multiple times during a node's lifetime — for example when the user tabs away from
    /// and back to the graph view, or when a block node is dragged and re-parented.
    /// </remarks>
    public void OnViewAttached();

    /// <summary>
    /// Called when the <see cref="VisualElement"/> for your <see cref="Node"/> receives a
    /// <see cref="DetachFromPanelEvent"/>.
    /// </summary>
    /// <remarks>
    /// Like <see cref="OnViewAttached"/>, this can fire multiple times during a node's lifetime, and a
    /// matching <see cref="OnViewAttached"/> may follow.
    /// </remarks>
    public void OnViewDetached();

    /// <summary>
    /// Called when the Level of Detail changes.
    /// </summary>
    /// <param name="zoom">Value equal to the x scale of the Graph View.</param>
    public void OnViewLODChanged(float zoom);

    /// <summary>
    /// Called when the node's culling state changes.
    /// </summary>
    /// <param name="cullingEnabled">
    /// <c>true</c> when the node is being culled; <c>false</c> when culling is being disabled and the node's
    /// <see cref="INodeView"/> is being repopulated.
    /// </param>
    /// <remarks>
    /// When <paramref name="cullingEnabled"/> is <c>false</c>, the contents of <see cref="INodeView.Root"/>
    /// have been cleared. Re-add the custom UI you allocated in <see cref="OnViewBuilt"/> — typically by
    /// calling <c>View.Root.Add(m_CachedElement)</c> on a cached field.
    /// </remarks>
    public void OnCullingChanged(bool cullingEnabled);
}
