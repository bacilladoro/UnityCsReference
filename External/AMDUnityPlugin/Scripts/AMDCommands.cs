using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEditor;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.AMD
{
    #region CmdData

    //Flags Verbatim from ffx_fsr.h
    ///<summary>Options that represent subfeatures of FSR2.</summary>
    [Flags]
    public enum FfxFsr2InitializationFlags
    {
        ///<summary>Flag indicating if the color provided is using a high-dynamic range.</summary>
        EnableHighDynamicRange                  = (1<<0),   // A bit indicating if the input color data provided is using a high-dynamic range.
        ///<summary>Flag indicating if the motion vectors are rendered at display resolution.</summary>
        EnableDisplayResolutionMotionVectors    = (1<<1),   // A bit indicating if the motion vectors are rendered at display resolution.
        ///<summary>Flag indicating if the motion vectors have a jitter pattern applied to them.</summary>
        EnableMotionVectorsJitterCancellation   = (1<<2),   // A bit indicating that the motion vectors have the jittering pattern applied to them.
        ///<summary>Flag indicating that if the input depth buffer data provided is inverted. (1 is close, 0 is far).</summary>
        DepthInverted                           = (1<<3),   // A bit indicating that the input depth buffer data provided is inverted [1..0].
        ///<summary>Flag indicating if the depth buffer data is using an infinite far plane.</summary>
        EnableDepthInfinite                     = (1<<4),   // A bit indicating that the input depth buffer data provided is using an infinite far plane.
        ///<summary>Flag indicating if automatic exposure should be applied to the input color data.</summary>
        EnableAutoExposure                      = (1<<5),   // A bit indicating if automatic exposure should be applied to input color data.
        ///<summary>Flag indicating if the application uses dynamic resolution scaling.</summary>
        EnableDynamicResolution                 = (1<<6),   // A bit indicating that the application uses dynamic resolution scaling.
        ///<summary>Flag indicating if the backend should use 1D textures.</summary>
        EnableTexture1DUsage                    = (1<<7)    // A bit indicating that the backend should use 1D textures.
    }

    //Quality mode verbatim from AMDCommands.h
    ///<summary>Options for FSR2 performance modes.</summary>
    ///<remarks>Each performance mode corresponds to a scaling ratio per dimension of the input texture:
    ///
    ///1. Quality: 1.5x
    ///2. Balanced: 1.7x
    ///3. Performance: 2.0x
    ///4. Ultra Performance: 3.0x
    ///
    ///Refer to the &lt;a href="https://gpuopen.com/manuals/fidelityfx_sdk/fidelityfx_sdk-page_techniques_super-resolution-temporal/#id8"&gt;Scaling modes section in FSR2 Documentation&lt;/a&gt; for more details.</remarks>
    ///<example nocheck="true">
    ///  <code><![CDATA[
    ///using UnityEngine;
    ///using UnityEngine.Rendering;
    ///using UnityEngine.Rendering.HighDefinition;
    ///using UnityEngine.Experimental.Rendering;
    ///using UnityEngine.AMD;
    ///
    /// // Example HDRP custom pass 
    ///public class CustomFSRPass : CustomPass
    ///{
    ///    void InitializeAMDDevice()
    ///    {
    ///        // device initialization code
    ///    }
    ///    bool HasOutputResolutionChanged(CustomPassContext ctx) 
    ///    { 
    ///        return m_QualityBefore != m_Quality; 
    ///    }
    ///    bool HasInputResolutionChanged(CustomPassContext ctx) 
    ///    { 
    ///        // detect resolution change
    ///    }
    ///
    ///    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
    ///    {
    ///        if (amdDevice == null)
    ///        {
    ///            InitializeAMDDevice();
    ///        }
    ///
    ///        float scalingRatio = fsr2Context == null ? 1.0f : amdDevice.GetUpscaleRatioFromQualityMode(m_Quality);
    ///        fsr2OutputColorBuffer = RTHandles.Alloc(
    ///            new Vector2(scalingRatio, scalingRatio), 
    ///            slices: 1, 
    ///            dimension: TextureDimension.Tex2D,
    ///            colorFormat: GraphicsFormat.R16G16B16A16_SFloat,
    ///            name: "fsr2OutputColorBuffer",
    ///            enableRandomWrite: true
    ///        );
    ///    }
    ///
    ///    protected override void Execute(CustomPassContext ctx)
    ///    {
    ///        bool initializeFsr2Context = fsr2Context == null || HasInputResolutionChanged(ctx) || HasOutputResolutionChanged(ctx);
    ///        if (initializeFsr2Context)
    ///        {
    ///            // fsr2Context initialization code
    ///        }
    ///
    ///        fsr2Context.executeData.enableSharpening = m_EnableSharpening ? 1 : 0;
    ///        // populate rest of the fsr2Context.executeData
    ///
    ///        FSR2TextureTable fsr2TextureTable = new FSR2TextureTable()
    ///        {
    ///            // set texture table
    ///        };
    ///
    ///        amdDevice.ExecuteFSR2(ctx.cmd, fsr2Context, fsr2TextureTable);
    ///    }
    ///
    ///    protected override void Cleanup()
    ///    {
    ///        // cleanup code
    ///    }
    ///
    ///    private GraphicsDevice amdDevice = null;
    ///    private FSR2Context fsr2Context = null;
    ///    private RTHandle fsr2OutputColorBuffer;
    ///
    ///    [SerializeField] public float m_Sharpness = 0.92f;
    ///    [SerializeField] public bool m_EnableSharpening = true;
    ///    [SerializeField] public FSR2Quality m_Quality = FSR2Quality.Quality;
    ///    FSR2Quality m_QualityBefore = FSR2Quality.Quality;
    ///    Vector2Int m_InputTextureSize = new Vector2Int(0,0);
    ///}]]></code>
    ///</example>
    public enum FSR2Quality
    {
        ///<summary>Highest quality, lower performance.</summary>
        Quality = 0,
        ///<summary>Balances performance with quality.</summary>
        Balanced,
        ///<summary>Fast performance, lower quality.</summary>
        Performance,
        ///<summary>Fastest performance, lowest quality.</summary>
        UltraPerformance
    }

    ///<summary>Represents the initialization state of a <see cref="FSR2Context" />. You can only use and set this when calling <see cref="GraphicsDevice.CreateFeature" />.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FSR2CommandInitializationData
    {
        //// These properties must match the code in AMDCommands.h in C++ ////
        ///<summary>The maximum width that rendering will be performed at.</summary>
        public uint maxRenderSizeWidth;
        ///<summary>The maximum height that rendering will be performed at.</summary>
        public uint maxRenderSizeHeight;
        ///<summary>The width of the presentation resolution targeted by the upscaling process.</summary>
        public uint displaySizeWidth;
        ///<summary>The height of the presentation resolution targeted by the upscaling process.</summary>
        public uint displaySizeHeight;
        ///<summary>Initialization flags.</summary>
        ///<seealso cref="FfxFsr2InitializationFlags" />
        public FfxFsr2InitializationFlags ffxFsrFlags;
        ///<exclude />
        internal uint featureSlot;
        ////////////////////////////////////////////////////////////////////

        ///<summary>Helper function. Controls the initialization feature flags set. See Also: <see cref="FfxFsr2InitializationFlags" />.</summary>
        ///<param name="flag">The feature flag to set or unset.</param>
        ///<param name="value">Indicates whether to set or unset the flag.</param>
        public void SetFlag(FfxFsr2InitializationFlags flag, bool value)
        {
            if (value)
            {
                ffxFsrFlags |= flag;
            }
            else
            {
                ffxFsrFlags &= ~flag;
            }
        }

        ///<summary>Helper function. Identifies whether an initialization flag is set or unset. See Also: <see cref="FfxFsr2InitializationFlags" />.</summary>
        ///<param name="flag">The feature flag to get the state from.</param>
        ///<returns>Indicates whether the feature state is set or unset.</returns>
        public bool GetFlag(FfxFsr2InitializationFlags flag)
        {
            return (ffxFsrFlags & flag) != 0;
        }
    }

    ///<summary>The set of texture slots available for the <see cref="FSR2Context" />. SA <see cref="GraphicsDevice.ExecuteFSR2" />.</summary>
    ///<remarks>Use this struct to specify input and output textures for the FSR2 implementation.
    ///
    ///**Note:** You must create output color texture <c>colorOutput</c> with <c>enableRandomWrite</c> parameter set to <c>true</c> when initializing the <see cref="T:UnityEngine.Rendering.RTHandle" /> of the texture. This is due to FSR2 passes requiring access to the resources in a compute shader
    ///
    ///Refer to the &lt;a href="https://gpuopen.com/manuals/fidelityfx_sdk/fidelityfx_sdk-page_techniques_super-resolution-temporal/#id12"&gt;Input resources section in FSR2 Documentation&lt;/a&gt; for more details.</remarks>
    ///<example nocheck="true">
    ///  <code><![CDATA[
    ///using UnityEngine;
    ///using UnityEngine.Rendering;
    ///using UnityEngine.Rendering.HighDefinition;
    ///using UnityEngine.Experimental.Rendering;
    ///using UnityEngine.AMD;
    ///
    /// // Example HDRP custom pass 
    ///public class CustomFSRPass : CustomPass
    ///{
    ///    void InitializeAMDDevice()
    ///    {
    ///        // device initialization code
    ///    }
    ///    bool HasOutputResolutionChanged(CustomPassContext ctx) 
    ///    { 
    ///        // detect resolution change
    ///    }
    ///    bool HasInputResolutionChanged(CustomPassContext ctx) 
    ///    { 
    ///        // detect resolution change
    ///    }
    ///
    ///    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
    ///    {
    ///        if (amdDevice == null)
    ///        {
    ///            InitializeAMDDevice();
    ///        }
    ///
    ///        float scalingRatio = fsr2Context == null ? 1.0f : amdDevice.GetUpscaleRatioFromQualityMode(m_Quality);
    ///        fsr2OutputColorBuffer = RTHandles.Alloc(
    ///            new Vector2(scalingRatio, scalingRatio), 
    ///            slices: 1, 
    ///            dimension: TextureDimension.Tex2D,
    ///            colorFormat: GraphicsFormat.R16G16B16A16_SFloat,
    ///            name: "fsr2OutputColorBuffer",
    ///            enableRandomWrite: true
    ///        );
    ///    }
    ///
    ///    protected override void Execute(CustomPassContext ctx)
    ///    {
    ///        bool initializeFsr2Context = fsr2Context == null || HasInputResolutionChanged(ctx) || HasOutputResolutionChanged(ctx);
    ///        if (initializeFsr2Context)
    ///        {
    ///            // fsr2Context initialization code
    ///        }
    ///
    ///        fsr2Context.executeData.enableSharpening = m_EnableSharpening ? 1 : 0;
    ///        // populate rest of the fsr2Context.executeData
    ///
    ///
    ///        FSR2TextureTable fsr2TextureTable = new FSR2TextureTable()
    ///        {
    ///            // Mandatory inputs
    ///            colorInput = ctx.cameraColorBuffer,
    ///            colorOutput = fsr2OutputColorBuffer,
    ///            depth = ctx.cameraDepthBuffer,
    ///            motionVectors = ctx.cameraMotionVectorsBuffer,
    ///
    ///            // Optional inputs
    ///            transparencyMask = null,
    ///            exposureTexture = null,
    ///            reactiveMask = null,
    ///            biasColorMask = null
    ///        };
    ///
    ///        amdDevice.ExecuteFSR2(ctx.cmd, fsr2Context, fsr2TextureTable);
    ///    }
    ///
    ///    protected override void Cleanup()
    ///    {
    ///        // cleanup code
    ///    }
    ///
    ///    private GraphicsDevice amdDevice = null;
    ///    private FSR2Context fsr2Context = null;
    ///    private RTHandle fsr2OutputColorBuffer;
    ///
    ///    [SerializeField] public float m_Sharpness = 0.92f;
    ///    [SerializeField] public bool m_EnableSharpening = true;
    ///    [SerializeField] public FSR2Quality m_Quality = FSR2Quality.Quality;
    ///    FSR2Quality m_QualityBefore = FSR2Quality.Quality;
    ///    Vector2Int m_InputTextureSize = new Vector2Int(0,0);
    ///}]]></code>
    ///</example>
    ///<seealso cref="AMDUnityPlugin" />
    ///<seealso cref="GraphicsDevice" />
    ///<seealso cref="FSR2Context" />
    ///<seealso cref="FSR2CommandInitializationData" />
    ///<seealso cref="FSR2CommandExecutionData" />
    public struct FSR2TextureTable
    {
        ///<summary>The input color buffer to upsample for <see cref="FSR2Context" />. This texture is mandatory and you must set it to a non-null value.</summary>
        public Texture colorInput       { set; get; }
        ///<summary>The output color buffer. This texture is mandatory and you must set it to a non-null value.</summary>
        public Texture colorOutput      { set; get; }
        ///<summary>The input depth buffer. This must be the same size as the input color buffer. This texture is mandatory and you must set it to a non-null value.</summary>
        public Texture depth            { set; get; }
        ///<summary>The motion vectors requested by the <see cref="FSR2Context" />. Depending on the <see cref="FfxFsr2InitializationFlags" /> specified in <see cref="FSR2Context.initData" />, this buffer can be a smaller scale or the full output resolution. This texture is mandatory and you must set it to a non-null value.</summary>
        public Texture motionVectors    { set; get; }
        ///<summary>A transparency bit mask. This must be the same size as the input texture. This texture helps the <see cref="FSR2Context" /> with ghosting issues. This texture is optional.</summary>
        public Texture transparencyMask { set; get; }
        ///<summary>A 1x1 texture with pre-exposure values. If you do not use pre-exposure, do not set this texture. This texture is optional.</summary>
        public Texture exposureTexture  { set; get; }
        ///<summary>Rendering mask specifying reliance on temporal information. &lt;a href="https://github.com/GPUOpen-Effects/FidelityFX-FSR2/tree/master#reactive-mask"&gt;Github documentation on reactive mask.&lt;/a&gt;</summary>
        public Texture reactiveMask     { set; get; }
        ///<summary>A mask, same size as colorInput, preferably of format R8_UNORM that informs FSR2 of possible moving pixels. If heavy ghosting is encountered, set pixels to this mask to fix the problem. This texture is optional.</summary>
        public Texture biasColorMask    { set; get; }
    }

    ///<summary>Represents the state of an FSR2Context. If you call Device.ExecuteFSR2, Unity sends the values in this struct to the runtime. After this, you can change the values in this struct without any side effects.</summary>
    ///<remarks>
    ///  <see cref="FSR2CommandExecutionData" /> expects <c>frameTimeDelta</c> in milliseconds while Unity's <see cref="Time.deltaTime" /> is seconds, and <c>cameraFovAngleVertical</c> in radians while Unity provides field of view in degrees. 
    ///
    ///FSR2 expects the motion vectors to be in screen space and their values to describe motion from the current frame to the previous frame. 
    ///Unity's motion vectors describe motion from the previous frame to the current frame and are in normalized device coordinates (NDC), in the [-1, +1] range.
    ///To conform to the FSR2 requirements, provide <c>MVScaleX</c> and <c>MVScaleY</c> values that scale the motion vector values with the motion vector render target size.
    ///
    ///Refer to the &lt;a href="https://gpuopen.com/manuals/fidelityfx_sdk/fidelityfx_sdk-page_techniques_super-resolution-temporal/#providing-motion-vectors"&gt;Providing Motion Vectors section in FSR2 Documentation&lt;/a&gt; for more details.</remarks>
    ///<example nocheck="true">
    ///  <code><![CDATA[
    ///using UnityEngine;
    ///using UnityEngine.Rendering;
    ///using UnityEngine.Rendering.HighDefinition;
    ///using UnityEngine.Experimental.Rendering;
    ///using UnityEngine.AMD;
    ///
    /// // Example HDRP custom pass 
    ///public class CustomFSRPass : CustomPass
    ///{
    ///    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
    ///    {
    ///        // setup code
    ///    }
    ///
    ///    protected override void Execute(CustomPassContext ctx)
    ///    {
    ///        bool initializeFsr2Context = fsr2Context == null || HasInputResolutionChanged(ctx) || HasOutputResolutionChanged(ctx);
    ///        if (initializeFsr2Context)
    ///        {
    ///            // initialize fsr2Context
    ///        }
    ///
    ///        Vector2Int mvSize = ctx.cameraMotionVectorsBuffer.GetScaledSize();
    ///        m_InputTextureSize = ctx.cameraColorBuffer.GetScaledSize();
    ///
    ///        fsr2Context.executeData.enableSharpening = m_EnableSharpening ? 1 : 0;
    ///        fsr2Context.executeData.sharpness = m_Sharpness;
    ///        fsr2Context.executeData.renderSizeWidth = (uint)m_InputTextureSize.x;
    ///        fsr2Context.executeData.renderSizeHeight = (uint)m_InputTextureSize.y;
    ///        fsr2Context.executeData.jitterOffsetX = -ctx.hdCamera.taaJitter.x;
    ///        fsr2Context.executeData.jitterOffsetY = -ctx.hdCamera.taaJitter.y;
    ///        fsr2Context.executeData.preExposure = 1.0f;
    ///        fsr2Context.executeData.frameTimeDelta = Time.deltaTime * 1000.0f; // FSR2 expects time in milliseconds
    ///        fsr2Context.executeData.cameraNear = ctx.hdCamera.camera.nearClipPlane;
    ///        fsr2Context.executeData.cameraFar = ctx.hdCamera.camera.farClipPlane;
    ///        fsr2Context.executeData.cameraFovAngleVertical = ctx.hdCamera.camera.fieldOfView * (Mathf.PI * 2.0f/360.0f); // FSR2 expects in radians
    ///
    ///        // Unity computes motion vectors in NDC, FSR2 expects them in screen space and from current frame to previous frame.
    ///        // Here we scale by the render target size to meet the FSR2 requirements,
    ///        // and also invert them to satisfy the frame of reference requirement.
    ///        fsr2Context.executeData.MVScaleX = -((float)mvSize.x); 
    ///        fsr2Context.executeData.MVScaleY = -((float)mvSize.y); 
    ///
    ///#if UNITY_EDITOR
    ///        // The same camera is used to render both Scene and Play mode views within the editor.
    ///        // In case both of these views are visible at the same time, we'll need to reset to avoid
    ///        // rendering artifacts.
    ///        fsr2Context.executeData.reset = 1;
    ///#else
    ///        fsr2Context.executeData.reset = (initializeFsr2Context || ctx.hdCamera.isFirstFrame) ? 1 : 0;
    ///#endif
    ///
    ///        FSR2TextureTable fsr2TextureTable = new FSR2TextureTable()
    ///        {
    ///            // initialize texture table
    ///        };
    ///
    ///        amdDevice.ExecuteFSR2(ctx.cmd, fsr2Context, fsr2TextureTable);
    ///    }
    ///
    ///    protected override void Cleanup()
    ///    {
    ///        // cleanup code
    ///    }
    ///
    ///    private GraphicsDevice amdDevice = null;
    ///    private FSR2Context fsr2Context = null;
    ///    private CommandBuffer cmd = null;
    ///    private RTHandle fsr2OutputColorBuffer;
    ///
    ///    [SerializeField] public float m_Sharpness = 0.92f;
    ///    [SerializeField] public bool m_EnableSharpening = true;
    ///    [SerializeField] public FSR2Quality m_Quality = FSR2Quality.Quality;
    ///
    ///    Vector2Int m_InputTextureSize = new Vector2Int(0,0);
    ///}]]></code>
    ///</example>
    ///<seealso cref="AMDUnityPlugin" />
    ///<seealso cref="GraphicsDevice" />
    ///<seealso cref="FSR2Context" />
    ///<seealso cref="FSR2TextureTable" />
    ///<seealso cref="FSR2CommandInitializationData" />
    [StructLayout(LayoutKind.Sequential)]
    public struct FSR2CommandExecutionData
    {
        //// These properties must match the code in AMDCommands.h in C++ ////
        internal enum Textures
        {
            ColorInput = 0,
            ColorOutput,
            Depth,
            MotionVectors,
            TransparencyMask,
            ExposureTexture,
            ReactiveMask,
            BiasColorMask,
        };

        ///<summary>The subpixel jitter offset applied to the camera (X axis).</summary>
        public float    jitterOffsetX;
        ///<summary>The subpixel jitter offset applied to the camera (Y axis).</summary>
        public float    jitterOffsetY;
        ///<summary>The scale factor to apply to motion vectors (X axis).</summary>
        public float    MVScaleX;
        ///<summary>The scale factor to apply to motion vectors (Y axis).</summary>
        public float    MVScaleY;
        ///<summary>The width resolution that was used for rendering the input resources.</summary>
        public uint     renderSizeWidth;
        ///<summary>The height resolution that was used for rendering the input resources.</summary>
        public uint     renderSizeHeight;
        ///<summary>Enable an additional sharpening pass.</summary>
        public int      enableSharpening;
        ///<summary>The sharpness value between 0 and 1, where 0 is no additional sharpness and 1 is maximum additional sharpness.</summary>
        public float    sharpness;
        ///<summary>The time elapsed since the last frame (expressed in milliseconds).</summary>
        public float    frameTimeDelta;
        ///<summary>The pre exposure value (must be &gt; 0.0f).</summary>
        public float    preExposure;
        ///<summary>A boolean value which when set to true, indicates the camera has moved discontinuously.</summary>
        public int      reset;
        ///<summary>The distance to the near plane of the camera.</summary>
        public float    cameraNear;
        ///<summary>The distance to the far plane of the camera.</summary>
        public float    cameraFar;
        ///<summary>The camera angle field of view in the vertical direction (expressed in radians).</summary>
        public float    cameraFovAngleVertical;
        ///<exclude />
        internal uint  featureSlot;
        ////////////////////////////////////////////////////////////////////
    }

    #endregion

    #region SerializationHelpers

    internal class NativeData<T>
        : IDisposable
        where T : struct
    {
        private IntPtr m_MarshalledValue = IntPtr.Zero;
        public T Value = new T();
        public IntPtr Ptr
        {
            get
            {
                unsafe { UnsafeUtility.CopyStructureToPtr(ref Value, m_MarshalledValue.ToPointer()); }
                return m_MarshalledValue;
            }
        }

        public NativeData()
        {
            m_MarshalledValue = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (m_MarshalledValue != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(m_MarshalledValue);
                m_MarshalledValue = IntPtr.Zero;
            }
        }

#pragma warning disable UA5000 // The Avoid Finalizer Analyzer produces compile errors for any new finalizers. This pre-existing finalizer declaration has been suppressed, but should be rewritten if possible.
        ~NativeData() { Dispose(false); }
#pragma warning restore UA5000
    }

    #endregion

    #region DeviceCommands

    ///<summary>Provides the persistent context for managing FSR2 initialization and per-frame execution state.</summary>
    ///<remarks>This class encapsulates both immutable and mutable configuration data required to use AMD FidelityFX Super Resolution 2 (FSR2). 
    ///It must persist across frames to maintain internal history and reconstruction buffers. 
    ///
    ///Use <c>FSR2Context</c> to implement a custom version of FSR2 outside of the built-in integration with the &lt;a href="https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@17.2/manual/Dynamic-Resolution.html"&gt;HDRP Dynamic Resolution&lt;/a&gt;.
    ///
    ///Modify the <see cref="FSR2Context.executeData" /> property to adjust per-frame parameters before calling <see cref="GraphicsDevice.ExecuteFSR2" />. 
    ///
    ///You can initialize <c>FSR2Context</c> using <see cref="GraphicsDevice.CreateFeature" /> and clean it up using <see cref="GraphicsDevice.DestroyFeature" />.</remarks>
    ///<example nocheck="true">
    ///  <code><![CDATA[
    ///using UnityEngine;
    ///using UnityEngine.Rendering;
    ///using UnityEngine.Rendering.HighDefinition;
    ///using UnityEngine.Experimental.Rendering;
    ///using UnityEngine.AMD;
    ///
    /// // Example HDRP custom pass 
    ///public class CustomFSRPass : CustomPass
    ///{
    ///    void InitializeAMDDevice()
    ///    {
    ///        // device initialization code
    ///    }
    ///    bool HasOutputResolutionChanged(CustomPassContext ctx) 
    ///    { 
    ///        // detect resolution change
    ///    }
    ///    bool HasInputResolutionChanged(CustomPassContext ctx) 
    ///    { 
    ///        // detect resolution change
    ///    }
    ///
    ///    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
    ///    {
    ///        // setup code
    ///    }
    ///
    ///    protected override void Execute(CustomPassContext ctx)
    ///    {
    ///        bool initializeFsr2Context = fsr2Context == null || HasInputResolutionChanged(ctx) || HasOutputResolutionChanged(ctx);
    ///        if (initializeFsr2Context)
    ///        {
    ///            if (fsr2Context != null)
    ///            {
    ///                amdDevice.DestroyFeature(ctx.cmd, fsr2Context);
    ///                fsr2Context = null;
    ///            }
    ///
    ///            Vector2Int renderSize = ctx.cameraColorBuffer.GetScaledSize();
    ///            float scalingRatio = amdDevice.GetUpscaleRatioFromQualityMode(m_Quality);
    ///            uint displaySizeWidth = (uint)(renderSize.x * scalingRatio);
    ///            uint displaySizeHeight = (uint)(renderSize.y * scalingRatio);
    ///
    ///            FSR2CommandInitializationData initData = new FSR2CommandInitializationData();
    ///            initData.SetFlag(FfxFsr2InitializationFlags.EnableHighDynamicRange, true);
    ///            initData.SetFlag(FfxFsr2InitializationFlags.EnableDisplayResolutionMotionVectors, true);
    ///            initData.SetFlag(FfxFsr2InitializationFlags.EnableMotionVectorsJitterCancellation, false);
    ///            initData.SetFlag(FfxFsr2InitializationFlags.DepthInverted, true);
    ///            initData.maxRenderSizeWidth = displaySizeWidth;
    ///            initData.maxRenderSizeHeight = displaySizeHeight;
    ///            initData.displaySizeWidth = displaySizeWidth;
    ///            initData.displaySizeHeight = displaySizeHeight;
    ///            fsr2Context = amdDevice.CreateFeature(ctx.cmd, initData);
    ///        }
    ///
    ///        Vector2Int mvSize = ctx.cameraMotionVectorsBuffer.GetScaledSize();
    ///        m_InputTextureSize = ctx.cameraColorBuffer.GetScaledSize();
    ///
    ///        fsr2Context.executeData.enableSharpening = m_EnableSharpening ? 1 : 0;
    ///        fsr2Context.executeData.sharpness = m_Sharpness;
    ///        fsr2Context.executeData.renderSizeWidth = (uint)m_InputTextureSize.x;
    ///        fsr2Context.executeData.renderSizeHeight = (uint)m_InputTextureSize.y;
    ///        fsr2Context.executeData.jitterOffsetX = -ctx.hdCamera.taaJitter.x;
    ///        fsr2Context.executeData.jitterOffsetY = -ctx.hdCamera.taaJitter.y;
    ///        fsr2Context.executeData.preExposure = 1.0f;
    ///        fsr2Context.executeData.frameTimeDelta = Time.deltaTime * 1000.0f; // FSR2 expects time in milliseconds
    ///        fsr2Context.executeData.cameraNear = ctx.hdCamera.camera.nearClipPlane;
    ///        fsr2Context.executeData.cameraFar = ctx.hdCamera.camera.farClipPlane;
    ///        fsr2Context.executeData.cameraFovAngleVertical = ctx.hdCamera.camera.fieldOfView * (Mathf.PI * 2.0f/360.0f); // FSR2 expects in radians
    ///
    ///        // Unity computes motion vectors in NDC, FSR2 expects them in screen space and from current frame to previous frame.
    ///        // Here we scale by the render target size to meet the FSR2 requirements,
    ///        // and also invert them to satisfy the frame of reference requirement.
    ///        fsr2Context.executeData.MVScaleX = -((float)mvSize.x); 
    ///        fsr2Context.executeData.MVScaleY = -((float)mvSize.y); 
    ///
    ///#if UNITY_EDITOR
    ///        // The same camera is used to render both Scene and Play mode views within the editor.
    ///        // In case both of these views are visible at the same time, we'll need to reset to avoid
    ///        // rendering artifacts.
    ///        fsr2Context.executeData.reset = 1;
    ///#else
    ///        fsr2Context.executeData.reset = (initializeFsr2Context || ctx.hdCamera.isFirstFrame) ? 1 : 0;
    ///#endif
    ///
    ///        FSR2TextureTable fsr2TextureTable = new FSR2TextureTable()
    ///        {
    ///            // Mandatory inputs
    ///            colorInput = ctx.cameraColorBuffer,
    ///            colorOutput = fsr2OutputColorBuffer,
    ///            depth = ctx.cameraDepthBuffer,
    ///            motionVectors = ctx.cameraMotionVectorsBuffer,
    ///
    ///            // Optional inputs
    ///            transparencyMask = null,
    ///            exposureTexture = null,
    ///            reactiveMask = null,
    ///            biasColorMask = null
    ///        };
    ///
    ///        amdDevice.ExecuteFSR2(ctx.cmd, fsr2Context, fsr2TextureTable);
    ///    }
    ///
    ///    protected override void Cleanup()
    ///    {
    ///        if (fsr2Context != null)
    ///        {
    ///            amdDevice.DestroyFeature(cmd, fsr2Context);
    ///            fsr2Context = null;
    ///        }
    ///
    ///        // other cleanup code
    ///    }
    ///
    ///    private GraphicsDevice amdDevice = null;
    ///    private FSR2Context fsr2Context = null;
    ///    private CommandBuffer cmd = null;
    ///    private RTHandle fsr2OutputColorBuffer;
    ///
    ///    [SerializeField] public float m_Sharpness = 0.92f;
    ///    [SerializeField] public bool m_EnableSharpening = true;
    ///    [SerializeField] public FSR2Quality m_Quality = FSR2Quality.Quality;
    ///
    ///    Vector2Int m_InputTextureSize = new Vector2Int(0,0);
    ///}]]></code>
    ///</example>
    ///<seealso cref="AMDUnityPlugin" />
    ///<seealso cref="GraphicsDevice" />
    public class FSR2Context
    {
        private NativeData<FSR2CommandInitializationData> m_InitData = new NativeData<FSR2CommandInitializationData>();
        private NativeData<FSR2CommandExecutionData> m_ExecData = new NativeData<FSR2CommandExecutionData>();

        ///<summary>The immutable initialization parameters used to configure FSR2.</summary>
        ///<remarks>Set once when creating the <see cref="FSR2Context" /> with <see cref="GraphicsDevice.CreateFeature" />. These values determine the internal resolution limits, display size, and operational flags.</remarks>
        ///<example nocheck="true">
        ///  <code><![CDATA[
        ///using UnityEngine;
        ///using UnityEngine.Rendering;
        ///using UnityEngine.Rendering.HighDefinition;
        ///using UnityEngine.Experimental.Rendering;
        ///using UnityEngine.AMD;
        ///
        /// // Example HDRP custom pass 
        ///public class CustomFSRPass : CustomPass
        ///{
        ///    void InitializeAMDDevice()
        ///    {
        ///        // device initialization code
        ///    }
        ///    bool HasOutputResolutionChanged(CustomPassContext ctx) 
        ///    { 
        ///        // detect resolution change
        ///    }
        ///    bool HasInputResolutionChanged(CustomPassContext ctx) 
        ///    { 
        ///        // detect resolution change
        ///    }
        ///
        ///    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        ///    {
        ///        // setup code
        ///    }
        ///
        ///    protected override void Execute(CustomPassContext ctx)
        ///    {
        ///        bool initializeFsr2Context = fsr2Context == null || HasInputResolutionChanged(ctx) || HasOutputResolutionChanged(ctx);
        ///        if (initializeFsr2Context)
        ///        {
        ///            if (fsr2Context != null)
        ///            {
        ///                amdDevice.DestroyFeature(ctx.cmd, fsr2Context);
        ///                fsr2Context = null;
        ///            }
        ///
        ///            Vector2Int renderSize = ctx.cameraColorBuffer.GetScaledSize();
        ///            float scalingRatio = amdDevice.GetUpscaleRatioFromQualityMode(m_Quality);
        ///            uint displaySizeWidth = (uint)(renderSize.x * scalingRatio);
        ///            uint displaySizeHeight = (uint)(renderSize.y * scalingRatio);
        ///
        ///            FSR2CommandInitializationData initData = new FSR2CommandInitializationData();
        ///            initData.SetFlag(FfxFsr2InitializationFlags.EnableHighDynamicRange, true);
        ///            initData.SetFlag(FfxFsr2InitializationFlags.EnableDisplayResolutionMotionVectors, true);
        ///            initData.SetFlag(FfxFsr2InitializationFlags.EnableMotionVectorsJitterCancellation, false);
        ///            initData.SetFlag(FfxFsr2InitializationFlags.DepthInverted, true);
        ///            initData.maxRenderSizeWidth = displaySizeWidth;
        ///            initData.maxRenderSizeHeight = displaySizeHeight;
        ///            initData.displaySizeWidth = displaySizeWidth;
        ///            initData.displaySizeHeight = displaySizeHeight;
        ///            fsr2Context = amdDevice.CreateFeature(ctx.cmd, initData);
        ///
        ///            // At this point, we can access the readonly initData within the fsr2Context.
        ///            Debug.LogFormat("FSR2 Context:\n" +
        ///                    "\tmaxRenderSizeWidth={0}\n\tmaxRenderSizeHeight={1}\n" +
        ///                    "\tdisplaySizeWidth={2}\n\tdisplaySizeHeight={3}\n" + 
        ///                    "\tflags={4}",
        ///                fsr2Context.initData.maxRenderSizeWidth,
        ///                fsr2Context.initData.maxRenderSizeHeight,
        ///                fsr2Context.initData.displaySizeWidth,
        ///                fsr2Context.initData.displaySizeHeight,
        ///                fsr2Context.initData.ffxFsrFlags
        ///            );
        ///        }
        ///
        ///        // pass execution code
        ///    }
        ///
        ///    protected override void Cleanup()
        ///    {
        ///        // cleanup code
        ///    }
        ///
        ///    private GraphicsDevice amdDevice = null;
        ///    private FSR2Context fsr2Context = null;
        ///    [SerializeField] public FSR2Quality m_Quality = FSR2Quality.Quality;
        ///    // other fields
        ///}]]></code>
        ///</example>
        ///<seealso cref="AMD.FSR2CommandInitializationData" />
        public ref readonly FSR2CommandInitializationData initData   { get { return ref m_InitData.Value; } }
        ///<summary>The mutable execution parameters used by FSR2 for each frame.</summary>
        ///<remarks>Modify this data before invoking <see cref="GraphicsDevice.ExecuteFSR2" /> to control frame-specific behavior such as jitter, motion vector scale, sharpness, and camera properties.</remarks>
        ///<example nocheck="true">
        ///  <code><![CDATA[
        ///using UnityEngine;
        ///using UnityEngine.Rendering;
        ///using UnityEngine.Rendering.HighDefinition;
        ///using UnityEngine.Experimental.Rendering;
        ///using UnityEngine.AMD;
        ///
        /// // Example HDRP custom pass 
        ///public class CustomFSRPass : CustomPass
        ///{
        ///    void InitializeAMDDevice()
        ///    {
        ///        // device initialization code
        ///    }
        ///    bool HasOutputResolutionChanged(CustomPassContext ctx) 
        ///    { 
        ///        // detect resolution change
        ///    }
        ///    bool HasInputResolutionChanged(CustomPassContext ctx) 
        ///    { 
        ///        // detect resolution change
        ///    }
        ///
        ///    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        ///    {
        ///        // setup code
        ///    }
        ///
        ///    protected override void Execute(CustomPassContext ctx)
        ///    {
        ///        bool initializeFsr2Context = fsr2Context == null || HasInputResolutionChanged(ctx) || HasOutputResolutionChanged(ctx);
        ///        if (initializeFsr2Context)
        ///        {
        ///            // fsr2Context initialization code
        ///        }
        ///
        ///        Vector2Int mvSize = ctx.cameraMotionVectorsBuffer.GetScaledSize();
        ///        m_InputTextureSize = ctx.cameraColorBuffer.GetScaledSize();
        ///
        ///        fsr2Context.executeData.enableSharpening = m_EnableSharpening ? 1 : 0;
        ///        fsr2Context.executeData.sharpness = m_Sharpness;
        ///        fsr2Context.executeData.renderSizeWidth = (uint)m_InputTextureSize.x;
        ///        fsr2Context.executeData.renderSizeHeight = (uint)m_InputTextureSize.y;
        ///        fsr2Context.executeData.jitterOffsetX = -ctx.hdCamera.taaJitter.x;
        ///        fsr2Context.executeData.jitterOffsetY = -ctx.hdCamera.taaJitter.y;
        ///        fsr2Context.executeData.preExposure = 1.0f;
        ///        fsr2Context.executeData.frameTimeDelta = Time.deltaTime * 1000.0f; // FSR2 expects time in milliseconds
        ///        fsr2Context.executeData.cameraNear = ctx.hdCamera.camera.nearClipPlane;
        ///        fsr2Context.executeData.cameraFar = ctx.hdCamera.camera.farClipPlane;
        ///        fsr2Context.executeData.cameraFovAngleVertical = ctx.hdCamera.camera.fieldOfView * (Mathf.PI * 2.0f/360.0f); // FSR2 expects in radians
        ///
        ///        // Unity computes motion vectors in NDC, FSR2 expects them in screen space and from current frame to previous frame.
        ///        // Here we scale by the render target size to meet the FSR2 requirements,
        ///        // and also invert them to satisfy the frame of reference requirement.
        ///        fsr2Context.executeData.MVScaleX = -((float)mvSize.x); 
        ///        fsr2Context.executeData.MVScaleY = -((float)mvSize.y); 
        ///
        ///#if UNITY_EDITOR
        ///        // The same camera is used to render both Scene and Play mode views within the editor.
        ///        // In case both of these views are visible at the same time, we'll need to reset to avoid
        ///        // rendering artifacts.
        ///        fsr2Context.executeData.reset = 1;
        ///#else
        ///        fsr2Context.executeData.reset = (initializeFsr2Context || ctx.hdCamera.isFirstFrame) ? 1 : 0;
        ///#endif
        ///
        ///
        ///        FSR2TextureTable fsr2TextureTable = new FSR2TextureTable()
        ///        {
        ///             // setup texture table for FSR2 inputs/outputs
        ///        };
        ///
        ///        amdDevice.ExecuteFSR2(ctx.cmd, fsr2Context, fsr2TextureTable);
        ///    }
        ///
        ///    protected override void Cleanup()
        ///    {
        ///        // cleanup code
        ///    }
        ///
        ///    private GraphicsDevice amdDevice = null;
        ///    private FSR2Context fsr2Context = null;
        ///
        ///    [SerializeField] public float m_Sharpness = 0.92f;
        ///    [SerializeField] public bool m_EnableSharpening = true;
        ///    
        ///    Vector2Int m_InputTextureSize = new Vector2Int(0,0);
        ///}]]></code>
        ///</example>
        ///<seealso cref="FSR2CommandExecutionData" />
        public ref FSR2CommandExecutionData executeData { get { return ref m_ExecData.Value; } }
        internal uint                   featureSlot { get { return initData.featureSlot; } }

        internal FSR2Context()
        {
        }

        internal void Init(FSR2CommandInitializationData initSettings, uint featureSlot)
        {
            m_InitData.Value = initSettings;
            m_InitData.Value.featureSlot = featureSlot;
        }

        internal void Reset()
        {
            m_InitData.Value = new FSR2CommandInitializationData();
            m_ExecData.Value = new FSR2CommandExecutionData();
        }

        internal IntPtr GetInitCmdPtr()
        {
            return m_InitData.Ptr;
        }

        internal IntPtr GetExecuteCmdPtr()
        {
            m_ExecData.Value.featureSlot = featureSlot;
            return m_ExecData.Ptr;
        }
    }

    #endregion
} // namespace AMD
