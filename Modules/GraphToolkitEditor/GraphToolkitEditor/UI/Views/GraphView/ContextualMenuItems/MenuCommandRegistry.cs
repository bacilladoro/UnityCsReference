// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Scripting.LifecycleManagement;
using UnityEditor;
using UnityEngine;

namespace Unity.GraphToolkit.Editor.ContextualMenuItems
{
    /// <summary>
    /// Discovers methods decorated with <see cref="GraphMenuAttribute"/>
    /// or <see cref="BlackboardMenuAttribute"/> and invokes them at
    /// menu-build time with the appropriate context.
    /// </summary>
    static partial class MenuCommandRegistry
    {
        [AutoStaticsCleanupOnCodeReload] // lazily rebuilt cache; cleared by Invalidate(), repopulated by EnsureBuilt()
        static Action<GraphMenuContext>[] s_GraphHandlers;
        [AutoStaticsCleanupOnCodeReload] // lazily rebuilt cache; cleared by Invalidate(), repopulated by EnsureBuilt()
        static Action<GraphMenuContext>[] s_BlackboardHandlers;

        internal static void InvokeGraphHandlers(GraphMenuContext context)
        {
            EnsureBuilt();
            var handlers = s_GraphHandlers;
            foreach (var action in handlers)
            {
                try
                {
                    action(context);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        internal static void InvokeBlackboardHandlers(GraphMenuContext context)
        {
            EnsureBuilt();
            var handlers = s_BlackboardHandlers;
            foreach (var action in handlers)
            {
                try
                {
                    action(context);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        /// <summary>
        /// Discards the cached invoker tables. Exposed so tests that add new
        /// decorated methods at runtime can force a refresh.
        /// </summary>
        internal static void Invalidate()
        {
            s_GraphHandlers = null;
            s_BlackboardHandlers = null;
        }

        static void EnsureBuilt()
        {
            if (s_GraphHandlers != null)
                return;

            s_GraphHandlers = BuildHandlers<GraphMenuAttribute>(static attr => attr.GraphType);
            s_BlackboardHandlers = BuildHandlers<BlackboardMenuAttribute>(static attr => attr.GraphType);
        }

        static Action<GraphMenuContext>[] BuildHandlers<TAttribute>(Func<TAttribute, Type> getGraphType)
            where TAttribute : Attribute
        {
            var contextType = typeof(GraphMenuContext);
            var attributeName = typeof(TAttribute).Name.Replace("Attribute", string.Empty);

            // Sort discovered methods by full name so menu order is stable
            // across runs and across .NET runtime versions.
            var methods = new List<MethodInfo>();
            foreach (var method in TypeCache.GetMethodsWithAttribute<TAttribute>())
                methods.Add(method);

            methods.Sort(static (a, b) => string.CompareOrdinal(FullName(a), FullName(b)));

            var handlers = new List<Action<GraphMenuContext>>(methods.Count);
            foreach (var method in methods)
            {
                if (TryBuildInvoker(method, contextType, attributeName, getGraphType, out var invoke))
                    handlers.Add(invoke);
            }
            return handlers.ToArray();
        }

        static bool TryBuildInvoker<TAttribute>(
            MethodInfo method,
            Type contextType,
            string attributeName,
            Func<TAttribute, Type> getGraphType,
            out Action<GraphMenuContext> invoke)
            where TAttribute : Attribute
        {
            invoke = null;

            if (!method.IsStatic)
            {
                Debug.LogWarning($"[{attributeName}] '{FullName(method)}' is ignored: the method must be static.");
                return false;
            }

            if (method.ReturnType != typeof(void))
            {
                Debug.LogWarning($"[{attributeName}] '{FullName(method)}' is ignored: the method must return void.");
                return false;
            }

            if (method.IsGenericMethodDefinition)
            {
                Debug.LogWarning($"[{attributeName}] '{FullName(method)}' is ignored: generic methods are not supported.");
                return false;
            }

            var parameters = method.GetParameters();
            if (parameters.Length != 1 || parameters[0].ParameterType != contextType)
            {
                Debug.LogWarning(
                    $"[{attributeName}] '{FullName(method)}' is ignored: the method must take a single " +
                    $"'{contextType.Name}' parameter.");
                return false;
            }

            var attributes = method.GetCustomAttributes<TAttribute>();
            var graphTypes = new List<Type>();
            foreach (var attribute in attributes)
            {
                var graphType = getGraphType(attribute);
                if (graphType != null)
                    graphTypes.Add(graphType);
            }

            if (graphTypes.Count == 0)
            {
                Debug.LogWarning(
                    $"[{attributeName}] '{FullName(method)}' is ignored: at least one non-null graph type must be specified.");
                return false;
            }

            var graphTypesArray = graphTypes.ToArray();
            var action = (Action<GraphMenuContext>)Delegate.CreateDelegate(typeof(Action<GraphMenuContext>), method);
            invoke = context =>
            {
                if (!IsGraphTypeSupported(context.Graph, graphTypesArray))
                    return;
                action(context);
            };
            return true;
        }

        static bool IsGraphTypeSupported(Graph graph, Type[] graphTypes)
        {
            if (graph == null)
                return false;
            var graphType = graph.GetType();
            foreach (var type in graphTypes)
            {
                if (type != null && type.IsAssignableFrom(graphType))
                    return true;
            }
            return false;
        }

        static string FullName(MethodInfo method)
        {
            return $"{method.DeclaringType?.FullName ?? "<unknown>"}.{method.Name}";
        }
    }
}
