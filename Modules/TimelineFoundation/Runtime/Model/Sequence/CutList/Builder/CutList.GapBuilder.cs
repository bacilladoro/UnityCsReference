// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

#pragma warning disable UAL0010,UAL0011,UAL0012,UAL0013,UAL0014 // AutoStaticsCleanup: TimelineFoundation not yet converted
using System;
using Unity.IntegerTime;
using Unity.Timeline.Foundation.Common;

namespace Unity.Timeline.Foundation.Model
{
    partial class CutList
    {
        public struct GapBuilder
        {
            public UniqueID id;
            public DiscreteTime duration;
            public IItemContent content;

            public GapBuilder(UniqueID id,
                              DiscreteTime duration = default,
                              IItemContent content = null)
            {
                this.id = id;
                this.duration = duration;
                this.content = content;
            }

            public static implicit operator ItemBuilder(in GapBuilder gap)
            {
                return new ItemBuilder(
                    gap.id,
                    ItemType.Gap,
                    DiscreteTime.Zero,
                    gap.duration,
                    UniqueID.Invalid,
                    DiscreteTime.Zero,
                    gap.content);
            }
        }
    }
}
#pragma warning restore UAL0010,UAL0011,UAL0012,UAL0013,UAL0014
