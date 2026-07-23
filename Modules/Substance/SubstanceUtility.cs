// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

/*
using scm=System.ComponentModel;
using uei=UnityEngine.Internal;
using RequiredByNativeCodeAttribute=UnityEngine.Scripting.RequiredByNativeCodeAttribute;
using UsedByNativeCodeAttribute=UnityEngine.Scripting.UsedByNativeCodeAttribute;
*/
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections;

namespace UnityEngine
{
    ///<summary>Deprecated feature, no longer available</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [Obsolete("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.", true)]
    public enum ProceduralProcessorUsage
    {
        ///<summary>Deprecated feature, no longer available</summary>
        Unsupported = 0,
        ///<summary>Deprecated feature, no longer available</summary>
        One = 1,
        ///<summary>Deprecated feature, no longer available</summary>
        Half = 2,
        ///<summary>Deprecated feature, no longer available</summary>
        All = 3
    }

    ///<summary>Deprecated feature, no longer available</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [Obsolete("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.", true)]
    public enum ProceduralCacheSize
    {
        ///<summary>Deprecated feature, no longer available</summary>
        Tiny = 0,
        ///<summary>Deprecated feature, no longer available</summary>
        Medium = 1,
        ///<summary>Deprecated feature, no longer available</summary>
        Heavy = 2,
        ///<summary>Deprecated feature, no longer available</summary>
        NoLimit = 3,
        ///<summary>Deprecated feature, no longer available</summary>
        None = 4
    }

    ///<summary>Deprecated feature, no longer available</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [Obsolete("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.", true)]
    public enum ProceduralLoadingBehavior
    {
        ///<summary>Deprecated feature, no longer available</summary>
        DoNothing = 0,
        ///<summary>Deprecated feature, no longer available</summary>
        Generate = 1,
        ///<summary>Deprecated feature, no longer available</summary>
        BakeAndKeep = 2,
        ///<summary>Deprecated feature, no longer available</summary>
        BakeAndDiscard = 3,
        ///<summary>Deprecated feature, no longer available</summary>
        Cache = 4,
        ///<summary>Deprecated feature, no longer available</summary>
        DoNothingAndCache = 5
    }

    ///<summary>Deprecated feature, no longer available</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [Obsolete("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.", true)]
    public enum ProceduralPropertyType
    {
        ///<summary>Deprecated feature, no longer available</summary>
        Boolean = 0,
        ///<summary>Deprecated feature, no longer available</summary>
        Float = 1,
        ///<summary>Deprecated feature, no longer available</summary>
        Vector2 = 2,
        ///<summary>Deprecated feature, no longer available</summary>
        Vector3 = 3,
        ///<summary>Deprecated feature, no longer available</summary>
        Vector4 = 4,
        ///<summary>Deprecated feature, no longer available</summary>
        Color3 = 5,
        ///<summary>Deprecated feature, no longer available</summary>
        Color4 = 6,
        ///<summary>Deprecated feature, no longer available</summary>
        Enum = 7,
        ///<summary>Deprecated feature, no longer available</summary>
        Texture = 8,
        ///<summary>Deprecated feature, no longer available</summary>
        String = 9
    }

    ///<summary>Deprecated feature, no longer available</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [Obsolete("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.", true)]
    public enum ProceduralOutputType
    {
        ///<summary>Deprecated feature, no longer available</summary>
        Unknown = 0,
        ///<summary>Deprecated feature, no longer available</summary>
        Diffuse = 1,
        ///<summary>Deprecated feature, no longer available</summary>
        Normal = 2,
        ///<summary>Deprecated feature, no longer available</summary>
        Height = 3,
        ///<summary>Deprecated feature, no longer available</summary>
        Emissive = 4,
        ///<summary>Deprecated feature, no longer available</summary>
        Specular = 5,
        ///<summary>Deprecated feature, no longer available</summary>
        Opacity = 6,
        ///<summary>Deprecated feature, no longer available</summary>
        Smoothness = 7,
        ///<summary>Deprecated feature, no longer available</summary>
        AmbientOcclusion = 8,
        ///<summary>Deprecated feature, no longer available</summary>
        DetailMask = 9,
        ///<summary>Deprecated feature, no longer available</summary>
        Metallic = 10,
        ///<summary>Deprecated feature, no longer available</summary>
        Roughness = 11
    }

