// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace Unity.UIToolkit.Editor;

/// <summary>
/// An inherited (parent-document) stylesheet together with the document that owns it.
/// </summary>
internal readonly struct ParentStyleSheet
{
    public readonly StyleSheet StyleSheet;
    public readonly VisualTreeAsset OwningDocument;

    public ParentStyleSheet(StyleSheet styleSheet, VisualTreeAsset owningDocument)
    {
        StyleSheet = styleSheet;
        OwningDocument = owningDocument;
    }
}

/// <summary>
/// The editable document (<see cref="EditedAsset"/>) and its inherited parent stylesheets (<see cref="ParentStyleSheets"/>).
/// </summary>
internal readonly struct StyleSheetsContext
{
    public static readonly StyleSheetsContext None = default;

    /// <summary>The document whose stylesheets can be edited, or null when there is nothing editable (a read-only display).</summary>
    public readonly VisualTreeAsset EditedAsset;

    readonly ParentStyleSheet[] m_ParentStyleSheets;

    public StyleSheetsContext(VisualTreeAsset editedAsset, ParentStyleSheet[] parentStyleSheets)
    {
        EditedAsset = editedAsset;
        m_ParentStyleSheets = parentStyleSheets;
    }

    /// <summary>Inherited (read-only) parent-document stylesheets.</summary>
    public IReadOnlyList<ParentStyleSheet> ParentStyleSheets => m_ParentStyleSheets ?? Array.Empty<ParentStyleSheet>();
}

/// <summary>
/// Builds <see cref="StyleSheetsContext"/> instances from their source (currently a UI <see cref="VisualElementEditingStage"/>).
/// </summary>
internal static class StyleSheetsContextFactory
{
    public static StyleSheetsContext FromStage(VisualElementEditingStage stage)
    {
        if (stage == null)
            return StyleSheetsContext.None;

        return new StyleSheetsContext(stage.EditedVisualTreeAsset, CollectParents(stage.Context));
    }

    static ParentStyleSheet[] CollectParents(VisualTreeAssetEditingContext context)
    {
        if (context.SubDocumentOptions != SubDocumentOptions.InContext
            || context.SubDocumentPath == null
            || context.SubDocumentPath.Length == 0)
        {
            return [];
        }

        using var _ = HashSetPool<(VisualTreeAsset, StyleSheet)>.Get(out var collected);
        var parents = new List<ParentStyleSheet>();
        CollectFrom(context.RootVisualTreeAsset, collected, parents);
        for (var i = 0; i < context.SubDocumentPath.Length - 1; i++)
            CollectFrom(context.SubDocumentPath[i]?.ResolveTemplate(), collected, parents);

        return parents.ToArray();
    }

    static void CollectFrom(VisualTreeAsset vta, HashSet<(VisualTreeAsset, StyleSheet)> collected, List<ParentStyleSheet> parents)
    {
        if (vta == null)
            return;

        using var _ = ListPool<StyleSheet>.Get(out var sheets);
        vta.GetAllReferencedStyleSheets(sheets);
        foreach (var sheet in sheets)
        {
            if (sheet != null && collected.Add((vta, sheet)))
                parents.Add(new ParentStyleSheet(sheet, vta));
        }
    }
}
