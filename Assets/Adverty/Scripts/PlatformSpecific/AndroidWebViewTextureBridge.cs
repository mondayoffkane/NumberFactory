using System;
using Adverty.Native;
#if UNITY_ANDROID && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace Adverty.PlatformSpecific
{
    public class AndroidWebViewTextureBridge : BaseWebViewTextureBridge, IAndroidWebViewTextureBridge
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        [DllImport(LIBRARY_NAME)]
        private static extern IntPtr getCustomRenderEventFunc();

        [DllImport(LIBRARY_NAME)]
        private static extern void setOnPageStarted(IntPtr ptr, IntPtr onPageStartedDelegate);

        [DllImport(LIBRARY_NAME)]
        private static extern IntPtr create(int cbo, int width, int height, int loadTimeout, int scale, bool showDebugView);
#endif
        public IntPtr GetCustomRenderEventFunc()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return getCustomRenderEventFunc();
#else
            return IntPtr.Zero;
#endif
        }

        public override IntPtr Create(IntPtr cbo, int width, int height, int loadTimeout, int scale, bool showDebugView)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return create(cbo.ToInt32(), width, height, loadTimeout, scale, showDebugView);
#else
            return IntPtr.Zero;
#endif
        }

        public void SetOnPageStarted(IntPtr ptr, IntPtr onPageStartedDelegate)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            setOnPageStarted(ptr, onPageStartedDelegate);
#endif
        }
    }
}
