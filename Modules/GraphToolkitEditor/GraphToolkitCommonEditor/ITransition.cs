// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System.Collections.Generic;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Interface for a transition in a <see cref="StateMachine"/>.
    /// </summary>
    /// <remarks>
    /// A transition represents the connection from a source <see cref="IState"/> to a destination
    /// <see cref="IState"/>, or a self transition anchored on a single state (in which case
    /// <see cref="FromState"/> and <see cref="ToState"/> are the same state, and the transition is an
    /// <see cref="ISelfTransition"/>). A transition can carry several stacked rules; enumerate
    /// <see cref="GetRules"/> to inspect each <see cref="ITransitionRule"/> and the conditions under which
    /// the transition is taken.
    /// This interface is implemented by Unity and is not intended to be implemented by user code.
    /// </remarks>
    public interface ITransition
    {
        /// <summary>
        /// The globally unique identifier for this transition.
        /// </summary>
        Hash128 ID { get; }

        /// <summary>
        /// The state the transition originates from.
        /// </summary>
        IState FromState { get; }

        /// <summary>
        /// The state the transition goes to. This is the same state as <see cref="FromState"/> for a self transition.
        /// </summary>
        IState ToState { get; }

        /// <summary>
        /// Retrieves the rules stacked on this transition, in the order they appear.
        /// </summary>
        /// <remarks>
        /// A single transition can hold several <see cref="ITransitionRule"/>s. Each rule has its own set of
        /// conditions; the transition is taken when the conditions of one of its enabled rules are met.
        /// </remarks>
        /// <returns>The rules stacked on this transition, in the order they appear.</returns>
        IEnumerable<ITransitionRule> GetRules();
    }
}
