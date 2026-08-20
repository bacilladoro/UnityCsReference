// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

#pragma warning disable UAL0010,UAL0011,UAL0012,UAL0013,UAL0014 // AutoStaticsCleanup: TimelineFoundation not yet converted
using System;

namespace Unity.Timeline.Foundation.Widgets
{
    static class UIResources
    {
        const string k_AssemblyPath = "TimelineFoundation/Widgets/";
        const string k_TemplatePath = k_AssemblyPath + "templates/";
        const string k_StylesheetPath = k_AssemblyPath + "stylesheets/";

        public static readonly TemplateResourceFactory TemplateFactory = new(k_TemplatePath);
        public static readonly StylesheetResourceFactory StylesheetFactory = new(k_StylesheetPath);

        public static readonly StylesheetResource CommonStylesheet = StylesheetFactory.Get("common");
        public static readonly StylesheetResource OverlayStylesheet = StylesheetFactory.Get("Overlays");
    }
}
#pragma warning restore UAL0010,UAL0011,UAL0012,UAL0013,UAL0014
