// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.GraphToolkit.Editor;

/// <summary>
/// Describes a single port that was added, removed, or modified, paired with the categories of changes that affected it.
/// </summary>
/// <remarks>
/// When the port was removed from the graph, <see cref="Port"/> is `null` and only <see cref="ID"/> identifies it.
/// <br/>
/// <br/>
/// For a usage example, see <see cref="GraphChanges"/>.
/// </remarks>
public readonly struct ChangedPort
{
    readonly IPort m_Port;
    readonly Hash128 m_ID;
    readonly ChangeKind m_ChangeKinds;

    internal ChangedPort(IPort port, Hash128 id, ChangeKind changeKinds)
    {
        m_Port = port;
        m_ID = id;
        m_ChangeKinds = changeKinds;
    }

    /// <summary>The port that was modified, or `null` if the port was removed.</summary>
    public IPort Port => m_Port;

    /// <summary>The unique identifier of the port.</summary>
    public Hash128 ID => m_ID;

    /// <summary>
    /// The categories of changes that affected this port, as a set of <see cref="ChangeKind"/> flags.
    /// </summary>
    /// <remarks>
    /// Contains <see cref="ChangeKind.Added"/> when the port was added, <see cref="ChangeKind.Removed"/> when the
    /// port was removed, in addition to any change categories reported by the graph (e.g. <see cref="ChangeKind.Data"/>,
    /// <see cref="ChangeKind.Layout"/>).
    /// </remarks>
    public ChangeKind ChangeKinds => m_ChangeKinds;
}

/// <summary>
/// Describes a single node that was added, or modified, paired with the categories of changes that affected it
/// and any of its ports that were also affected.
/// </summary>
/// <remarks>
/// A removed node does not have a matching <see cref="ChangedNode"/>.
/// <br/>
/// <br/>
/// <see cref="Node"/> is either a user-defined <see cref="Editor.Node"/> subclass or an <see cref="IVariableNode"/>.
/// <br/>
/// <br/>
/// For a usage example, see <see cref="GraphChanges"/>.
/// </remarks>
public readonly struct ChangedNode
{
    readonly INode m_Node;
    readonly Hash128 m_ID;
    readonly ChangeKind m_ChangeKinds;
    readonly IReadOnlyList<ChangedPort> m_ChangedPorts;

    internal ChangedNode(INode node, Hash128 id, ChangeKind changeKinds, IReadOnlyList<ChangedPort> changedPorts)
    {
        m_Node = node;
        m_ID = id;
        m_ChangeKinds = changeKinds;
        m_ChangedPorts = changedPorts;
    }

    /// <summary>The node that was modified, or `null` if the node was removed.</summary>
    public INode Node => m_Node;

    /// <summary>The unique identifier of the node.</summary>
    public Hash128 ID => m_ID;

    /// <summary>
    /// The categories of changes that affected this node, as a set of <see cref="ChangeKind"/> flags.
    /// </summary>
    /// <remarks>
    /// Contains <see cref="ChangeKind.Added"/> when the node was added, <see cref="ChangeKind.Removed"/> when the
    /// node was removed, and <see cref="ChangeKind.PortChanged"/> when any port on the node changed
    /// (inspect <see cref="ChangedPorts"/> for details), in addition to any change categories reported by the
    /// graph (e.g. <see cref="ChangeKind.Data"/>, <see cref="ChangeKind.Layout"/>).
    /// </remarks>
    public ChangeKind ChangeKinds => m_ChangeKinds;

    /// <summary>Ports of this node that were added, removed, or modified.</summary>
    public IReadOnlyList<ChangedPort> ChangedPorts => m_ChangedPorts ?? Array.Empty<ChangedPort>();
}

