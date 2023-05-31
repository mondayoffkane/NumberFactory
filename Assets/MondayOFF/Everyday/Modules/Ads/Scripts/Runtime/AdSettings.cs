using System.Collections.Generic;
using UnityEngine;

namespace MondayOFF {
    [System.Serializable]
    internal class AdSettings {
        const int BACKUP_RETENTION_THRESHOLD = 2;
        [SerializeField] internal bool initializeOnLoad = true;
        [SerializeField] internal bool showBannerOnLoad = true;
        [SerializeField] internal AdInitializationOrder adInitializationOrder = AdInitializationOrder.Banner_Inter_Reward;
        [SerializeField] internal float adInitializationDelay = 2f;
        [SerializeField] internal float interstitialInterval = 30f;
        [SerializeField] internal bool resetTimerOnRewarded = true;
        [SerializeField] internal MaxSdkBase.BannerPosition bannerPosition = MaxSdkBase.BannerPosition.BottomCenter;

        [SerializeField] internal string AOS_IS_AdUnitID = "";
        [SerializeField] internal string AOS_RV_AdUnitID = "";
        [SerializeField] internal string AOS_BN_AdUnitID = "";

        [SerializeField] internal string AOS_IS_Backup_AdUnitID = "";
        [SerializeField] internal string AOS_RV_Backup_AdUnitID = "";
        [SerializeField] internal string AOS_BN_Backup_AdUnitID = "";

        [SerializeField] internal string AOS_APS_AppID = "";
        [SerializeField] internal string AOS_APS_IS_SlotID = "";
        [SerializeField] internal string AOS_APS_RV_SlotID = "";
        [SerializeField] internal string AOS_APS_BN_SlotID = "";

        [SerializeField] internal string AOS_PlayOnApiKey = "";
        [SerializeField] internal string AOS_AdvertyApiKey = "";

        [SerializeField] internal string iOS_IS_AdUnitID = "";
        [SerializeField] internal string iOS_RV_AdUnitID = "";
        [SerializeField] internal string iOS_BN_AdUnitID = "";

        [SerializeField] internal string iOS_IS_Backup_AdUnitID = "";
        [SerializeField] internal string iOS_RV_Backup_AdUnitID = "";
        [SerializeField] internal string iOS_BN_Backup_AdUnitID = "";

        [SerializeField] internal string iOS_APS_AppID = "";
        [SerializeField] internal string iOS_APS_IS_SlotID = "";
        [SerializeField] internal string iOS_APS_RV_SlotID = "";
        [SerializeField] internal string iOS_APS_BN_SlotID = "";

        [SerializeField] internal string iOS_PlayOnApiKey = "";
        [SerializeField] internal string iOS_storeID = "";
        [SerializeField] internal string iOS_AdvertyApiKey = "";

        [SerializeField] internal bool showPlayOnAfterInterstitial = true;
        [SerializeField] internal int playOnInterstitialCount = 2;
        [SerializeField] internal PlayOnPosition playOnPosition = default;
        [SerializeField] internal bool initializeAdvertyOnAwake = true;

#if UNITY_IOS
        internal string playOnAPIKey => iOS_PlayOnApiKey;

        internal string advertyApiKey => iOS_AdvertyApiKey;

        internal string apsAppId => iOS_APS_AppID;
        internal string apsInterstitialSlotId => iOS_APS_IS_SlotID;
        internal string apsRewardedSlotId => iOS_APS_RV_SlotID;
        internal string apsBannerSlotId => iOS_APS_BN_SlotID;
#else
        internal string playOnAPIKey => AOS_PlayOnApiKey;
        internal string advertyApiKey => AOS_AdvertyApiKey;

        internal string apsAppId => AOS_APS_AppID;
        internal string apsInterstitialSlotId => AOS_APS_IS_SlotID;
        internal string apsRewardedSlotId => AOS_APS_RV_SlotID;
        internal string apsBannerSlotId => AOS_APS_BN_SlotID;
#endif
        internal bool hasInterstitial => !string.IsNullOrEmpty(interstitialAdUnitId);
        internal bool hasRewarded => !string.IsNullOrEmpty(rewardedAdUnitId);
        internal bool hasBanner => !string.IsNullOrEmpty(bannerAdUnitId);

