using UnityEngine;

namespace MondayOFF {
    public static partial class EveryDay {
        public const string Version = "3.0.26";

        internal static System.Action onEverydayInitialized = default;
        internal static bool isInitialized = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AfterSceneLoad() {
            isInitialized = false;
            var initMessage =
@$"
================== Everyday {Version} ==================
    Log Level: {EverydaySettings.Instance.logLevel}
    Test Mode: {EverydaySettings.Instance.isTestMode}
    Retention: {Retention.Days}
========================================================";
            Debug.Log(initMessage);

            var isGdprApplies = GDPR.IsApplicable();
            PlayerPrefs.SetInt("IABTCF_gdprApplies", isGdprApplies ? 1 : 0);

            EverydayAppTracking.RequestTrackingAuthorization(Initialize);
        }

        private static void Initialize(AttAuthorizationStatus consentStatus) {
            if (isInitialized) {
                return;
            }


            EverydaySettings.AdSettings.Initialize();

            EverydayLogger.Info($"Consent status: {consentStatus}");

            PrepareSettings(consentStatus);

            // Initialize Facebook
            EverydayLogger.Info("Initializing Facebook SDK");
            FacebookInitializer.Initialize();

            // Initialize Singular
            EverydayLogger.Info("Initializing Singular SDK");
            if (SingularSDK.instance != null) {
                GameObject.Destroy(SingularSDK.instance);
                SingularSDK.instance = null;
            }
            var singularGO = new GameObject("SingularSDKObject", typeof(SingularSDK));
            SingularSDK.InitializeSingularSDK(MondayOFF.Keys.EVERYDAY_SINGULAR_API_KEY, MondayOFF.Keys.EVERYDAY_SINGULAR_SECRET_KEY);
#if !UNITY_EDITOR
            SingularSDK.SkanRegisterAppForAdNetworkAttribution();
#endif
            // MaxSDK
            EverydayLogger.Info("Initializing MaxSDK");
            MaxSdkCallbacks.OnSdkInitializedEvent -= OnMaxSdkInitialized;
            MaxSdkCallbacks.OnSdkInitializedEvent += OnMaxSdkInitialized;
            MaxSdk.SetSdkKey(MondayOFF.Keys.EVERYDAY_MAX_KEY);
            MaxSdk.InitializeSdk();
        }

        private static void OnMaxSdkInitialized(MaxSdk.SdkConfiguration sdkConfiguration) {
            // Send Max AdInfo to Singular
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent -= SingularAdDataSender.SendAdData;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent -= SingularAdDataSender.SendAdData;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent -= SingularAdDataSender.SendAdData;

            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += SingularAdDataSender.SendAdData;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += SingularAdDataSender.SendAdData;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += SingularAdDataSender.SendAdData;

            // Initialize Ads Manager
            AdsManager.PrepareManager();
            if (EverydaySettings.AdSettings.initializeOnLoad) {
                AdsManager.Initialize();
            }

            isInitialized = true;
            onEverydayInitialized?.Invoke();
            onEverydayInitialized = null;
        }
    }
}
