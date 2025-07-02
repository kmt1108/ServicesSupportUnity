using System;
using UnityEngine;

namespace Dktech.Services.Advertisement
{
    [Serializable] 
    public abstract class NativeOverlayAd : AdBase
    {
        protected const string ADMOB_LABEL = "DKTech SDK: Native Overlay Ads Admob ";
        protected const string APPLOVIN_LABEL = "DKTech SDK: Native Overlay Ads Applovin ";
        [SerializeField] protected BannerPosition position;
        [SerializeField] protected NativeStyle style;
        public BannerPosition Position => position;
        public NativeStyle Style => style;
        protected bool isShowing;
        protected int retryAttempt;
        public bool IsShowing => isShowing;
        public static bool IsAdClicked { get; set; }
        protected override void SetAdInfo(AdRequestInfo adRequest)
        {
            base.SetAdInfo(adRequest);
            this.position = ((NativeOverlaysAdRequest)adRequest).adPosition;
            this.style = ((NativeOverlaysAdRequest)adRequest).style;

        }
        public void Setup(AdRequestInfo adRequest)
        {
            SetAdInfo(adRequest);
            this.RequestLoadAd();
        }
        public abstract void HideAd();
    }
}