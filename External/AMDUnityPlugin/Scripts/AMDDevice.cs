using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEditor;


namespace UnityEngine.AMD
{
    // -----------------------------------------------------------------------------------
    //  Public Enums must match C++ enums found in AMDDevice.h
    // -----------------------------------------------------------------------------------
    #region GraphicsDeviceEnums
    internal enum PluginEvent
    {
        DestroyFeature = 0,
        FSR2Execute = 1,
        FSR2PostExecute = 2,
        FSR2Init = 3
    }
    #endregion

    // -----------------------------------------------------------------------------------
    //  Main AMD device. Use to interact with AMD specific features on a unity SRP
    // -----------------------------------------------------------------------------------
    ///<summary>Provides the main entry point for the AMD Module. Use this to interact with the FSR2 feature.</summary>
    ///<remarks>The <c>GraphicsDevice</c> includes an interface for creating and managing feature contexts, handling FSR2 command execution, and providing utility methods for quality mode resolution management.
    ///
    ///<c>GraphicsDevice</c> is needed to implement FSR2 outside of the built-in integration to the &lt;a href="https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@17.2/manual/Dynamic-Resolution.html"&gt;HDRP Dynamic Resolution&lt;/a&gt;.
    ///
    ///Before using <c>GraphicsDevice</c>, ensure the <see cref="AMDUnityPlugin" /> is loaded and the device is initialized via <see cref="GraphicsDevice.CreateGraphicsDevice" />.</remarks>
    ///<example nocheck="true">
    ///  <code><![CDATA[
    ///using UnityEngine;
    ///using UnityEngine.Rendering;
    ///using UnityEngine.Rendering.HighDefinition;
    ///using UnityEngine.AMD;
    ///
    /// // Example HDRP custom pass 
    ///public class CustomFSRPass : CustomPass
    ///{
    ///    public static bool EnsureAMDPluginLoaded()
    ///    {
    ///        if (!AMDUnityPlugin.IsLoaded())
    ///        {
    ///            Debug.Log("AMDUnityPlugin is not loaded!");
    ///            if (!AMDUnityPlugin.Load())
    ///            {
    ///                Debug.LogError("Unable to load AMDUnityPlugin");
    ///                return false;
    ///            }
    ///        }
    ///        Debug.Log("AMDUnityPlugin is successfully loaded!");
    ///        return true;
    ///    }
    ///
    ///    void InitializeAMDDevice()
    ///    {
    ///        if (!EnsureAMDPluginLoaded())
    ///            return;
    ///
    ///        // AMDUnityPlugin initialization will handle device creation for us.
    ///        // In case the device is not created, we call the static method GraphicsDevice.CreateGraphicsDevice().
    ///        amdDevice = GraphicsDevice.device == null ? GraphicsDevice.CreateGraphicsDevice() : GraphicsDevice.device;
    ///
    ///        Debug.LogFormat("AMD.GraphicsDevice initialized w/ version {0}", GraphicsDevice.version);
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
    ///            dimension: TextureDimension.Tex2D,
    ///            colorFormat: GraphicsFormat.R16G16B16A16_SFloat,
    ///            name: "fsr2OutputColorBuffer",
    ///            enableRandomWrite: true
    ///        );
    ///
    ///        // other pass setup code
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
    ///            FSR2CommandInitializationData initData = new FSR2CommandInitializationData();
    ///            // populate initData
    ///            fsr2Context = amdDevice.CreateFeature(ctx.cmd, initData);
    ///        }
    ///
    ///        fsr2Context.executeData.enableSharpening = m_EnableSharpening ? 1 : 0;
    ///        // populate rest of fsr2Context.executeData 
    ///        
    ///        FSR2TextureTable fsr2TextureTable = new FSR2TextureTable()
    ///        {
    ///            // populate texture table
    ///        };
    ///
    ///        amdDevice.ExecuteFSR2(ctx.cmd, fsr2Context, fsr2TextureTable);
    ///    }
    ///
    ///    protected override void Cleanup()
    ///    {
    ///        // pass cleanup code
    ///
    ///        // No explicit clean up is necessary for AMD.GraphicsDevice, all handled internally
    ///    }
    ///
    ///    private GraphicsDevice amdDevice = null;
    ///    private FSR2Context fsr2Context = null;
    ///    private RTHandle fsr2OutputColorBuffer;
    ///    // other member variables
    ///}
    ///]]></code>
    ///</example>
    ///<seealso cref="AMDUnityPlugin" />
    ///<seealso cref="FSR2Context" />
    ///<seealso cref="FSR2TextureTable" />
    ///<seealso cref="FSR2CommandInitializationData" />
    ///<seealso cref="FSR2CommandExecutionData" />
    public class GraphicsDevice
    {
        #region Private

