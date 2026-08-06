// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.GraphToolkit.Editor;

class UserNodeViewBuilderLookup
{
    const BindingFlags k_ConstructorBindingFlags =
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    readonly Dictionary<Type, ConstructorInfo> m_ConstructorCache = new();

    public IUserNodeView Build(Node node, INodeView view)
    {
        if (node == null)
            return null;

        var nodeType = node.GetType();

        if (!m_ConstructorCache.TryGetValue(nodeType, out var constructor))
        {
            constructor = FindConstructor(nodeType);
            m_ConstructorCache[nodeType] = constructor;
        }

        if (constructor == null)
            return null;

        var instance = (IUserNodeView)constructor.Invoke(Array.Empty<object>());
        instance.Initialize(node, view);
        return instance;
    }

    static ConstructorInfo FindConstructor(Type nodeType)
    {
        var current = nodeType;
        while (current != null && typeof(Node).IsAssignableFrom(current))
        {
            foreach (var candidate in TypeCache.GetTypesDerivedFrom<IUserNodeView>())
            {
                if (candidate.IsAbstract || candidate.IsGenericTypeDefinition)
                    continue;

                if (!IsNodeViewOf(candidate, current))
                    continue;

                var constructor = candidate.GetConstructor(
                    k_ConstructorBindingFlags,
                    null,
                    Type.EmptyTypes,
                    null);

                if (constructor != null)
                    return constructor;
            }

            current = current.BaseType;
        }

        return null;
    }

    static bool IsNodeViewOf(Type candidate, Type nodeType)
    {
        var t = candidate;
        while (t != null && t != typeof(object))
        {
            if (t.IsGenericType
                && t.GetGenericTypeDefinition() == typeof(NodeView<>)
                && t.GetGenericArguments()[0] == nodeType)
                return true;
            t = t.BaseType;
        }
        return false;
    }

    internal class TestAccess
    {
        readonly UserNodeViewBuilderLookup m_Lookup;

        public TestAccess(UserNodeViewBuilderLookup lookup)
        {
            m_Lookup = lookup;
        }

        public int ConstructorCacheCount => m_Lookup.m_ConstructorCache.Count;

        public bool TryGetCachedConstructor(Type nodeType, out ConstructorInfo constructor)
        {
            return m_Lookup.m_ConstructorCache.TryGetValue(nodeType, out constructor);
        }
    }
}
