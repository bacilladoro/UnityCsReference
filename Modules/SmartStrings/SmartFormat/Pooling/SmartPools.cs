// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

//
// Copyright SmartFormat Project maintainers and contributors.
// Licensed under the MIT license.

using Unity.SmartStrings.Core.Formatting;
using Unity.SmartStrings.Core.Output;
using Unity.SmartStrings.Core.Parsing;
using UnityEngine.Pool;

namespace Unity.SmartStrings.Pooling.SmartPools;

/// <summary>The object pool for <see cref="FormatDetails"/>.</summary>
internal static class FormatDetailsPool
{
    public static readonly ObjectPool<FormatDetails> Pool = new(
        createFunc: () => new FormatDetails(),
        actionOnRelease: fd => fd.Clear());
}

/// <summary>The object pool for <see cref="Format"/>.</summary>
internal static class FormatPool
{
    public static readonly ObjectPool<Format> Pool = new(
        createFunc: () => new Format(),
        actionOnGet: fmt => fmt.OnTakenFromPool(),
        actionOnRelease: fmt => fmt.ReturnToPool());
}

/// <summary>The object pool for <see cref="FormattingInfo"/>.</summary>
internal static class FormattingInfoPool
{
    public static readonly ObjectPool<FormattingInfo> Pool = new(
        createFunc: () => new FormattingInfo(),
        actionOnRelease: fi => fi.ReturnToPool());
}

/// <summary>The object pool for <see cref="LiteralText"/>.</summary>
internal static class LiteralTextPool
{
    public static readonly ObjectPool<LiteralText> Pool = new(
        createFunc: () => new LiteralText(),
        actionOnRelease: lt => lt.Clear());
}

/// <summary>The object pool for <see cref="ParsingErrors"/>.</summary>
internal static class ParsingErrorsPool
{
    public static readonly ObjectPool<ParsingErrors> Pool = new(
        createFunc: () => new ParsingErrors(),
        actionOnRelease: pe => pe.Clear());
}

/// <summary>The object pool for <see cref="Placeholder"/>.</summary>
internal static class PlaceholderPool
{
    public static readonly ObjectPool<Placeholder> Pool = new(
        createFunc: () => new Placeholder(),
        actionOnRelease: ph => ph.ReturnToPool());
}

/// <summary>The object pool for <see cref="Selector"/>.</summary>
internal static class SelectorPool
{
    public static readonly ObjectPool<Selector> Pool = new(
        createFunc: () => new Selector(),
        actionOnRelease: selector => selector.Clear());
}

/// <summary>The object pool for <see cref="SplitList"/>.</summary>
internal static class SplitListPool
{
    public static readonly ObjectPool<SplitList> Pool = new(
        createFunc: () => new SplitList(),
        actionOnRelease: sl => sl.Clear());
}

/// <summary>The object pool for <see cref="StringOutput"/>.</summary>
internal static class StringOutputPool
{
    public static readonly ObjectPool<StringOutput> Pool = new(
        createFunc: () => new StringOutput(),
        actionOnRelease: so => so.Clear());
}
