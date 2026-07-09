// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Collections.Generic;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Helpers that determine which <see cref="ConditionComparison"/> operators apply to a given type.
    /// </summary>
    internal static class ConditionComparisonExtensions
    {
        static readonly ConditionComparison[] k_EqualityComparisons =
        {
            ConditionComparison.Equal,
            ConditionComparison.NotEqual,
        };

        static readonly ConditionComparison[] k_AllComparisons =
        {
            ConditionComparison.Equal,
            ConditionComparison.NotEqual,
            ConditionComparison.Less,
            ConditionComparison.LessOrEqual,
            ConditionComparison.Greater,
            ConditionComparison.GreaterOrEqual,
        };

        static readonly HashSet<Type> k_OrderedTypes = new(new[]
        {
            typeof(byte), typeof(sbyte),
            typeof(short), typeof(ushort),
            typeof(int), typeof(uint),
            typeof(long), typeof(ulong),
            typeof(float), typeof(double),
            typeof(char),
        });

        /// <summary>
        /// Whether a type supports ordering operators (less/greater), as opposed to equality only.
        /// </summary>
        /// <param name="typeHandle">The type to test.</param>
        /// <returns>True for numeric and enum types; false otherwise.</returns>
        public static bool SupportsOrdering(TypeHandle typeHandle)
        {
            var type = typeHandle.Resolve();
            return type != null && k_OrderedTypes.Contains(type);
        }

        /// <summary>
        /// Gets the comparison operators available for a given type.
        /// </summary>
        /// <param name="typeHandle">The type of the compared variable.</param>
        /// <returns>All six operators for ordered types; equality operators only otherwise.</returns>
        public static IReadOnlyList<ConditionComparison> GetAvailableComparisons(TypeHandle typeHandle)
        {
            return SupportsOrdering(typeHandle) ? k_AllComparisons : k_EqualityComparisons;
        }

        /// <summary>
        /// Gets the glyph used to display a <see cref="ConditionComparison"/>.
        /// </summary>
        /// <param name="comparison">The comparison operator.</param>
        /// <returns>The glyph representing the operator.</returns>
        public static string ToGlyph(this ConditionComparison comparison)
        {
            switch (comparison)
            {
                case ConditionComparison.Equal: return "=";
                case ConditionComparison.NotEqual: return "≠";
                case ConditionComparison.Less: return "<";
                case ConditionComparison.LessOrEqual: return "≤";
                case ConditionComparison.Greater: return ">";
                case ConditionComparison.GreaterOrEqual: return "≥";
                default: return comparison.ToString();
            }
        }
    }
}
