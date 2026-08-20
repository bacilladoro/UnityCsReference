// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

#pragma warning disable UAL0010,UAL0011,UAL0012,UAL0013,UAL0014 // AutoStaticsCleanup: TimelineFoundation not yet converted
using System;
using Unity.IntegerTime;
using Unity.Timeline.Foundation.Common;
using Unity.Timeline.Foundation.Model;
using Unity.Timeline.Foundation.Time;
using UnityEngine.Bindings;

namespace Unity.Timeline.Foundation.ViewModel
{
    [VisibleToOtherModules("UnityEditor.TimelineFoundationModule")]
    internal class Sequence : Stack
    {
        public static Sequence Invalid = new Sequence();

        public override UniqueID ID => model.ID;

        public ISequence model { get; }

        public DiscreteTime duration { get; private set; }
        public FrameRate frameRate { get; private set; } = FrameRate.k_60Fps;
        public bool isValid => model != null;

        Sequence()
        {
            model = null;
        }

        public Sequence(ISequence source)
        {
            model = source ?? throw new ArgumentNullException(nameof(source));
        }

        internal void UpdateMetadata_Internal()
        {
            duration = model.GetDuration();
            frameRate = model.GetFrameRate();
        }

        public override string ToString()
        {
            return model.ToString();
        }
    }
}
#pragma warning restore UAL0010,UAL0011,UAL0012,UAL0013,UAL0014