/// <summary>
/// Describes a single variable that was added, or modified, paired with the categories of changes that affected it.
/// </summary>
/// <remarks>
/// A removed <see cref="IVariable"/> does not have a matching `ChangedVariable`.
/// <br/>
/// <br/>
/// For a usage example, see <see cref="GraphChanges"/>.
/// </remarks>
public readonly struct ChangedVariable
{
    readonly IVariable m_Variable;
    readonly Hash128 m_ID;
    readonly ChangeKind m_ChangeKinds;

    internal ChangedVariable(IVariable variable, Hash128 id, ChangeKind changeKinds)
    {
        m_Variable = variable;
        m_ID = id;
        m_ChangeKinds = changeKinds;
    }

    /// <summary>The variable that was modified, or `null` if the variable was removed.</summary>
    public IVariable Variable => m_Variable;

    /// <summary>The unique identifier of the variable.</summary>
    public Hash128 ID => m_ID;

    /// <summary>
    /// The categories of changes that affected this variable, as a set of <see cref="ChangeKind"/> flags.
    /// </summary>
    /// <remarks>
    /// Contains <see cref="ChangeKind.Added"/> when the variable was added, <see cref="ChangeKind.Removed"/> when the
    /// variable was removed, in addition to any change categories reported by the graph (e.g. <see cref="ChangeKind.Data"/>).
    /// </remarks>
    public ChangeKind ChangeKinds => m_ChangeKinds;
}

/// <summary>
/// Describes a single constant node that was added, or modified, paired with the categories of changes that affected it.
/// </summary>
/// <remarks>
/// A removed <see cref="ConstantNode"/> does not have a matching <see cref="ChangedConstantNode"/>.
/// <br/>
/// <br/>
/// For a usage example, see <see cref="GraphChanges"/>.
/// </remarks>
public readonly struct ChangedConstantNode
{
    readonly IConstantNode m_ConstantNode;
    readonly Hash128 m_ID;
    readonly ChangeKind m_ChangeKinds;

    internal ChangedConstantNode(IConstantNode constantNode, Hash128 id, ChangeKind changeKinds)
    {
        m_ConstantNode = constantNode;
        m_ID = id;
        m_ChangeKinds = changeKinds;
    }

    /// <summary>The constant node that was modified, or `null` if the constant node was removed.</summary>
    public IConstantNode ConstantNode => m_ConstantNode;

    /// <summary>The unique identifier of the constant node.</summary>
    public Hash128 ID => m_ID;

    /// <summary>
    /// The categories of changes that affected this constant node, as a set of <see cref="ChangeKind"/> flags.
    /// </summary>
    /// <remarks>
    /// Contains <see cref="ChangeKind.Added"/> when the constant node was added, <see cref="ChangeKind.Removed"/>
    /// when the constant node was removed, in addition to any change categories reported by the graph
    /// (e.g. <see cref="ChangeKind.Data"/>).
    /// </remarks>
    public ChangeKind ChangeKinds => m_ChangeKinds;
}

/// <summary>
/// Describes a single subgraph node that was added, or modified, paired with the categories of changes that affected it.
/// </summary>
/// <remarks>
/// A removed <see cref="SubgraphNode"/> does not have a matching <see cref="ChangedSubgraphNode"/>.
/// <br/>
/// <br/>
/// For a usage example, see <see cref="GraphChanges"/>.
/// </remarks>
public readonly struct ChangedSubgraphNode
{
    readonly ISubgraphNode m_SubgraphNode;
    readonly Hash128 m_ID;
    readonly ChangeKind m_ChangeKinds;

    internal ChangedSubgraphNode(ISubgraphNode subgraphNode, Hash128 id, ChangeKind changeKinds)
    {
        m_SubgraphNode = subgraphNode;
        m_ID = id;
        m_ChangeKinds = changeKinds;
    }

    /// <summary>The subgraph node that was modified, or `null` if the subgraph node was removed.</summary>
    public ISubgraphNode SubgraphNode => m_SubgraphNode;

    /// <summary>The unique identifier of the subgraph node.</summary>
    public Hash128 ID => m_ID;

    /// <summary>
    /// The categories of changes that affected this subgraph node, as a set of <see cref="ChangeKind"/> flags.
    /// </summary>
    /// <remarks>
    /// Contains <see cref="ChangeKind.Added"/> when the subgraph node was added, <see cref="ChangeKind.Removed"/>
    /// when the subgraph node was removed, in addition to any change categories reported by the graph
    /// (e.g. <see cref="ChangeKind.Data"/>).
    /// </remarks>
    public ChangeKind ChangeKinds => m_ChangeKinds;
}

