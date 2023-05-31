using UnityEngine;

namespace MondayOFF {
    internal partial class EverydaySettings : ScriptableObject {
        public static EverydaySettings Instance {
            get {
#if UNITY_EDITOR
                var settingAssets = UnityEditor.AssetDatabase.FindAssets("t:EverydaySettings");

                if (settingAssets.Length != 1) {
                    throw new UnityEditor.Build.BuildFailedException("[EVERYDAY] There should be ONLY 1 settings object.");
                }

                instance = UnityEditor.AssetDatabase.LoadAssetAtPath<EverydaySettings>(UnityEditor.AssetDatabase.GUIDToAssetPath(settingAssets[0]));
#else
                var assets = Resources.LoadAll<EverydaySettings>("EverydaySettings");
                if (assets == null || assets.Length <= 0) {
                    Debug.Log("NOT found, search all");
                    assets = Resources.LoadAll<EverydaySettings>("");
                }
                if (assets.Length != 1) {
                    EverydayLogger.Error($"Found 0 or multiple {typeof(EverydaySettings).Name}s in Resources folder. There should only be one.");
                } else {
                    instance = assets[0];
                }
#endif
                Debug.Assert(instance != null, "EverydaySettings asset not found.");

                return instance;
            }
        }

        public static AdSettings AdSettings => Instance?.adSettings;
        private static EverydaySettings instance;

        [SerializeField] internal LogLevel logLevel = LogLevel.Warning;
        [SerializeField] internal bool isTestMode = false;
        [SerializeField] internal AdSettings adSettings = default;
        [SerializeField] internal string appId = "";
    }
}