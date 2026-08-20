// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

#pragma warning disable UAL0010,UAL0011,UAL0012,UAL0013,UAL0014 // AutoStaticsCleanup: UnityConnectHub not yet converted
namespace UnityEditor.Connect
{
    /// <summary>
    /// Class to contain config stuff for the IAP service, URLs and such.
    /// </summary>
    internal class PurchasingConfiguration
    {
        static readonly PurchasingConfiguration k_Instance;

        readonly string m_PurchasingPackageUrl;
        readonly string m_AnalyticsApiUrl;
        readonly string m_GooglePlayDevConsoleUrl;

        static PurchasingConfiguration()
        {
            k_Instance = new PurchasingConfiguration();
        }

        PurchasingConfiguration()
        {
            m_PurchasingPackageUrl = "https://public-cdn.cloud.unity3d.com/UnityEngine.Cloud.Purchasing.unitypackage";
            m_AnalyticsApiUrl = "https://analytics.cloud.unity3d.com";
            m_GooglePlayDevConsoleUrl = "https://play.google.com/apps/publish/";
        }

        public static PurchasingConfiguration instance => k_Instance;

        public string purchasingPackageUrl
        {
            get { return m_PurchasingPackageUrl; }
        }

        public string analyticsApiUrl
        {
            get { return m_AnalyticsApiUrl; }
        }

        public string googlePlayDevConsoleUrl
        {
            get { return m_GooglePlayDevConsoleUrl; }
        }
    }
}
#pragma warning restore UAL0010,UAL0011,UAL0012,UAL0013,UAL0014