/// <summary>
/// The set of changes to a graph reported to <see cref="Graph.OnGraphChanged"/> in a single change event.
/// </summary>
/// <remarks>
/// Access this through <see cref="GraphLogger.GraphChanges"/> inside <see cref="Graph.OnGraphChanged"/>.
/// Inspect <see cref="ChangedNodes"/>, <see cref="ChangedVariables"/>, <see cref="ChangedConstantNodes"/>,
/// and <see cref="ChangedSubgraphNodes"/> to react to what was added, removed, or modified. Each entry's
/// `ChangeKinds` property is a set of <see cref="ChangeKind"/> bit-flags describing the categories
/// of change that apply.
/// </remarks>
/// <example>
/// <code lang="cs">
/// <![CDATA[
/// public override void OnGraphChanged(GraphLogger graphLogger)
/// {
///     GraphChanges changes = graphLogger.GraphChanges;
///
///     foreach (ChangedNode changedNode in changes.ChangedNodes)
///     {
///         // Removed nodes have a null Node; only ID is available.
///         if (changedNode.Node == null)
///         {
///             Debug.Log($"Node removed: {changedNode.ID}");
///             continue;
///         }
///
///         if ((changedNode.ChangeKinds & ChangeKind.Added) != 0)
///             Debug.Log($"Node added: {changedNode.Node}");
///
///         if ((changedNode.ChangeKinds & ChangeKind.Data) != 0)
///             Debug.Log($"Node data changed: {changedNode.Node}");
///
///         if ((changedNode.ChangeKinds & ChangeKind.Layout) != 0)
///             Debug.Log($"Node moved or resized: {changedNode.Node}");
///
///         // Node is either a user-defined Node subclass or an IVariableNode.
///         switch (changedNode.Node)
///         {
///             case IVariableNode variableNode: /* variable reference */ break;
///             case Node userNode:              /* user-defined node */  break;
///         }
///
///         // When ChangeKind.PortChanged is set, inspect ChangedPorts for details.
///         foreach (ChangedPort changedPort in changedNode.ChangedPorts)
///         {
///             if ((changedPort.ChangeKinds & ChangeKind.Removed) != 0)
///                 Debug.Log($"  Port removed: {changedPort.ID}");
///             else
///                 Debug.Log($"  Port changed: {changedPort.Port?.Name} ({changedPort.ChangeKinds})");
///         }
///     }
///
///     foreach (ChangedVariable changedVariable in changes.ChangedVariables)
///     {
///         if (changedVariable.Variable == null)
///             Debug.Log($"Variable removed: {changedVariable.ID}");
///         else
///             Debug.Log($"Variable changed: {changedVariable.Variable.Name} ({changedVariable.ChangeKinds})");
///     }
///
///     foreach (ChangedConstantNode changedConstant in changes.ChangedConstantNodes)
///     {
///         if (changedConstant.ConstantNode == null)
///             Debug.Log($"Constant node removed: {changedConstant.ID}");
///         else
///             Debug.Log($"Constant node changed: {changedConstant.ConstantNode} ({changedConstant.ChangeKinds})");
///     }
///
///     foreach (ChangedSubgraphNode changedSubgraph in changes.ChangedSubgraphNodes)
///     {
///         if (changedSubgraph.SubgraphNode == null)
///             Debug.Log($"Subgraph node removed: {changedSubgraph.ID}");
///         else
///             Debug.Log($"Subgraph node changed: {changedSubgraph.SubgraphNode} ({changedSubgraph.ChangeKinds})");
///     }
/// }
/// ]]>
/// </code>
/// </example>
public class GraphChanges
{
    IReadOnlyList<ChangedNode> m_ChangedNodes;
    IReadOnlyList<ChangedVariable> m_ChangedVariables;
    IReadOnlyList<ChangedConstantNode> m_ChangedConstantNodes;
    IReadOnlyList<ChangedSubgraphNode> m_ChangedSubgraphNodes;
    internal void SetChangeData(
        IReadOnlyList<ChangedNode> changedNodes,
        IReadOnlyList<ChangedVariable> changedVariables,
        IReadOnlyList<ChangedConstantNode> changedConstantNodes,
        IReadOnlyList<ChangedSubgraphNode> changedSubgraphNodes)
    {
        m_ChangedNodes = changedNodes ?? Array.Empty<ChangedNode>();
        m_ChangedVariables = changedVariables ?? Array.Empty<ChangedVariable>();
        m_ChangedConstantNodes = changedConstantNodes ?? Array.Empty<ChangedConstantNode>();
        m_ChangedSubgraphNodes = changedSubgraphNodes ?? Array.Empty<ChangedSubgraphNode>();
    }
    /// <summary>
    /// The nodes that were added, removed, or modified in this change event.
    /// </summary>
    /// <remarks>
    /// Each entry's <see cref="ChangedNode.ChangeKinds"/> indicates the kind of change (e.g. `"Added"`, `"Removed"`,
    /// `"PortChanged"`, or one of the graph's standard change categories).
    /// For removed nodes, <see cref="ChangedNode.Node"/> is `null` and only <see cref="ChangedNode.ID"/> identifies it.
    /// </remarks>
    public IReadOnlyList<ChangedNode> ChangedNodes => m_ChangedNodes ?? Array.Empty<ChangedNode>();

