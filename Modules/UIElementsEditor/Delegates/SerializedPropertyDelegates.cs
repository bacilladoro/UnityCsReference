// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

#pragma warning disable UAL0010,UAL0011,UAL0012,UAL0013,UAL0014 // AutoStaticsCleanup: UIToolkitFramework not yet converted
using System;
using Unity.Scripting.LifecycleManagement;

namespace UnityEditor.UIElements
{
    internal class SerializedPropertyDelegates
    {
        [NoAutoStaticsCleanup]
        internal static Func<SerializedProperty, bool> IsPropertyValid = property => property.isValid;
    }
}
#pragma warning restore UAL0010,UAL0011,UAL0012,UAL0013,UAL0014
