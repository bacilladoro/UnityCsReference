// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System.Collections.Generic;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Interface for a condition that groups other conditions.
    /// </summary>
    /// <remarks>
    /// A group condition combines its nested conditions with the logical operation specified by
    /// <see cref="Operation"/>. Nested conditions can themselves be group conditions, forming a
    /// tree that expresses arbitrary boolean logic. To traverse the tree, enumerate
    /// <see cref="Get"/> and check whether each element is itself an <see cref="IGroupCondition"/>.
    /// This interface is implemented by Unity and is not intended to be implemented by user code.
    /// </remarks>
    public interface IGroupCondition : ICondition
    {
        /// <summary>
        /// The logical operation applied to the nested conditions.
        /// </summary>
        GroupConditionOperation Operation { get; }

        /// <summary>
        /// Retrieves the conditions nested in this group.
        /// </summary>
        /// <returns>The nested conditions, in the order they appear in the group.</returns>
        IEnumerable<ICondition> Get();
    }
}
