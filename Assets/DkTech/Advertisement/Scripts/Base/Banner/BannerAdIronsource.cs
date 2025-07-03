#if ironsource_enabled
using com.unity3d.mediation;
using Dktech.Services.Firebase;
#endif
using System;
using UnityEngine;

namespace Dktech.Services.Advertisement
{
    public class BannerAdIronSource : BannerAd
    {

#if ironsource_enabled
        LevelPlayBannerAd bannerAd;
#endif
        int retryAttempt;
        protected override void SetAdInfo(AdRequestInfo adRequest)
        {
            base.SetAdInfo(adRequest);
            isCollapsible = ((BannerAdRequest)adRequest).isCollapsible;
        }

#if ironsource_enabled
        public override void LoadAd()
        {
            if (waitingDelayReload) return;
            if (bannerAd != null)
            {
                DestroyAd();
            }
            isLoaded = false;
            bannerAd = new LevelPlayBannerAd(currentID, LevelPlayAdSize.CreateAdaptiveAdSize(), position switch
            {
                BannerPosition.Top => LevelPlayBannerPosition.TopCenter,
                _ => LevelPlayBannerPosition.BottomCenter
            },displayOnLoad:false);

            // Load the banner with the request.
            RegisterBannerIronSourceEventHandlers();
            bannerAd.LoadAd();
            isShowing = false;
            Debug.Log($"{IRONSOURCE_LABEL} loading: {name}, id:{currentID}");
        }
        public override void LoadAndShowAd()
        {
            if (waitingDelayReload) return;
            if (bannerAd != null)
            {
                DestroyAd();
            }
            isLoaded = false;
            bannerAd = new LevelPlayBannerAd(currentID, LevelPlayAdSize.CreateAdaptiveAdSize(), position switch
            {
                BannerPosition.Top => LevelPlayBannerPosition.TopCenter,
                _ => LevelPlayBannerPosition.BottomCenter
            }, displayOnLoad: true);

            // Load the banner with the request.
            RegisterBannerIronSourceEventHandlers();
            bannerAd.LoadAd();
            isShowing = false;
            Debug.Log($"{IRONSOURCE_LABEL} loading: {name}, id:{currentID}");
        }

        private void RegisterBannerIronSourceEventHandlers()
        {
            // Raised when an ad is loaded into the banner view.
            bannerAd.OnAdLoaded += (info) =>
            {
                Debug.Log($"{IRONSOURCE_LABEL} loaded {name} with response : {info}");
                isLoaded = true;
                OnAdLoaded?.Invoke(this);
                retryAttempt = 0;
            };
            // Raised when an ad fails to load into the banner view.
            bannerAd.OnAdLoadFailed += (error) =>
            {
                Debug.LogError($"{IRONSOURCE_LABEL} failed to load {name} with error : {error}");
                if (HaveInternet) FirebaseManager.TrackEvent("BANNER_ADS_LOAD_FAIL");
                if (isShowing)
                {
                    isShowing = false;
                }
                CheckSwitchID();
                retryAttempt++;
                double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
                DelayLoadAd((int)retryDelay*1000);
            };
            // Raised when a click is recorded for an ad.
            bannerAd.OnAdClicked += (info) =>
            {
                Debug.Log(IRONSOURCE_LABEL+"was clicked.");
                IsAdClicked = true;
            };
        }
        public override void HideAd()
        {
            if (bannerAd != null && isShowing)
            {
                if (autoReload)
                {
                    if (IsCollapsible) LoadAd();
                    else bannerAd.HideAd();
                }
                else
                {
                    bannerAd.DestroyAd();
                }
                isShowing = false;
            }
        }

        public override void DestroyAd()
        {
            if (bannerAd != null)
            {
                if (isShowing)
                {
                    bannerAd.HideAd();
                    isShowing = false;
                }
                bannerAd.DestroyAd();
            }
        }

        public override void ShowAd()
        {
            if (IsAvailable())
            {
                bannerAd.ShowAd();
                isShowing = true;
            }
        }

        public override bool IsAvailable()
        {
            return bannerAd != null&&isLoaded&&!AdsUtilities.NoAds;
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

        public override void LoadAndShowAd()
        {
            throw new NotImplementedException();
        }
#endif
    }
}