        static private GraphicsDevice sGraphicsDeviceInstance = null;
        private Stack<FSR2Context> s_ContextObjectPool = new Stack<FSR2Context>();

        private GraphicsDevice()
        {
        }

        private bool Initialize()
        {
            return AMDUP_InitApi();
        }

        private void Shutdown()
        {
            AMDUP_ShutdownApi();
        }

        ~GraphicsDevice()
        {
            Shutdown();
        }

        private void InsertEventCall(CommandBuffer cmd, PluginEvent pluginEvent, IntPtr ptr)
        {
            cmd.IssuePluginEventAndData(AMDUP_GetRenderEventCallback(), (int)pluginEvent + AMDUP_GetBaseEventId(), ptr);
        }

        private static GraphicsDevice InternalCreate()
        {
            if (sGraphicsDeviceInstance != null)
            {
                sGraphicsDeviceInstance.Shutdown();
                sGraphicsDeviceInstance.Initialize();
                return sGraphicsDeviceInstance;
            }
        
            var newGraphicsDevice = new GraphicsDevice();
            if (newGraphicsDevice.Initialize())
            {
                sGraphicsDeviceInstance = newGraphicsDevice;
                return newGraphicsDevice;
            }

            Debug.LogWarning("Unity has an invalid api for dvice. Init failed[");
            return null;
        }

        private static int CreateSetTextureUserData(int featureId, int textureSlot, bool clearTextureTable)
        {
            int featureIdMask = (featureId & 0xffff); //16 bits
            int textureSlotMask   = (textureSlot & 0x7fff); //15 bits;
            int clearTableMask    = clearTextureTable ? 0x1 : 0x0; //1 bit
            return (featureIdMask << 16) | (textureSlotMask << 1) | clearTableMask;
        }

        private void SetTexture(CommandBuffer cmd, FSR2Context fsr2Context, FSR2CommandExecutionData.Textures textureSlot, Texture texture, bool clearTextureTable = false)
        {
            if (texture == null)
                return;

            uint userData = (uint)CreateSetTextureUserData((int)fsr2Context.featureSlot, (int)textureSlot, clearTextureTable);
            cmd.IssuePluginCustomTextureUpdateV2(
                AMDUP_GetSetTextureEventCallback(), texture, userData);
        }

        #endregion

        // -----------------------------------------------------------------------------------
        // Public API to interact with AMD Features
        // -----------------------------------------------------------------------------------
        #region PublicAPI

        ///<summary>Creates the main API object. Call this method only once in your application.</summary>
        ///<returns>The Device API object to access AMD features. If you call this function again, the function returns the same device.</returns>
        public static GraphicsDevice CreateGraphicsDevice()
        {
            return GraphicsDevice.InternalCreate();
        }

        ///<summary>Gets the device created by GraphicsDevice.CreateGraphicsDevice. If the device hasn't been created this property evaluates to null.</summary>
        public static GraphicsDevice device { get { return sGraphicsDeviceInstance; } }

        ///<summary>Gets the version that corresponds to the Unity host plugin that manages the AMD.AMDUnityPlugin official library.</summary>
        public static uint version  { get { return AMDUP_GetDeviceVersion();} }

        ///<summary>Creates an FSR2Context object.</summary>
        ///<param name="cmd">The rendering command buffer to record commands into. This call does not execute the command buffer. You must execute the command buffer yourself at any time after this call.</param>
        ///<param name="initSettings">Initial settings structure for the FSR2 feature.</param>
        ///<returns>Returns a Fidelity FX Super Resolution 2.0 context object.</returns>
        public FSR2Context CreateFeature(CommandBuffer cmd, in FSR2CommandInitializationData initSettings)
        {
            FSR2Context fsrContext = null;
            if (s_ContextObjectPool.Count == 0)
            {
                fsrContext = new FSR2Context();
            }
            else
            {
                fsrContext = s_ContextObjectPool.Pop();
            }

            fsrContext.Init(initSettings, AMDUP_CreateFeatureSlot());
            InsertEventCall(cmd, PluginEvent.FSR2Init, fsrContext.GetInitCmdPtr());
            return fsrContext;
        }

        ///<summary>Queries the resolution configuration from a specified quality mode preset.</summary>
        ///<param name="qualityMode">The input quality mode. See <see cref="FSR2Quality" /> for the list of quality modes.</param>
        ///<param name="displayWidth">The input display resolution width.</param>
        ///<param name="displayHeight">The input display resolution height.</param>
        ///<param name="renderWidth">The output resolution width calculated.</param>
        ///<param name="renderHeight">The output resolution height calculated.</param>
        ///<returns>Returns true on success. False otherwise.</returns>
        public bool GetRenderResolutionFromQualityMode(FSR2Quality qualityMode, uint displayWidth, uint displayHeight, out uint renderWidth, out uint renderHeight)
        {
            return AMDUP_GetRenderResolutionFromQualityMode(qualityMode, displayWidth, displayHeight, out renderWidth, out renderHeight);
        }

