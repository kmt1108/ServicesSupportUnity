using Dktech.Services.Firebase;
using System;
using UnityEngine;

namespace Dktech.Services.Advertisement
{
    public class BannerAdApplovin : BannerAd
    {
#if applovin_enabled

        public override void LoadAd()
        {
            isLoaded = false;
            // Banners are automatically sized to 320x50 on phones and 728x90 on tablets
            // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
            MaxSdk.CreateBanner(id, MaxSdkBase.BannerPosition.BottomCenter);
            //MaxSdk.SetBannerExtraParameter(bannerAdUnitId, "adaptive_banner", "true");
            // Set background or background color for banners to be fully functional
            MaxSdk.SetBannerBackgroundColor(currentID, Color.clear);
            Debug.Log($"{APPLOVIN_LABEL} loading: {name}, id:{currentID}");
        }
        public override void LoadAndShowAd()
        {
            isLoaded=false;
            // Banners are automatically sized to 320x50 on phones and 728x90 on tablets
            // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
            MaxSdk.CreateBanner(id, MaxSdkBase.BannerPosition.BottomCenter);
            //MaxSdk.SetBannerExtraParameter(bannerAdUnitId, "adaptive_banner", "true");
            // Set background or background color for banners to be fully functional
            MaxSdk.SetBannerBackgroundColor(currentID, Color.clear);
            Debug.Log($"{APPLOVIN_LABEL} loading: {name}, id:{currentID}");
            MaxSdk.ShowBanner(id);
            isShowing = true;
        }
        #region Events Listener
        internal void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            this.LoadAdComplete();
            Debug.Log($"{APPLOVIN_LABEL} loaded {name} with response : {adInfo}");
            isLoaded = true;
            OnAdLoaded?.Invoke(this);
        }

        internal void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            this.LoadAdComplete();
            Debug.LogError($"{APPLOVIN_LABEL} failed to load {name} with error : {errorInfo}");
            if (HaveInternet) FirebaseManager.TrackEvent("BANNER_ADS_LOAD_FAIL");
        }

        internal void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log(APPLOVIN_LABEL+adUnitId+" was clicked");
            IsAdClicked = true;
        }

        internal void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log(String.Format(APPLOVIN_LABEL + "paid {0}.",
                    adInfo.Revenue));
            FirebaseManager.SendRevFirebase(adInfo);
            FirebaseManager.SendRevAdjust(adInfo);
        }

        internal void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log(APPLOVIN_LABEL + " expanded");
        }

        internal void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log(APPLOVIN_LABEL + " collapsed");
        }
        #endregion

        public override void HideAd()
        {
            if (isShowing)
            {
                isShowing = false;
                MaxSdk.HideBanner(id);
            }
        }

        public override void DestroyAd()
        {
            if (isShowing)
            {
                isShowing = false;
                MaxSdk.HideBanner(id);
            }
            MaxSdk.DestroyBanner(id);
        }

        public override void ShowAd()
        {
            MaxSdk.ShowBanner(id);
            isShowing = true;
        }

        public override bool IsAvailable()
        {
            return !AdsUtilities.NoAds;
        }
#else
        public override void HideAd()
        {
            throw new NotImplementedException();
        }

        public override void LoadAd()
        {
            throw new NotImplementedException();
        }
        public override void LoadAndShowAd()
        {
            throw new NotImplementedException();
        }

        public override void ShowAd()
        {
            throw new NotImplementedException();
        }

        public override void DestroyAd()
        {
            throw new NotImplementedException();
        }

        public override bool IsAvailable()
        {
            throw new NotImplementedException();
        }


#endif
    }
}