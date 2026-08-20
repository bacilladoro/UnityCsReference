// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

namespace Unity.U2D.Physics.Editor
{
    sealed class PhysicsCoreProjectSettings2DProvider
    {
        static internal class ProjectSettingPath
        {
            public const string PhysicsCoreModule = "Project/Physics Core 2D";
        }

        static internal class AssetPath
        {
            public const string PhysicsCoreSettingsAsset = "ProjectSettings/PhysicsCoreProjectSettings2D.asset";
            public static readonly string PhysicsCoreSettingsAssetError = $"{nameof(CreateProjectSettingsProvider)} failed to load asset {PhysicsCoreSettingsAsset}.";
        }

        static internal class StyleSheetPath
        {
            public const string projectSettingsSheet = "PhysicsCore2D/StyleSheets/ProjectSettings.uss";
            public const string projectSettingsCommonSheet = "StyleSheets/ProjectSettings/ProjectSettingsCommon.uss";
            public const string commonSheet = "StyleSheets/Extensions/base/common.uss";
            public const string darkSheet = "StyleSheets/Extensions/base/dark.uss";
            public const string lightSheet = "StyleSheets/Extensions/base/light.uss";
        }

        static internal class UXMLPath
        {
            public const string physicsCoreProjectSettings2D = "PhysicsCore2D/UXML/PhysicsCoreProjectSettings2D.uxml";
            public const string physicsCoreSettings2D = "PhysicsCore2D/UXML/PhysicsCoreSettings2D.uxml";
        }

        // Create a new Physics Core Settings 2D asset via a save dialog, then assign it into the given field.
        static void CreateAndAssignSettingsAsset(SerializedObject serializedObject, ObjectField field)
        {
            var path = EditorUtility.SaveFilePanelInProject("Create Physics Core Settings 2D", "PhysicsCoreSettings2D", "asset",
                "Choose where to save the new Physics Core Settings 2D asset.");
            if (string.IsNullOrEmpty(path))
                return;

            var asset = ScriptableObject.CreateInstance<PhysicsCoreSettings2D>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();

            field.value = asset;
            serializedObject.ApplyModifiedProperties();
        }

        static SerializedObject LoadPhysicsCoreSettingsAsset()
        {
            var found = AssetDatabase.LoadAllAssetsAtPath(AssetPath.PhysicsCoreSettingsAsset);
            if (found == null)
                return null;

            return new SerializedObject(found[0]);
        }

        [SettingsProvider]
        internal static SettingsProvider CreateProjectSettingsProvider()
        {
            var provider = new SettingsProvider(ProjectSettingPath.PhysicsCoreModule, SettingsScope.Project)
            {
                label = "Physics Core 2D",
                keywords = SettingsProvider.GetSearchKeywordsFromPath(AssetPath.PhysicsCoreSettingsAsset),
                activateHandler = (searchContext, root) =>
                {
                    var serializedObject = LoadPhysicsCoreSettingsAsset();
                    if (serializedObject == null)
                    {
                        Debug.LogError(AssetPath.PhysicsCoreSettingsAssetError);
                        return;
                    }

                    // Create settings root.
                    var physicsCoreProjectSettingsUXML = EditorGUIUtility.Load(UXMLPath.physicsCoreProjectSettings2D) as VisualTreeAsset;
                    physicsCoreProjectSettingsUXML.CloneTree(root);

                    // Add styles.
                    var content = root.Q<ScrollView>(className: "project-settings-section-content");
                    content.styleSheets.Add(EditorGUIUtility.Load(StyleSheetPath.projectSettingsSheet) as StyleSheet);
                    content.styleSheets.Add(EditorGUIUtility.Load(StyleSheetPath.projectSettingsCommonSheet) as StyleSheet);
                    content.styleSheets.Add(EditorGUIUtility.Load(StyleSheetPath.commonSheet) as StyleSheet);
                    content.styleSheets.Add(EditorGUIUtility.Load(EditorGUIUtility.isProSkin ? StyleSheetPath.darkSheet : StyleSheetPath.lightSheet) as StyleSheet);

                    // Add core settings property.
                    {
                        var row = new VisualElement { style = { flexDirection = FlexDirection.Row } };

                        var coreProjectSettingsField = new ObjectField
                        {
                            label = "Physics Core Settings",
                            tooltip = "The active Physics Core Settings 2D.",
                            objectType = typeof(PhysicsCoreSettings2D),
                            bindingPath = "m_PhysicsCoreSettings",
                            style = { flexGrow = 1 }
                        };

                        // Increase the margin.
                        coreProjectSettingsField.style.marginLeft = 10;
                        coreProjectSettingsField.style.marginRight = 10;

                        // Show as inspector class.
                        coreProjectSettingsField.AddToClassList(InspectorElement.ussClassName);
                        row.Add(coreProjectSettingsField);

                        // Shown only while the slot is empty; creates a new asset via a save dialog and assigns it.
                        var makeAssetButton = new Button(() => CreateAndAssignSettingsAsset(serializedObject, coreProjectSettingsField))
                        {
                            text = "Make Asset",
                            tooltip = "Create a new Physics Core Settings 2D asset and assign it here.",
                            style = { marginRight = 10 }
                        };
                        row.Add(makeAssetButton);

                        content.Add(row);

                        void UpdateMakeAssetVisibility() =>
                            makeAssetButton.style.display = coreProjectSettingsField.value == null ? DisplayStyle.Flex : DisplayStyle.None;

                        // Ensure we read the change immediately.
                        coreProjectSettingsField.RegisterValueChangedCallback(_ =>
                        {
                            UpdateMakeAssetVisibility();
                            PhysicsEditorOnly.ReadProjectSettings();
                            PhysicsCoreSettings2DProvider.RefreshActiveSettingContent();
                        });

                        // Bind the project settings object, then set the initial visibility from the bound value.
                        // Binding assigns the field's value directly rather than raising a ChangeEvent, so the
                        // visibility must be calculated after binding rather than before it.
                        root.Bind(serializedObject);
                        UpdateMakeAssetVisibility();
                    }
                }
            };

            return provider;
        }
    }
}
