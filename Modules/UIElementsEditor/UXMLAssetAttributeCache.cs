// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

#pragma warning disable UAL0010,UAL0011,UAL0012,UAL0013,UAL0014 // AutoStaticsCleanup: UIToolkitFramework not yet converted
using System;
using System.Collections.Generic;
using Unity.Profiling;
using Unity.Scripting.LifecycleManagement;

namespace UnityEditor.UIElements
{
    class UxmlAssetAttributeCache
    {
        readonly Dictionary<string, Type> m_Cache = new();
        string m_CurrentTypeName;

        internal bool GetAssetAttributeType(string fullTypeName, string attributeName, out Type assetType)
        {
            LoadAssetAttributesForType(fullTypeName);
            return m_Cache.TryGetValue(attributeName, out assetType);
        }

        internal Dictionary<string, Type>.KeyCollection GetAssetAttributeNames(string fullTypeName)
        {
            LoadAssetAttributesForType(fullTypeName);
            return m_Cache.Keys;
        }

        [NoAutoStaticsCleanup]
        static ProfilerMarker s_RegisterMarker = new ProfilerMarker(ProfilerCategory.UIToolkit, "UxmlAssetAttributeCache.LoadAssetAttributesForType");

        void LoadAssetAttributesForType(string fullTypeName)
        {
            // Avoid reloading attribute info if the type is the same as we loaded last
            if (fullTypeName == m_CurrentTypeName)
                return;

            using var _ = s_RegisterMarker.Auto();

            m_Cache.Clear();
            m_CurrentTypeName = fullTypeName;

            static void CacheEnumerableSerialization(IEnumerable<UxmlSerializedAttributeDescription> attributes, Dictionary<string, Type> cache)
            {
                foreach (var description in attributes)
                {
                    if (description.isUnityObject)
                    {
                        cache[description.name] = description.type;
                    }
                }
            }

            var description = UxmlSerializedDataRegistry.GetDescription(m_CurrentTypeName);
            if (description != null && UxmlCodeDependencies.instance.HasAnyAssetAttributes(description))
            {
                CacheEnumerableSerialization(description.serializedAttributes, m_Cache);
            }
        }
    }
}
#pragma warning restore UAL0010,UAL0011,UAL0012,UAL0013,UAL0014
