using System;

namespace Dktech.Services.Advertisement
{
    public abstract class RewardedAd : AdBase
    {
        public static Action OnAdClosed { get; set; }
        public const string ADMOB_LABEL = "DKTech SDK: Rewarded Ads Admob ";
        public const string APPLOVIN_LABEL = "DKTech SDK: Rewarded Ads Applovin ";
        public const string IRONSOURCE_LABEL = "DKTech SDK: Rewarded Ads IronSource ";
        protected static Action<bool> rewardedAction { get; set; }
        public static bool IsShowing { get; set; }
        protected int retryAttempt;
        protected bool rewardEarned;
        public void Setup(AdRequestInfo adRequest)
        {
            SetAdInfo(adRequest);
            this.RequestLoadAd();
        }
        public abstract void ShowAd(Action<bool> rewardedAction);
    }
}