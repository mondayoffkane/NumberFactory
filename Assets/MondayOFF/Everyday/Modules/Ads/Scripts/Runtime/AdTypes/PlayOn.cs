using UnityEngine;

namespace MondayOFF {
    internal sealed class PlayOn : AdTypeBase {
        const string AD_PLATFORM = "odeeo";
        const string AD_CURRENCY = "USD";
        private AdUnit _adUnit = default;
        private int _shownCount = 0;

        public override void Dispose() {
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent -= AutoShowPlayOn;
            _adUnit.Dispose();
            _adUnit = null;
        }

        internal override bool IsReady() {
            if (!PlayOnSDK.IsInitialized()) {
                EverydayLogger.Info("PlayOn is not initialized");
                return false;
            }

            return true;
        }

        internal override bool Show() {
            if (EverydaySettings.AdSettings.showPlayOnAfterInterstitial && EverydaySettings.AdSettings.playOnInterstitialCount > 0) {
                EverydayLogger.Warn($"PlayOn is set to show after {EverydaySettings.AdSettings.playOnInterstitialCount}. Do you really want to manually show PlayOn?");
                return false;
            }

            if (!_adUnit.IsAdAvailable()) {
                EverydayLogger.Info("PlayOn Ad Unit is not available yet");
                return false;
            }
            EverydayLogger.Info("Show PlayOn");
            _adUnit.ShowAd();
            return true;
        }

        internal void Hide() {
            EverydayLogger.Info("Hide PlayON");
            _adUnit.CloseAd();
        }

        internal void LinkLogoToRectTransform(PlayOnSDK.Position position, RectTransform rectTransform, Canvas canvas) {
            if (EverydaySettings.AdSettings.playOnPosition.useScreenPositioning) {
                EverydayLogger.Info("Using legacy PlayOn positioning! Do not use PlayOnAnchor");
                return;
            }
            _adUnit.LinkLogoToRectTransform(position, rectTransform, canvas);
        }

        internal PlayOn() { }

        internal void Initialize() {
            _adUnit = new AdUnit(PlayOnSDK.AdUnitType.AudioLogoAd);

            // _adUnit.SetProgressBar(Color.black);
            // _adUnit.SetVisualization(Color.black, Color.red);

            if (EverydaySettings.AdSettings.playOnPosition.useScreenPositioning) {
                _adUnit.SetLogo(
                    EverydaySettings.AdSettings.playOnPosition.playOnLogoAnchor,
                    EverydaySettings.AdSettings.playOnPosition.playOnLogoOffset.x,
                    EverydaySettings.AdSettings.playOnPosition.playOnLogoOffset.y,
                    EverydaySettings.AdSettings.playOnPosition.playOnLogoSize);
            }

            PlayOnSDK.OnInitializationFinished += OnInitialized;
            PlayOnSDK.OnInitializationFailed += (errorCode, errorMessage) => {
                EverydayLogger.Error($"PlayOn initialization failed with error code {errorCode} and message {errorMessage}");
            };

            if (EverydaySettings.AdSettings.showPlayOnAfterInterstitial && EverydaySettings.AdSettings.playOnInterstitialCount > 0) {
                MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += AutoShowPlayOn;
            }
            PlayOnSDK.Initialize(EverydaySettings.AdSettings.playOnAPIKey, EverydaySettings.AdSettings.iOS_storeID);
        }

        private void OnInitialized() {
            PlayOnSDK.OnInitializationFinished -= OnInitialized;

            var mainThreadDispatcherInstance = UnityMainThreadDispatcher.Instance();
            mainThreadDispatcherInstance.gameObject.AddComponent<ApplicationLifecycleTracker>();

            PlayOnSDK.SetGdprConsent(AdsManager.HAS_USER_CONSENT);
            PlayOnSDK.SetDoNotSell(!AdsManager.HAS_USER_CONSENT, AdsManager.US_PRIVACY_STRING);
            PlayOnSDK.SetIsChildDirected(false);

            _adUnit.AdCallbacks.OnImpression += OnAdImpression;
        }

        private void AutoShowPlayOn(string _, MaxSdk.AdInfo __) {
            if (_shownCount++ % EverydaySettings.AdSettings.playOnInterstitialCount == 0) {
                if (!_adUnit.IsAdAvailable()) {
                    EverydayLogger.Info("PlayOn Ad Unit is not available yet");
                    _shownCount--;
                    return;
                }
                _adUnit.ShowAd();
            }
        }

        private void OnAdImpression(AdUnit.ImpressionData impressionData) {
            PlayOnSDK.AdUnitType adType = impressionData.GetAdType();
            double revenue = impressionData.GetRevenue();
            string placementID = impressionData.GetPlacementID();

            // Initialize the SingularAdData object with the relevant data
            SingularAdData singularAdData =
                new SingularAdData(AD_PLATFORM, AD_CURRENCY, revenue / 1000d)
                    .WithPlacementId(placementID)
                    .WithAdType(adType.ToString());

            // Report the data to Singular
            SingularSDK.AdRevenue(singularAdData);
        }

        private class ApplicationLifecycleTracker : MonoBehaviour {
            private void OnApplicationPause(bool pauseStatus) {
                EverydayLogger.Info($"PlayOn OnApplicationPause({pauseStatus})");
                PlayOnSDK.onApplicationPause(pauseStatus);
            }
        }
    }
}