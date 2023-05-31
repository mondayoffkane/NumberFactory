
using UnityEngine;
using System.Runtime.InteropServices;

namespace MondayOFF {
    /************************************************************************************************
        MondayOFFAppTracking.RequestTrackingAuthorization(callbackFunction(bool)) 만 호출 하시면 됩니다.
        유저가 ok를 누른 경우에만 callbackFunction에 true가 들어갑니다.
    ************************************************************************************************/
    public static class EverydayAppTracking {
#if UNITY_IOS && !UNITY_EDITOR
        private static System.Action<AttAuthorizationStatus> OnAppTrackingAllow = default;

        /// <summary>Requests App Tracking Authorization to a user.</summary>
        /// <param name="onAllowCallback">Delegate to be called on authorization. True only if the user allows app tracking.</param>
        internal static void RequestTrackingAuthorization(System.Action<AttAuthorizationStatus> onAllowCallback) {
            OnAppTrackingAllow = onAllowCallback;
            _RequestTrackingAuthorization(OnCompleteCallback);
        }

        [DllImport("__Internal")]
        private static extern void _RequestTrackingAuthorization(System.Action<AttAuthorizationStatus> onAllowCallback);

        [AOT.MonoPInvokeCallback(typeof(System.Action<int>))]
        private static void OnCompleteCallback(AttAuthorizationStatus status) {
            OnAppTrackingAllow?.Invoke(status);
        }

#else
        internal static void RequestTrackingAuthorization(System.Action<AttAuthorizationStatus> onAllowCallback) {
            // No action required for Android
            onAllowCallback?.Invoke(AttAuthorizationStatus.Authorized);
        }
#endif
    }
}