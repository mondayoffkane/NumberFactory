#if !UNITY_EDITOR
using UnityEngine;

namespace MondayOFF {
    public static partial class EveryDay {
        private static void PrepareSettings(in AttAuthorizationStatus consentStatus) {
            AdsManager.HAS_USER_CONSENT = consentStatus == AttAuthorizationStatus.Authorized;
            // MAX
            MaxSdk.SetHasUserConsent(AdsManager.HAS_USER_CONSENT);
            MaxSdk.SetDoNotSell(!AdsManager.HAS_USER_CONSENT);
            MaxSdk.SetIsAgeRestrictedUser(false);
            
            // FB
#if UNITY_IOS
            AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(AdsManager.HAS_USER_CONSENT);
#endif
            if (AdsManager.HAS_USER_CONSENT) {
                AudienceNetwork.AdSettings.SetDataProcessingOptions(new string[] { });
            } else {
                AudienceNetwork.AdSettings.SetDataProcessingOptions(new string[] { "LDU" }, 0, 0);
            }

            // Privacy string
            char[] privacyCharacters = new char[4];
            privacyCharacters[0] = '1';
            privacyCharacters[1] = 'Y';
            if (AdsManager.HAS_USER_CONSENT) {
                privacyCharacters[2] = 'N';
            } else {
                privacyCharacters[2] = 'Y';
            }
            privacyCharacters[3] = 'N';

            AdsManager.US_PRIVACY_STRING = new string(privacyCharacters);

            // if (!PlayerPrefs.HasKey("IABTCF_TCString")) {
            //     PlayerPrefs.SetInt("IABTCF_gdprApplies", 0);
            // }
        }
    }
}
#endif
