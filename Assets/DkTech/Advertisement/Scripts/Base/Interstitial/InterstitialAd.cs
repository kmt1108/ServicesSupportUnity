using System;

namespace Dktech.Services.Advertisement
{
    public abstract class InterstitialAd : AdBase
    {
        public static Action OnStartShowAd { get; set; }
        public static Action OnAdShowed { get; set; }
        public static Action OnAdShowFailed { get; set; }
        public static Action OnAdClosed { get; set; }
        protected const string ADMOB_LABEL = "DKTech SDK: Intersitial Ads Admob ";
        protected const string APPLOVIN_LABEL = "DKTech SDK: Intersitial Ads Applovin ";
        protected const string IRONSOURCE_LABEL = "DKTech SDK: Intersitial Ads IronSource ";
        public static bool IsShowing { get; set; }
        protected int retryAttempt;
        public void Setup(AdRequestInfo adRequest)
        {
            SetAdInfo(adRequest);
            this.RequestLoadAd();
        }
    }
}