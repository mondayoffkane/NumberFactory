#if EVERYDAY_NO_LOG
using System.Diagnostics;
#endif

namespace MondayOFF {
    public static class EverydayLogger {
        private const string EVERYDAY_TAG = "[EVERYDAY]";
#if UNITY_EDITOR
        private const string ERROR_TAG = "[<color=red>ERROR</color>]";
        private const string WARN_TAG = "[<color=yellow>WARN</color>]";
        private const string INFO_TAG = "[<color=blue>INFO</color>]";
#else
        private const string ERROR_TAG = "[ERROR]";
        private const string WARN_TAG = "[WARN]";
        private const string INFO_TAG = "[INFO]";
#endif

#if EVERYDAY_NO_LOG
        [Conditional("EVERYDAY_DONT_SHOW_LOGGING_NAME_SOMETHING_NEVER_USED")]
#endif
        public static void Error(string message) {
#if !UNITY_EDITOR
            if (EverydaySettings.Instance.logLevel >= LogLevel.Error)
#endif
            UnityEngine.Debug.LogError($"{EVERYDAY_TAG}{ERROR_TAG} {message}");

        }

#if EVERYDAY_NO_LOG
        [Conditional("EVERYDAY_DONT_SHOW_LOGGING_NAME_SOMETHING_NEVER_USED")]
#endif
        public static void Warn(string message) {
#if !UNITY_EDITOR
            if (EverydaySettings.Instance.logLevel >= LogLevel.Warning)
#endif
            UnityEngine.Debug.LogWarning($"{EVERYDAY_TAG}{WARN_TAG} {message}");

        }

#if EVERYDAY_NO_LOG
        [Conditional("EVERYDAY_DONT_SHOW_LOGGING_NAME_SOMETHING_NEVER_USED")]
#endif
        public static void Info(string message) {
#if !UNITY_EDITOR
            if (EverydaySettings.Instance.logLevel >= LogLevel.Info)
#endif
            UnityEngine.Debug.Log($"{EVERYDAY_TAG}{INFO_TAG} {message}");
        }
    }
}