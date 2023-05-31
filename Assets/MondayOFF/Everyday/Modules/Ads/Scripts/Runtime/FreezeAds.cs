using System;
using UnityEngine;

namespace MondayOFF {
    internal static class FreezeAds {
        const string FREEZE_ADS_TIMESTAMP = "MOFF_FreezeAds_TimeStamp";
        internal static void BeginFreeze() {
            PlayerPrefs.SetString(key: FREEZE_ADS_TIMESTAMP, DateTime.Now.AddHours(24).ToString());
            PlayerPrefs.Save();
        }

        internal static bool isFrozen() {
            var freezeTime = PlayerPrefs.GetString(FREEZE_ADS_TIMESTAMP, "");
            if (!DateTime.TryParse(freezeTime, out var freezeTimeParsed)) {
                EverydayLogger.Warn($"Failed to parse freeze time: {freezeTime}");
                PlayerPrefs.DeleteKey(FREEZE_ADS_TIMESTAMP);
                return false;
            }

            return DateTime.Now < freezeTimeParsed;
        }
    }
}