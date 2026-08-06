// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace UnityEngine.Experimental.Animations
{
    ///<summary>Describes how an <see cref="AnimationStream" /> is initialized</summary>
    ///<remarks>On every frame, the values in the <see cref="AnimationStream" /> must be reinitialized. <see cref="AnimationStreamSource" /> describes which values should be used: the default values as stored in the <see cref="Animator" />, or the result of previous inputs.</remarks>
    public enum AnimationStreamSource
    {
        ///<summary>
        ///  <see cref="AnimationStream" /> will be initialized with the default values from the <see cref="Animator" />.</summary>
        ///<remarks>Before it is modified during <see cref="IAnimationJob.ProcessAnimation" /> or <see cref="IAnimationJob.ProcessRootMotion" />, the <see cref="AnimationStream" /> contains the default values of the associated <see cref="Animator" />.
        ///
        ///This is the default behaviour for an <see cref="AnimationPlayableOutput" />.</remarks>
        ///<seealso cref="AnimationPlayableOutputExtensions.SetAnimationStreamSource" />
        DefaultValues,
        ///<summary>
        ///  <see cref="AnimationStream" /> will be initialized with the values from the previous <see cref="AnimationPlayableOutput" /> connected to the same <see cref="Animator" />.</summary>
        ///<remarks>Before it is modified during <see cref="IAnimationJob.ProcessAnimation" /> or <see cref="IAnimationJob.ProcessRootMotion" />, the <see cref="AnimationStream" /> contains the values written by any previous inputs.</remarks>
        ///<seealso cref="AnimationPlayableOutputExtensions.SetAnimationStreamSource" />
        PreviousInputs
    }

    ///<summary>Static class providing experimental extension methods for <see cref="AnimationPlayableOutput" /> .</summary>
    ///<remarks>The extension methods in this class can directly be used on an <see cref="AnimationPlayableOutput" />.</remarks>
    ///<seealso cref="AnimationPlayableOutput" />
    [NativeHeader("Modules/Animation/ScriptBindings/AnimationPlayableOutputExtensions.bindings.h")]
    [NativeHeader("Modules/Animation/AnimatorDefines.h")]
    [StaticAccessor("AnimationPlayableOutputExtensionsBindings", StaticAccessorType.DoubleColon)]
    public static class AnimationPlayableOutputExtensions
    {
        ///<summary>Gets the stream source of the specified <see cref="AnimationPlayableOutput" />.</summary>
        ///<param name="output">The <see cref="AnimationPlayableOutput" /> instance that calls this method.</param>
        ///<returns>Returns the <see cref="AnimationStreamSource" /> of the output.</returns>
        ///<seealso cref="AnimationStreamSource" />
        public static AnimationStreamSource GetAnimationStreamSource(this AnimationPlayableOutput output)
        {
            return InternalGetAnimationStreamSource(output.GetHandle());
        }

        ///<summary>Sets the stream source for the specified <see cref="AnimationPlayableOutput" />.</summary>
        ///<remarks>When setting the <see cref="AnimationStreamSource" /> of the output to <see cref="AnimationStreamSource.DefaultValues" />, the <see cref="AnimationStream" /> of this output initalizes every frame with the default values of the <see cref="Animator" />.
        ///
        ///When setting the <see cref="AnimationStreamSource" /> of the output to <see cref="AnimationStreamSource.PreviousInputs" />, the <see cref="AnimationStream" /> of this output initalizes every frame with the result of any previously evaluated outputs on the same <see cref="Animator" />.
        ///
        ///If you use the graph connected to an <see cref="AnimationPlayableOutput" /> to post-process the result of other Animation graphs connected to the same <see cref="Animator" />, you should use <see cref="AnimationStreamSource.PreviousInputs" />. For example, if you use the <see cref="AnimationStream" /> to build an Inverse Kinematics constraint to post-process the built-in <see cref="T:UnityEditor.Animations.AnimatorController" />, your <see cref="AnimationPlayableOutput" /> should be set to <see cref="AnimationStreamSource.PreviousInputs" />.
        ///
        ///In order to start the <see cref="AnimationStream" /> from a blank slate, you should use <see cref="AnimationStreamSource.DefaultValues" />.
        ///For example, to build a custom animation source starting from the default pose, the <see cref="AnimationPlayableOutput" /> should be set to <see cref="AnimationStreamSource.DefaultValues" />.</remarks>
        ///<param name="output">The <see cref="AnimationPlayableOutput" /> instance that calls this method.</param>
        ///<param name="streamSource">The <see cref="AnimationStreamSource" /> to apply on this output.</param>
        ///<seealso cref="AnimationStreamSource" />
        public static void SetAnimationStreamSource(this AnimationPlayableOutput output, AnimationStreamSource streamSource)
        {
            InternalSetAnimationStreamSource(output.GetHandle(), streamSource);
        }

        ///<summary>Gets the priority index of the specified <see cref="AnimationPlayableOutput" />.</summary>
        ///<remarks>Default sorting order is set to 100.</remarks>
        ///<param name="output">The <see cref="AnimationPlayableOutput" /> instance that calls this method.</param>
        ///<returns>Returns the sorting order of the output.</returns>
        public static ushort GetSortingOrder(this AnimationPlayableOutput output)
        {
            return (ushort)InternalGetSortingOrder(output.GetHandle());
        }

        ///<summary>Sets the sorting order for the specified <see cref="AnimationPlayableOutput" />.</summary>
        ///<param name="output">The <see cref="AnimationPlayableOutput" /> instance that calls this method.</param>
        ///<param name="sortingOrder">The sorting order to apply to this output.</param>
        public static void SetSortingOrder(this AnimationPlayableOutput output, ushort sortingOrder)
        {
            InternalSetSortingOrder(output.GetHandle(), (int)sortingOrder);
        }

        [NativeMethod(ThrowsException = true)]
        extern private static AnimationStreamSource InternalGetAnimationStreamSource(PlayableOutputHandle output);
        [NativeMethod(ThrowsException = true)]
        extern private static void InternalSetAnimationStreamSource(PlayableOutputHandle output, AnimationStreamSource streamSource);

        [NativeMethod(ThrowsException = true)]
        extern private static int InternalGetSortingOrder(PlayableOutputHandle output);
        [NativeMethod(ThrowsException = true)]
        extern private static void InternalSetSortingOrder(PlayableOutputHandle output, int sortingOrder);
    }
}