    ///<summary>Deprecated feature, no longer available</summary>
    [StructLayout(LayoutKind.Sequential)]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [Obsolete("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.", true)]
    public sealed partial class ProceduralPropertyDescription
    {
        ///<summary>Deprecated feature, no longer available</summary>
        public string name;
        ///<summary>Deprecated feature, no longer available</summary>
        public string label;
        ///<summary>Deprecated feature, no longer available</summary>
        public string group;
        ///<summary>Deprecated feature, no longer available</summary>
        public ProceduralPropertyType type;
        ///<summary>Deprecated feature, no longer available</summary>
        public bool hasRange;
        ///<summary>Deprecated feature, no longer available</summary>
        public float minimum;
        ///<summary>Deprecated feature, no longer available</summary>
        public float maximum;
        ///<summary>Deprecated feature, no longer available</summary>
        public float step;
        ///<summary>Deprecated feature, no longer available</summary>
        public string[] enumOptions;
        ///<summary>Deprecated feature, no longer available</summary>
        public string[] componentLabels;
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [Obsolete("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.", true)]
    [ExcludeFromPreset]
    public sealed partial class ProceduralMaterial : Material
    {
        internal ProceduralMaterial()
            : base((Material)null)
        {
            FeatureRemoved();
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public ProceduralPropertyDescription[] GetProceduralPropertyDescriptions()
        {
            throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public bool HasProceduralProperty(string inputName)
        {
            throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public bool GetProceduralBoolean(string inputName)
        {
            throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public bool IsProceduralPropertyVisible(string inputName)
        {
            throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public void SetProceduralBoolean(string inputName, bool value)
        {
            FeatureRemoved();
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public float GetProceduralFloat(string inputName)
        {
            throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public void SetProceduralFloat(string inputName, float value)
        {
            FeatureRemoved();
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public Vector4 GetProceduralVector(string inputName)
        {
            throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public void SetProceduralVector(string inputName, Vector4 value)
        {
            FeatureRemoved();
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public Color GetProceduralColor(string inputName)
        {
            throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public void SetProceduralColor(string inputName, Color value)
        {
            FeatureRemoved();
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public int GetProceduralEnum(string inputName)
        {
            throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public void SetProceduralEnum(string inputName, int value)
        {
            FeatureRemoved();
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public Texture2D GetProceduralTexture(string inputName)
        {
            throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public void SetProceduralTexture(string inputName, Texture2D value)
        {
            FeatureRemoved();
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public string GetProceduralString(string inputName)
        {
            throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public void SetProceduralString(string inputName, string value)
        {
            FeatureRemoved();
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public bool IsProceduralPropertyCached(string inputName)
        {
            throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public void CacheProceduralProperty(string inputName, bool value)
        {
            FeatureRemoved();
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public void ClearCache()
        {
            FeatureRemoved();
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public ProceduralCacheSize cacheSize
        {
            get { throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store."); }
            set { FeatureRemoved(); }
        }


        ///<summary>Deprecated feature, no longer available</summary>
        public int animationUpdateRate
        {
            get { throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store."); }
            set { FeatureRemoved(); }
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public void RebuildTextures()
        {
            FeatureRemoved();
        }

        ///<summary>Triggers an immediate (synchronous) rebuild of this ProceduralMaterial's dirty textures.</summary>
        public void RebuildTexturesImmediately()
        {
            FeatureRemoved();
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public bool isProcessing
        {
            get { throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store."); }
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public static void StopRebuilds()
        {
            FeatureRemoved();
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public bool isCachedDataAvailable
        {
            get { throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store."); }
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public bool isLoadTimeGenerated
        {
            get { throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store."); }
            set { FeatureRemoved(); }
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public ProceduralLoadingBehavior loadingBehavior
        {
            get { throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store."); }
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public static bool isSupported
        {
            get { throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store."); }
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public static ProceduralProcessorUsage substanceProcessorUsage
        {
            get { throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store."); }
            set { FeatureRemoved(); }
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public string preset
        {
            get { throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store."); }
            set { FeatureRemoved(); }
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public Texture[] GetGeneratedTextures()
        {
            throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public ProceduralTexture GetGeneratedTexture(string textureName)
        {
            throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public bool isReadable
        {
            get { throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store."); }
            set { FeatureRemoved(); }
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public void FreezeAndReleaseSourceData()
        {
            FeatureRemoved();
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public bool isFrozen
        {
            get { throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store."); }
        }
    }

    ///<summary>Deprecated feature, no longer available</summary>
    [Obsolete("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.", true)]
    [ExcludeFromPreset]
    public sealed partial class ProceduralTexture : Texture
    {
        private ProceduralTexture()
        {
            throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public ProceduralOutputType GetProceduralOutputType()
        {
            throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
        }

        ///<summary>Deprecated feature, no longer available</summary>
        internal ProceduralMaterial GetProceduralMaterial()
        {
            throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public bool hasAlpha
        {
            get { throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store."); }
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public TextureFormat format
        {
            get { throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store."); }
        }

        ///<summary>Deprecated feature, no longer available</summary>
        public Color32[] GetPixels32(int x, int y, int blockWidth, int blockHeight)
        {
            throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
        }
    }

}