        ///<summary>Gets a precomputed upscaling ratio based on a preset quality setting.</summary>
        ///<param name="qualityMode">The input quality mode. See <see cref="FSR2Quality" /> for the list of quality modes.</param>
        ///<returns>The upscaling per-dimension ratio.</returns>
        public float GetUpscaleRatioFromQualityMode(FSR2Quality qualityMode)
        {
            return AMDUP_GetUpscaleRatioFromQualityMode(qualityMode);
        }

        ///<summary>Destroys a specific FSR2Context created with GraphicsDevice.CreateFeature.</summary>
        ///<param name="cmd">The rendering command buffer to record commands into. This call does not execute the command buffer. You must execute the command buffer yourself at any time after this call.</param>
        ///<param name="fsrContext">The command object to destroy.</param>
        public void DestroyFeature(CommandBuffer cmd, FSR2Context fsrContext)
        {
            InsertEventCall(cmd, PluginEvent.DestroyFeature, new IntPtr(fsrContext.featureSlot));
            fsrContext.Reset();
            s_ContextObjectPool.Push(fsrContext);
        }

        ///<summary>Records the execution of the FSR2 pass into a rendering command buffer. This call does not execute the command buffer, it only appends custom commands into it.</summary>
        ///<param name="cmd">The rendering command buffer to record commands into. This call does not execute the command buffer. You must execute the command buffer yourself at any time after this call.</param>
        ///<param name="fsr2Context">The source feature context to execute. You must set the parameters for this command in the <see cref="FSR2Context" /> object prior to this call.</param>
        ///<param name="textures">The collection of textures represented by <see cref="FSR2TextureTable" />, where inputs/outputs are specified for the FSR 2.0 pass to execute.</param>
        public void ExecuteFSR2(CommandBuffer cmd, FSR2Context fsr2Context, in FSR2TextureTable textures)
        {
            SetTexture(cmd, fsr2Context, FSR2CommandExecutionData.Textures.ColorInput,       textures.colorInput, true);
            SetTexture(cmd, fsr2Context, FSR2CommandExecutionData.Textures.ColorOutput,      textures.colorOutput);
            SetTexture(cmd, fsr2Context, FSR2CommandExecutionData.Textures.Depth,            textures.depth);
            SetTexture(cmd, fsr2Context, FSR2CommandExecutionData.Textures.MotionVectors,    textures.motionVectors);
            SetTexture(cmd, fsr2Context, FSR2CommandExecutionData.Textures.TransparencyMask, textures.transparencyMask);
            SetTexture(cmd, fsr2Context, FSR2CommandExecutionData.Textures.ExposureTexture,  textures.exposureTexture);
            SetTexture(cmd, fsr2Context, FSR2CommandExecutionData.Textures.BiasColorMask,    textures.biasColorMask);
            InsertEventCall(cmd, PluginEvent.FSR2Execute, fsr2Context.GetExecuteCmdPtr());

            // D3D12 requires to pump submission into its own thread.
            // this is caused by the current implementation of the plugin. 
            // this function is probably noop in other graphics APIs
            InsertEventCall(cmd, PluginEvent.FSR2PostExecute, fsr2Context.GetExecuteCmdPtr());
        }

        #endregion

        // -----------------------------------------------------------------------------------
        // All required imports for the plugin
        // -----------------------------------------------------------------------------------

        #region Imports

        [DllImport("AMDUnityPlugin", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private extern static bool AMDUP_InitApi();

        [DllImport("AMDUnityPlugin", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private extern static void AMDUP_ShutdownApi();

        [DllImport("AMDUnityPlugin", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern uint AMDUP_GetDeviceVersion();

        [DllImport("AMDUnityPlugin", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr AMDUP_GetRenderEventCallback();

        [DllImport("AMDUnityPlugin", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr AMDUP_GetSetTextureEventCallback();

        [DllImport("AMDUnityPlugin", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern uint AMDUP_CreateFeatureSlot();

        [DllImport("AMDUnityPlugin", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern bool AMDUP_GetRenderResolutionFromQualityMode(FSR2Quality qualityMode, uint displayWidth, uint displayHeight, out uint renderWidth, out uint renderHeight);

        [DllImport("AMDUnityPlugin", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern float AMDUP_GetUpscaleRatioFromQualityMode(FSR2Quality qualityMode);

        [DllImport("AMDUnityPlugin", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int AMDUP_GetBaseEventId();

        #endregion
    };
} // namespace AMD
