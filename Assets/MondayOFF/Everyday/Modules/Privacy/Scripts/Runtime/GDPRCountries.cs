using UnityEngine;
using System.Runtime.InteropServices;

namespace MondayOFF {
    internal static class GDPR {
        private static string[] Countries = new[]{
            "AT",
            "BE",
            "BG",
            "HR",
            "CY",
            "CZ",
            "DK",
            "EE",
            "FI",
            "FR",
            "DE",
            "GR",
            "HU",
            "IE",
            "IT",
            "LV",
            "LT",
            "LU",
            "MT",
            "NL",
            "PL",
            "PT",
            "RO",
            "SK",
            "SI",
            "ES",
            "SE",
            "GB",
            "IS",
            "LI",
            "NO",
            "CH",
            "AD",
            "MC",
            "SM",
            "VA",
            "AX",
            "FO",
            "GL",
            "GF",
            "GP",
            "MQ",
            "RE",
            "MF",
            "PM",
            "YT",
            "BL",
            "SX",
            "AW",
            "CW",
            "WF",
            "PF",
            "NC",
            "TF",
            "AI",
            "BM",
            "IO",
            "VG",
            "KY",
            "FK",
            "GI",
            "MS",
            "PN",
            "SH",
            "GS",
            "TC",
            "AD",
            "LI",
            "MC",
            "SM",
            "VA",
            "ME",
            "RS",
            "UM",
            "SJ",
            "BQ",
            "GG",
            "JE",
            "IM",
            "BL",
            "MF",
        };

        internal static bool IsApplicable() {
            string CountryCode = GetCountry();
            return System.Array.IndexOf(Countries, CountryCode) > -1;
        }

#if UNITY_IOS && !UNITY_EDITOR
        private static string GetCountry() {
            return _GetLocale();
        }

        [DllImport("__Internal")]
        private static extern string _GetLocale();

#elif UNITY_ANDROID && !UNITY_EDITOR
        private static string GetCountry() {
            AndroidJavaClass resourcesClass = new AndroidJavaClass("android.content.res.Resources");
            AndroidJavaObject system = resourcesClass.CallStatic<AndroidJavaObject>("getSystem");
            AndroidJavaObject configuration = system.Call<AndroidJavaObject>("getConfiguration");
            AndroidJavaObject locale = configuration.Get<AndroidJavaObject>("locale");
            string country = locale.Call<string>("getCountry");
            return country;
        }
#else
        private static string GetCountry() {
            return "US";
        }
#endif
    }
}