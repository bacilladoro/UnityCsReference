// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

#pragma warning disable UAL0010,UAL0011,UAL0012,UAL0013,UAL0014 // AutoStaticsCleanup: UIToolkitFramework not yet converted
using Unity.Scripting.LifecycleManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace UnityEngine.UIElements
{
    /// <summary>
    /// Represents text rendering settings for a specific UI panel.
    /// <seealso cref="PanelSettings.textSettings"/>
    /// </summary>
    [HelpURL("UIE-text-setting-asset")]
    public partial class PanelTextSettings : TextSettings
    {
        [AutoStaticsCleanupOnCodeReload]
        private static PanelTextSettings s_DefaultPanelTextSettings;

        internal static PanelTextSettings defaultPanelTextSettings
        {
            get
            {
                InitializeDefaultPanelTextSettingsIfNull();
                return s_DefaultPanelTextSettings;
            }
        }

        internal static void InitializeDefaultPanelTextSettingsIfNull()
        {
            if (s_DefaultPanelTextSettings == null)
            {
                s_DefaultPanelTextSettings = ScriptableObject.CreateInstance<PanelTextSettings>();
            }
        }
    }
}
#pragma warning restore UAL0010,UAL0011,UAL0012,UAL0013,UAL0014
