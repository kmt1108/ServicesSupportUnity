using System;
using UnityEngine;
[Serializable]
public enum BannerPosition { Bottom = 0, Top = 1, Center = 2 }

namespace Dktech.Services.Advertisement
{
    [Serializable]
    public abstract class BannerAd : AdBase
    {
        protected const string ADMOB_LABEL = "DKTech SDK: Banner Ads Admob ";
        protected const string APPLOVIN_LABEL = "DKTech SDK: Banner Ads Applovin ";
        protected const string IRONSOURCE_LABEL = "DKTech SDK: Banner Ads Ironsource ";
        [SerializeField] protected BannerPosition position;
        public BannerPosition Position => position;
        protected bool isCollapsible;
        protected bool isLoaded;
        protected bool isShowing;
        public bool IsShowing => isShowing;
        public bool IsCollapsible => isCollapsible;
        public bool IsLoaded => isLoaded;
        public static bool IsAdClicked { get; set; }
        protected override void SetAdInfo(AdRequestInfo adRequest)
        {
            base.SetAdInfo(adRequest);
            this.position = ((BannerAdRequest)adRequest).adPosition;
        }
        public void Setup(BannerAdRequest adRequest)
        {
            SetAdInfo(adRequest);
            this.RequestLoadAd();
        }
        public abstract void LoadAndShowAd();
        public abstract void HideAd();
    }
}