using System;
using UnityEngine;

namespace MondayOFF {
    internal static class Retention {
        const string LAST_LAUNCH_DATE = "MOFF_LastLaunchDate";
        private static int _days = -1;

        internal static int Days {
            get {
                if (_days < 0) {
                    _days = 0;
                    var lastLaunchDate = PlayerPrefs.GetString(LAST_LAUNCH_DATE, "");
                    var lastLaunchDateParsed = DateTime.MinValue;
                    if (DateTime.TryParse(lastLaunchDate, out lastLaunchDateParsed)) {
                        var today = DateTime.Today;
                        var daysDiff = (today - lastLaunchDateParsed).TotalDays;
                        if (daysDiff > 0) {
                            _days = (int)daysDiff;
                        }
                    }

                    PlayerPrefs.SetString(LAST_LAUNCH_DATE, DateTime.Today.ToString());
                    PlayerPrefs.Save();
                }
                return _days;
            }
        }
    }
}