        internal string interstitialAdUnitId => interstitialAdUnitIDs[interstitialAdUnitLevel];
        internal string rewardedAdUnitId => rewardedAdUnitIDs[rewardedAdUnitLevel];
        internal string bannerAdUnitId => bannerAdUnitIDs[bannerAdUnitLevel];
        internal AdUnitLevel interstitialAdUnitLevel = AdUnitLevel.Backup;
        internal AdUnitLevel rewardedAdUnitLevel = AdUnitLevel.Backup;
        internal AdUnitLevel bannerAdUnitLevel = AdUnitLevel.Backup;

        private Dictionary<AdUnitLevel, string> interstitialAdUnitIDs = default;
        private Dictionary<AdUnitLevel, string> rewardedAdUnitIDs = default;
        private Dictionary<AdUnitLevel, string> bannerAdUnitIDs = default;

        internal System.Func<bool> IsNoAds = () => false;

        internal bool HasAPSKey() {
#if UNITY_EDITOR
            return false;
#endif
            return !string.IsNullOrEmpty(apsAppId);
        }

        internal bool HasAPSKey(AdType adType) {
            if (!HasAPSKey()) {
                return false;
            }

            switch (adType) {
                case AdType.Interstitial:
                    return !string.IsNullOrEmpty(apsInterstitialSlotId);
                case AdType.Rewarded:
                    return !string.IsNullOrEmpty(apsRewardedSlotId);
                case AdType.Banner:
                    return !string.IsNullOrEmpty(apsBannerSlotId);
            }

            return false;
        }

        internal void Initialize() {
            // Backup check on initialization
            if (Retention.Days < BACKUP_RETENTION_THRESHOLD) {
                interstitialAdUnitLevel = rewardedAdUnitLevel = bannerAdUnitLevel = AdUnitLevel.Default;
            } else {
                interstitialAdUnitLevel = rewardedAdUnitLevel = bannerAdUnitLevel = AdUnitLevel.Backup;
            }

            interstitialAdUnitIDs = new Dictionary<AdUnitLevel, string>() {
#if UNITY_IOS
                {AdUnitLevel.Default, iOS_IS_AdUnitID},
                {AdUnitLevel.Backup, string.IsNullOrEmpty(iOS_IS_Backup_AdUnitID)? iOS_IS_AdUnitID : iOS_IS_Backup_AdUnitID},
#else
            { AdUnitLevel.Backup, AOS_IS_AdUnitID},
                { AdUnitLevel.Default, string.IsNullOrEmpty(AOS_IS_Backup_AdUnitID)? AOS_IS_AdUnitID : AOS_IS_Backup_AdUnitID },
#endif
        };

            rewardedAdUnitIDs = new Dictionary<AdUnitLevel, string>(){
#if UNITY_IOS
                {AdUnitLevel.Default, iOS_RV_AdUnitID},
                {AdUnitLevel.Backup, string.IsNullOrEmpty(iOS_RV_Backup_AdUnitID)? iOS_RV_AdUnitID : iOS_RV_Backup_AdUnitID},
#else
                {AdUnitLevel.Backup, AOS_RV_AdUnitID},
                {AdUnitLevel.Default, string.IsNullOrEmpty(AOS_RV_Backup_AdUnitID)? AOS_RV_AdUnitID : AOS_RV_Backup_AdUnitID},
#endif
            };
            bannerAdUnitIDs = new Dictionary<AdUnitLevel, string>(){
#if UNITY_IOS
                {AdUnitLevel.Default, iOS_BN_AdUnitID},
                {AdUnitLevel.Backup, string.IsNullOrEmpty(iOS_BN_Backup_AdUnitID)? iOS_BN_AdUnitID : iOS_BN_Backup_AdUnitID},
#else
                {AdUnitLevel.Backup, AOS_BN_AdUnitID},
                {AdUnitLevel.Default, string.IsNullOrEmpty(AOS_BN_Backup_AdUnitID)? AOS_BN_AdUnitID : AOS_BN_Backup_AdUnitID},
#endif
            };
        }
    }

    [System.Serializable]
    internal class PlayOnPosition {
        [SerializeField] internal bool useScreenPositioning = false;
        [Tooltip("Anchor position of logo ad")]
        [SerializeField][DrawIf("useScreenPositioning")] internal PlayOnSDK.Position playOnLogoAnchor = PlayOnSDK.Position.TopLeft;
        [Tooltip("Offset from anchor")]
        [SerializeField][DrawIf("useScreenPositioning")] internal Vector2Int playOnLogoOffset = new Vector2Int(0, 150);
        [Tooltip("Logo ad size")]
        [SerializeField][DrawIf("useScreenPositioning")] internal int playOnLogoSize = 64;
    }
}