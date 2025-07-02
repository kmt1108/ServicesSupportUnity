using System;
using UnityEngine;

[Serializable]
public enum AppOpenPosition {AppOpen=0,OnResume=1}
namespace Dktech.Services.Advertisement
{
    public abstract class AppOpenAd : AdBase
    {
        public static Action OnStartShowAd { get; set; }
        public static Action OnAdShow { get; set; }
        public static Action OnAdShowFailed { get; set; }
        public static Action OnAdClosed { get; set; }
        protected const string ADMOB_LABEL = "DKTech SDK: App Open Ads Admob ";
        protected const string APPLOVIN_LABEL = "DKTech SDK: App Open Ads Applovin ";
        [SerializeField] protected AppOpenPosition position;
        public AppOpenPosition Position => position;
        protected int retryAttempt;
        public static bool IsShowing { get; set; }
        protected override void SetAdInfo(AdRequestInfo adRequest)
        {
            base.SetAdInfo(adRequest);
            this.position = ((AppOpenAdRequest)adRequest).position;
        }
        public void Setup(AdRequestInfo adRequest)
        {
            SetAdInfo(adRequest);
#if UNITY_EDITOR
            DelayLoadAd(200);
#else
            this.RequestLoadAd();
#endif
        }
    }
}