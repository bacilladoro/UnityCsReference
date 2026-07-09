// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// The comparison operator used by a <see cref="VariableConditionModel"/> to compare a variable against a value.
    /// </summary>
    internal enum ConditionComparison
    {
        /// <summary>
        /// The variable equals the value.
        /// </summary>
        Equal,

        /// <summary>
        /// The variable does not equal the value.
        /// </summary>
        NotEqual,

        /// <summary>
        /// The variable is less than the value.
        /// </summary>
        Less,

        /// <summary>
        /// The variable is less than or equal to the value.
        /// </summary>
        LessOrEqual,

        /// <summary>
        /// The variable is greater than the value.
        /// </summary>
        Greater,

        /// <summary>
        /// The variable is greater than or equal to the value.
        /// </summary>
        GreaterOrEqual,
    }
}
