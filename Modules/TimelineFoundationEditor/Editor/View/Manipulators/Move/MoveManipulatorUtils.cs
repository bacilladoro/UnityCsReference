// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

#pragma warning disable UAL0010,UAL0011,UAL0012,UAL0013,UAL0014 // AutoStaticsCleanup: TimelineFoundation not yet converted
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Timeline.Foundation.View.Internals
{
    static class MoveManipulatorUtils
    {
        static List<VisualElement> s_Picks = new List<VisualElement>();

        public static ItemElement FindItemFromTarget(VisualElement target, Vector2 mousePosition)
        {
            var itemElement = PickElement<ItemElement>(target, mousePosition);
            return itemElement is ISelectableElement
                || itemElement?.GetFirstOfType<ISelectableElement>() != null
                    ? itemElement
                    : null;
        }

        public static T PickElement<T>(VisualElement target, Vector2 mousePosition) where T : VisualElement
        {
            target.panel.PickAll(mousePosition, s_Picks);
            foreach (VisualElement element in s_Picks)
                if (element is T specificElement)
                    return specificElement;
            return null;
        }
    }
}
#pragma warning restore UAL0010,UAL0011,UAL0012,UAL0013,UAL0014
