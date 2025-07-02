using System;

namespace Dktech.Services.Advertisement
{
    public abstract class NativeAd : AdBase
    {
        protected const string ADMOB_LABEL = "DKTech SDK: Native Ads Admob ";
        protected const string APPLOVIN_LABEL = "DKTech SDK: Native Ads Applovin ";
        protected bool isLoaded;
        protected bool isShowing;
        public bool IsShowing => isShowing;
        public bool IsLoaded => isLoaded;
        public static bool IsAdClicked { get; set; }
        protected int retryAttempt;
        internal Action actionShowed;
        internal Action<bool> actionClosed;
        public void Setup(AdRequestInfo adRequest)
        {
            SetAdInfo(adRequest);
#if !UNITY_EDITOR
            this.RequestLoadAd();
            #endif
        }
        public abstract void ShowAd(NativeAdContent nativeAdContent, Action actShowed = null, Action<bool> actClosed = null);
        public abstract void HideAd();
    }
}