    /// <summary>
    /// The variables that were added, removed, or modified in this change event.
    /// </summary>
    /// <remarks>
    /// Each entry's <see cref="ChangedVariable.ChangeKinds"/> indicates the kind of change (e.g. `"Added"`, `"Removed"`,
    /// or one of the graph's standard change categories).
    /// For removed variables, <see cref="ChangedVariable.Variable"/> is `null` and only <see cref="ChangedVariable.ID"/> identifies it.
    /// </remarks>
    public IReadOnlyList<ChangedVariable> ChangedVariables => m_ChangedVariables ?? Array.Empty<ChangedVariable>();

    /// <summary>
    /// The constant nodes that were added, removed, or modified in this change event.
    /// </summary>
    /// <remarks>
    /// Each entry's <see cref="ChangedConstantNode.ChangeKinds"/> indicates the kind of change (e.g. `"Added"`,
    /// `"Removed"`, or one of the graph's standard change categories).
    /// For removed constant nodes, <see cref="ChangedConstantNode.ConstantNode"/> is `null` and only
    /// <see cref="ChangedConstantNode.ID"/> identifies it.
    /// </remarks>
    public IReadOnlyList<ChangedConstantNode> ChangedConstantNodes => m_ChangedConstantNodes ?? Array.Empty<ChangedConstantNode>();

    /// <summary>
    /// The subgraph nodes that were added, removed, or modified in this change event.
    /// </summary>
    /// <remarks>
    /// Each entry's <see cref="ChangedSubgraphNode.ChangeKinds"/> indicates the kind of change (e.g. `"Added"`,
    /// `"Removed"`, or one of the graph's standard change categories).
    /// For removed subgraph nodes, <see cref="ChangedSubgraphNode.SubgraphNode"/> is `null` and only
    /// <see cref="ChangedSubgraphNode.ID"/> identifies it.
    /// </remarks>
    public IReadOnlyList<ChangedSubgraphNode> ChangedSubgraphNodes => m_ChangedSubgraphNodes ?? Array.Empty<ChangedSubgraphNode>();
}
