// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.SmartStrings.Editor;

// Project Settings page (Project/Smart Strings) that shows the active settings, or a button to create them.
class SmartStringsSettingsProvider : AssetSettingsProvider
{
    public SmartStringsSettingsProvider()
        : base("Project/Smart Strings", () => SmartStringsEditorSettings.ActiveSettings)
    {
    }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        var root = new ScrollView
        {
            horizontalScrollerVisibility = ScrollerVisibility.Hidden,
            style = { marginLeft = 9, marginTop = 1 }
        };
        rootElement.Add(root);
        BuildContent(root);
    }

    static void BuildContent(VisualElement root)
    {
        root.Clear();
        root.Add(new Label(L10n.Tr("Smart Strings")) { style = { marginBottom = 12, fontSize = 19 } });

        var active = SmartStringsEditorSettings.ActiveSettings;
        if (active != null)
        {
            root.Add(new InspectorElement(active));
            return;
        }

        root.Add(new HelpBox(L10n.Tr("There are no active Smart Strings settings. Create one to customize the default formatter and include it in builds."), HelpBoxMessageType.Info));
        root.Add(new Button(() =>
        {
            var created = CreateSettingsAsset();
            if (created != null)
            {
                SmartStringsEditorSettings.ActiveSettings = created;
                BuildContent(root);
            }
        })
        {
            text = L10n.Tr("Create"),
            style = { width = 100, marginTop = 4 }
        });
    }

    static SmartStringsSettings CreateSettingsAsset()
    {
        var path = EditorUtility.SaveFilePanelInProject(L10n.Tr("Create Smart Strings Settings"), L10n.Tr("Smart Strings Settings"), "asset",
            L10n.Tr("Choose where to save the Smart Strings settings asset."));
        if (string.IsNullOrEmpty(path))
            return null;

        var settings = ScriptableObject.CreateInstance<SmartStringsSettings>();
        settings.SmartFormatter = Smart.CreateDefaultSmartFormat();
        AssetDatabase.CreateAsset(settings, path);
        AssetDatabase.SaveAssets();
        return settings;
    }

    [SettingsProvider]
    static SettingsProvider CreateProvider() => new SmartStringsSettingsProvider();
}
