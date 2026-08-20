// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

#pragma warning disable UAL0010,UAL0011,UAL0012,UAL0013,UAL0014 // AutoStaticsCleanup: UIToolkitFramework not yet converted
using Unity.Scripting.LifecycleManagement;
using System;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements.StyleSheets;

[VisibleToOtherModules("UnityEditor.UIBuilderModule")]
internal interface IStyleProfiler
{
    void BeginMatchingStyleSheet(StyleSheet styleSheet, SelectorAccelerationCacheEntry accelerationCacheEntry);
    void BeginMatchingElement(VisualElement element);
    void BeginMatchingSelector(StyleComplexSelector complexSelector);
    void EndMatchingSelector(StyleComplexSelector complexSelector, bool match, bool passedAncestorFilter);
    void EndMatchingStyleSheet(StyleSheet styleSheet);
}

static class StyleProfilerStorage<TProfilerType> where TProfilerType : struct, IStyleProfiler
{
    [NoAutoStaticsCleanup]
    static TProfilerType s_Instance;

    // Caution: only call this using ref InstanceByRef to avoid copying the struct
    public static ref TProfilerType InstanceByRef => ref s_Instance;
}

[VisibleToOtherModules("UnityEditor.UIBuilderModule", "UnityEditor.UIToolkitAuthoringModule")]
struct NoOpStyleProfiler : IStyleProfiler
{
    public void BeginMatchingStyleSheet(StyleSheet styleSheet, SelectorAccelerationCacheEntry accelerationCacheEntry)
    {
    }

    public void BeginMatchingElement(VisualElement element)
    {
    }

    public void BeginMatchingSelector(StyleComplexSelector complexSelector)
    {
    }

    public void EndMatchingSelector(StyleComplexSelector complexSelector, bool match, bool passedAncestorFilter)
    {
    }

    public void EndMatchingStyleSheet(StyleSheet styleSheet)
    {
    }
}
#pragma warning restore UAL0010,UAL0011,UAL0012,UAL0013,UAL0014
