// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.CommandStateObserver;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Unity.GraphToolsFoundation.Editor
{
    /// <summary>
    /// Holds the selected graph elements in the current view, for the current graph.
    /// </summary>
    [Serializable]
    [MovedFrom(false, "Unity.GraphToolsFoundation.Editor", "Unity.GraphTools.Foundation.Editor")]
    sealed class SelectionStateComponent : PersistedStateComponent<SelectionStateComponent.StateUpdater>
    {
        /// <summary>
        /// An observer that updates the <see cref="SelectionStateComponent"/> when a graph is loaded.
        /// </summary>
        public class GraphLoadedObserver : StateObserver
        {
            ToolStateComponent m_ToolStateComponent;
            SelectionStateComponent m_SelectionStateComponent;

            /// <summary>
            /// Initializes a new instance of the <see cref="GraphLoadedObserver"/> class.
            /// </summary>
            public GraphLoadedObserver(ToolStateComponent toolStateComponent, SelectionStateComponent selectionStateComponent)
                : base(new [] { toolStateComponent},
                    new IStateComponent[] { selectionStateComponent })
            {
                m_ToolStateComponent = toolStateComponent;
                m_SelectionStateComponent = selectionStateComponent;
            }

            /// <inheritdoc />
            public override void Observe()
            {
                using (var obs = this.ObserveState(m_ToolStateComponent))
                {
                    if (obs.UpdateType != UpdateType.None)
                    {
                        using (var updater = m_SelectionStateComponent.UpdateScope)
                        {
                            updater.SaveAndLoadStateForGraph(m_ToolStateComponent.GraphModel);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updater for <see cref="SelectionStateComponent"/>.
        /// </summary>
        public class StateUpdater : BaseUpdater<SelectionStateComponent>
        {
            /// <summary>
            /// Marks graph elements as selected or unselected.
            /// </summary>
            /// <param name="graphElementModels">The graph elements to select or unselect.</param>
            /// <param name="select">True if the graph elements should be selected.
            /// False if the graph elements should be unselected.</param>
            public void SelectElements(IEnumerable<GraphElementModel> graphElementModels, bool select)
            {
                SelectElements(graphElementModels.Select(m => m.Guid), select);
            }

            /// <summary>
            /// Marks graph elements as selected or unselected.
            /// </summary>
            /// <param name="graphElementModelGuids">The graph elements guids to select or unselect.</param>
            /// <param name="select">True if the graph elements should be selected.
            /// False if the graph elements should be unselected.</param>
            public void SelectElements(IEnumerable<Hash128> graphElementModelGuids, bool select)
            {
                // Invalidate m_State.m_SelectedModels
                m_State.m_SelectedModels = null;

                var currentSelection = new HashSet<Hash128>(m_State.m_SelectionHashes);
                if (select)
                {
                    m_State.m_SelectionHashes = m_State.m_SelectionHashes.Concat(graphElementModelGuids).Distinct().ToList();
                }
                else
                {
                    foreach (var graphElementModelGuid in graphElementModelGuids)
                    {
                        m_State.m_SelectionHashes.Remove(graphElementModelGuid);
                    }
                }

                currentSelection.SymmetricExceptWith(m_State.m_SelectionHashes);
                if (currentSelection.Any())
                {
                    m_State.CurrentChangeset.ChangedModels.UnionWith(currentSelection);
                    m_State.SetUpdateType(UpdateType.Partial);
                }
            }

            /// <summary>
            /// Marks graph element as selected or unselected.
            /// </summary>
            /// <param name="graphElementModel">The graph element to select or unselect.</param>
            /// <param name="select">True if the graph element should be selected.
            /// False if the graph element should be unselected.</param>
            public void SelectElement(GraphElementModel graphElementModel, bool select)
            {
                if (select)
                {
                    if (m_State.m_SelectedModels != null && !m_State.m_SelectedModels.Contains(graphElementModel))
                        m_State.m_SelectedModels.Add(graphElementModel);

                    var guid = graphElementModel.Guid;
                    if (!m_State.m_SelectionHashes.Contains(guid))
                        m_State.m_SelectionHashes.Add(guid);
                }
                else
                {
                    if (m_State.m_SelectionHashes.Remove(graphElementModel.Guid))
                        m_State.m_SelectedModels?.Remove(graphElementModel);
                }

                m_State.CurrentChangeset.ChangedModels.Add(graphElementModel.Guid);
                m_State.SetUpdateType(UpdateType.Partial);
            }

            /// <summary>
            /// Unselects all graph elements.
            /// </summary>
            public void ClearSelection()
            {
                m_State.CurrentChangeset.ChangedModels.UnionWith(m_State.m_SelectionHashes);
                m_State.SetUpdateType(UpdateType.Partial);

                // If m_SelectedModels is not null, we maintain it. Otherwise, we let GetSelection rebuild it.
                m_State.m_SelectedModels?.Clear();
                m_State.m_SelectionHashes.Clear();
            }

            /// <summary>
            /// Saves the state component and replaces it by the state component associated with <paramref name="graphModel"/>.
            /// </summary>
            /// <param name="graphModel">The graph model for which we want to load a state component.</param>
            public void SaveAndLoadStateForGraph(GraphModel graphModel)
            {
                PersistedStateComponentHelpers.SaveAndLoadPersistedStateForGraph(m_State, this, graphModel);
            }
        }

        static IReadOnlyList<GraphElementModel> s_EmptyList = new List<GraphElementModel>();

        [SerializeField, Obsolete]
#pragma warning disable CS0618
        List<SerializableGUID> m_Selection;
#pragma warning restore CS0618

        // Source of truth
        [SerializeField]
        List<Hash128> m_SelectionHashes;

        // Cache of selected models, built using m_Selection, for use by GetSelection().
        List<GraphElementModel> m_SelectedModels;

        ChangesetManager<SimpleChangeset> m_ChangesetManager = new ChangesetManager<SimpleChangeset>();

        /// <inheritdoc />
        public override ChangesetManager ChangesetManager => m_ChangesetManager;

        SimpleChangeset CurrentChangeset => m_ChangesetManager.CurrentChangeset;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionStateComponent" /> class.
        /// </summary>
        public SelectionStateComponent()
        {
            m_SelectionHashes = new List<Hash128>();
            m_SelectedModels = null;
        }

        /// <summary>
        /// Gets a changeset that encompasses all changeset having a version larger than <paramref name="sinceVersion"/>.
        /// </summary>
        /// <param name="sinceVersion">The version from which to consider changesets.</param>
        /// <returns>The aggregated changeset.</returns>
        public SimpleChangeset GetAggregatedChangeset(uint sinceVersion)
        {
            return m_ChangesetManager.GetAggregatedChangeset(sinceVersion, CurrentVersion);
        }

        /// <summary>
        /// Returns true if the selection is empty.
        /// </summary>
        public bool IsSelectionEmpty => m_SelectionHashes.Count == 0;

        /// <summary>
        /// Gets the list of selected graph element models. If not done yet, this
        /// function resolves the list of models from a list of GUID, using the graph.
        /// </summary>
        /// <param name="graph">The graph containing the selected models.</param>
        /// <returns>A list of selected graph element models.</returns>
        public IReadOnlyList<GraphElementModel> GetSelection(GraphModel graph)
        {
            if (m_SelectedModels == null)
            {
                if (graph == null)
                {
                    return s_EmptyList;
                }

                m_SelectedModels = new List<GraphElementModel>();
                foreach (var guid in m_SelectionHashes)
                {
                    if (graph.TryGetModelFromGuid(guid, out var model))
                    {
                        Debug.Assert(model != null);
                        m_SelectedModels.Add(model);
                    }
                }
            }

            return m_SelectedModels;
        }

        /// <summary>
        /// Checks if the graph element model is selected.
        /// </summary>
        /// <param name="model">The model to check.</param>
        /// <returns>True if the model is selected. False otherwise.</returns>
        public bool IsSelected(GraphElementModel model)
        {
            return model != null && m_SelectionHashes.Contains(model.Guid);
        }

        /// <summary>
        /// Checks if the graph element model is selected.
        /// </summary>
        /// <param name="model">The model guid to check.</param>
        /// <returns>True if the model is selected. False otherwise.</returns>
        public bool IsSelected(Hash128 modelGuid)
        {
            return m_SelectionHashes.Contains(modelGuid);
        }

        /// <inheritdoc />
        protected override void Move(IStateComponent other, IChangeset changeset)
        {
            base.Move(other, changeset);

            if (other is SelectionStateComponent selectionStateComponent)
            {
                var changedModels = new HashSet<Hash128>(m_SelectionHashes);
                changedModels.SymmetricExceptWith(selectionStateComponent.m_SelectionHashes);
                CurrentChangeset.ChangedModels.UnionWith(changedModels);
                SetUpdateType(UpdateType.Partial);

                m_SelectionHashes = selectionStateComponent.m_SelectionHashes;
                m_SelectedModels = null;

                selectionStateComponent.m_SelectionHashes = null;
                selectionStateComponent.m_SelectedModels = null;
            }
        }

        /// <inheritdoc />
        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();

#pragma warning disable CS0612
#pragma warning disable CS0618
            m_Selection = new List<SerializableGUID>(m_SelectionHashes.Count);
            foreach (var guid in m_SelectionHashes)
            {
                m_Selection.Add(guid);
            }
#pragma warning restore CS0618
#pragma warning restore CS0612
        }

        /// <inheritdoc />
        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();

#pragma warning disable CS0612
            if (m_Selection != null)
            {
                m_SelectionHashes = new List<Hash128>(m_Selection.Count);
                foreach (var guid in m_Selection)
                {
                    m_SelectionHashes.Add(guid);
                }

                m_Selection = null;
            }
#pragma warning restore CS0612
        }
    }
}