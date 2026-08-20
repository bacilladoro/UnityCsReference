// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

#pragma warning disable UAL0010,UAL0011,UAL0012,UAL0013,UAL0014 // AutoStaticsCleanup: TimelineFoundation not yet converted
using System;
using Unity.IntegerTime;
using Unity.Timeline.Foundation.Time;
using Unity.Timeline.Foundation.Common;

namespace Unity.Timeline.Foundation.Model.Internals
{
    struct Content
    {
        public IItemContent itemContent;
        public IItemContent transitionContent;

        public Content(IItemContent itemContent, IItemContent transitionContent)
        {
            this.itemContent = itemContent;
            this.transitionContent = transitionContent;
        }
    }

    struct ItemData : IEquatable<ItemData>
    {
        public static ItemData Invalid = new ItemData
        {
            id = UniqueID.Invalid,
            type = CutList.ItemType.Invalid,
            range = TimeRange.Empty,
            endTransitionId = UniqueID.Invalid,
            endTransitionDuration = DiscreteTime.Zero,
            content = default
        };

        public UniqueID id;
        public CutList.ItemType type;
        public TimeRange range;
        public UniqueID endTransitionId;
        public DiscreteTime endTransitionDuration;
        public Content content;

        public static bool operator ==(ItemData left, ItemData right)
        {
            return left.id == right.id;
        }

        public static bool operator !=(ItemData left, ItemData right)
        {
            return left.id != right.id;
        }

        public bool Equals(ItemData other)
        {
            return id == other.id;
        }

        public override bool Equals(object obj)
        {
            return obj is ItemData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }
}
#pragma warning restore UAL0010,UAL0011,UAL0012,UAL0013,UAL0014
