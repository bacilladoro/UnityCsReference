// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

#pragma warning disable UAL0010,UAL0011,UAL0012,UAL0013,UAL0014 // AutoStaticsCleanup: TimelineFoundation not yet converted
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IntegerTime;

namespace Unity.Timeline.Foundation.Time
{
    class DiscreteTimeComparer : IComparer<DiscreteTime>, IEqualityComparer<DiscreteTime>
    {
        public static readonly DiscreteTimeComparer instance = new DiscreteTimeComparer();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(DiscreteTime x, DiscreteTime y)
        {
            if (x.Value < y.Value)
                return -1;
            return x.Value > y.Value ? 1 : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(DiscreteTime x, DiscreteTime y)
        {
            return x.Value == y.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode(DiscreteTime time)
        {
            return time.Value.GetHashCode();
        }
    }
}
#pragma warning restore UAL0010,UAL0011,UAL0012,UAL0013,UAL0014
