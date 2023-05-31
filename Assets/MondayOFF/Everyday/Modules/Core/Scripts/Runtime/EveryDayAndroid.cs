// #if UNITY_ANDROID && !UNITY_EDITOR
// using UnityEngine;

// namespace MondayOFF {
//     public static partial class EveryDay {
//         private static void PrepareSettings(in AttAuthorizationStatus consentStatus) {
//             // MAX
//             MaxSdk.SetHasUserConsent(consentStatus);
//             MaxSdk.SetDoNotSell(!consentStatus);

//             // FB
//             AudienceNetwork.AdSettings.SetDataProcessingOptions(new string[] { });

//             // Privacy string
//             AdsManager.US_PRIVACY_STRING = "1---";
//             AdsManager.HAS_USER_CONSENT = consentStatus;
//         }
//     }
// }
// #endif