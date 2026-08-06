// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.Playables;

namespace UnityEngine.Animations
{
    ///<summary>A <see cref="IPlayableOutput" /> implementation that connects the <see cref="PlayableGraph" /> to an <see cref="Animator" /> in the Scene.</summary>
    ///<remarks>**NOTE:** You can use <see cref="PlayableOutputExtensions" /> methods on AnimationPlayableOutput objects.</remarks>
    [NativeHeader("Modules/Animation/ScriptBindings/AnimationPlayableOutput.bindings.h")]
    [NativeHeader("Modules/Animation/Director/AnimationPlayableOutput.h")]
    [NativeHeader("Modules/Animation/Animator.h")]
    [NativeHeader("Runtime/Director/Core/HPlayableGraph.h")]
    [NativeHeader("Runtime/Director/Core/HPlayableOutput.h")]
    [StaticAccessor("AnimationPlayableOutputBindings", StaticAccessorType.DoubleColon)]
    [RequiredByNativeCode]
    public struct AnimationPlayableOutput : IPlayableOutput
    {
        private PlayableOutputHandle m_Handle;

        ///<summary>Creates an <see cref="AnimationPlayableOutput" /> in the <see cref="PlayableGraph" />.</summary>
        ///<remarks>The <see cref="Animator" /> plays the source <see cref="Playable" /> of the <see cref="AnimationPlayableOutput" />. This source Playable can be set with SetSourcePlayable.</remarks>
        ///<param name="graph">The <see cref="PlayableGraph" /> that will contain the <see cref="AnimationPlayableOutput" />.</param>
        ///<param name="name">The name of the output.</param>
        ///<param name="target">The <see cref="Animator" /> that will process the <see cref="PlayableGraph" />.</param>
        ///<returns>A new <see cref="AnimationPlayableOutput" /> attached to the <see cref="PlayableGraph" />.</returns>
        public static AnimationPlayableOutput Create(PlayableGraph graph, string name, Animator target)
        {
            PlayableOutputHandle handle;
            if (!AnimationPlayableGraphExtensions.InternalCreateAnimationOutput(ref graph, name, out handle))
                return AnimationPlayableOutput.Null;

            AnimationPlayableOutput output = new AnimationPlayableOutput(handle);
            output.SetTarget(target);

            return output;
        }

        internal AnimationPlayableOutput(PlayableOutputHandle handle)
        {
            if (handle.IsValid())
            {
                if (!handle.IsPlayableOutputOfType<AnimationPlayableOutput>())
                    throw new InvalidCastException("Can't set handle: the playable is not an AnimationPlayableOutput.");
            }

            m_Handle = handle;
        }

        ///<exclude />
        public static AnimationPlayableOutput Null
        {
            get { return new AnimationPlayableOutput(PlayableOutputHandle.Null); }
        }

        ///<exclude />
        public PlayableOutputHandle GetHandle()
        {
            return m_Handle;
        }

        ///<exclude />
        public static implicit operator PlayableOutput(AnimationPlayableOutput output)
        {
            return new PlayableOutput(output.GetHandle());
        }

        ///<exclude />
        public static explicit operator AnimationPlayableOutput(PlayableOutput output)
        {
            return new AnimationPlayableOutput(output.GetHandle());
        }

        ///<summary>Returns the <see cref="Animator" /> that plays the animation graph.</summary>
        ///<returns>The targeted <see cref="Animator" />.</returns>
        public Animator GetTarget()
        {
            return InternalGetTarget(ref m_Handle);
        }

        ///<summary>Sets the <see cref="Animator" /> that plays the animation graph.</summary>
        ///<param name="value">The targeted <see cref="Animator" />.</param>
        public void SetTarget(Animator value)
        {
            InternalSetTarget(ref m_Handle, value);
        }

        [NativeMethod(ThrowsException = true)]
        extern private static Animator InternalGetTarget(ref PlayableOutputHandle handle);

        [NativeMethod(ThrowsException = true)]
        extern private static void InternalSetTarget(ref PlayableOutputHandle handle, Animator target);
    }
}
