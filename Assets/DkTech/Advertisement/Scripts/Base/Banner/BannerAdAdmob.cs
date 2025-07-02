#if admob_enabled
using Dktech.Services.Firebase;
using GoogleMobileAds.Api;
#endif
using System;
using UnityEngine;

namespace Dktech.Services.Advertisement
{
    public class BannerAdAdmob : BannerAd
    {
#if admob_enabled
        BannerView bannerAd;
#endif
        int retryAttempt;
        protected override void SetAdInfo(AdRequestInfo adRequest)
        {
            base.SetAdInfo(adRequest);
            isCollapsible = ((BannerAdRequest)adRequest).isCollapsible;
        }

#if admob_enabled
        public override void LoadAd()
        {
            if (waitingDelayReload) return;
            if (bannerAd != null)
            {
                DestroyAd();
            }
            isLoaded = false;
            AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            this.bannerAd = new BannerView(currentID, adaptiveSize, position switch { BannerPosition.Top=>AdPosition.Top,
                                                                               BannerPosition.Bottom=>AdPosition.Bottom,
                                                                               BannerPosition.Center=>AdPosition.Center,
                                                                               _=>AdPosition.Bottom }) ;

            RegisterBannerAdmobEventHandlers(this.bannerAd);
            AdRequest adRequest = new AdRequest();
            if (IsCollapsible)
            {
                adRequest.Extras.Add("collapsible", position switch
                {
                    BannerPosition.Top => "top",
                    _ => "bottom"
                });
            }

            // Load the banner with the request.
            this.bannerAd.LoadAd(adRequest);
            bannerAd.Hide();
            isShowing = false;
            Debug.Log($"{ADMOB_LABEL} loading: {name}, id:{currentID}");
        }
        public override void LoadAndShowAd()
        {
            if (waitingDelayReload) return;
            if (bannerAd != null)
            {
                DestroyAd();
            }
            isLoaded = false;
            AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            this.bannerAd = new BannerView(currentID, adaptiveSize, position switch { BannerPosition.Top=>AdPosition.Top,
                                                                               BannerPosition.Bottom=>AdPosition.Bottom,
                                                                               BannerPosition.Center=>AdPosition.Center,
                                                                               _=>AdPosition.Bottom }) ;

            RegisterBannerAdmobEventHandlers(this.bannerAd);
            AdRequest adRequest = new AdRequest();
            if (IsCollapsible)
            {
                adRequest.Extras.Add("collapsible", position switch
                {
                    BannerPosition.Top => "top",
                    _ => "bottom"
                });
            }

            // Load the banner with the request.
            this.bannerAd.LoadAd(adRequest);
            isShowing = true;
            Debug.Log($"{ADMOB_LABEL} loading: {name}, id:{currentID}");
        }

        private void RegisterBannerAdmobEventHandlers(BannerView _bannerView)
        {
            // Raised when an ad is loaded into the banner view.
            _bannerView.OnBannerAdLoaded += () =>
            {
                this.LoadAdComplete();
                Debug.Log($"{ADMOB_LABEL} loaded {name} with response : {_bannerView.GetResponseInfo()}");
                isLoaded = true;
                OnAdLoaded?.Invoke(this);
                retryAttempt = 0;
            };
            // Raised when an ad fails to load into the banner view.
            _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
            {
                this.LoadAdComplete();
                Debug.LogError($"{ADMOB_LABEL} failed to load {name} with error : {error}");
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
            // Raised when the ad is estimated to have earned money.
            _bannerView.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format(ADMOB_LABEL+"paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
                string adapter = "Admob";
                try
                {
                    adapter = _bannerView.GetResponseInfo().GetLoadedAdapterResponseInfo().AdSourceName;
                }
                catch (Exception)
                {
                    Debug.LogWarning("DKTech SDK: Can't get Ad Network");
                }
                FirebaseManager.SendRevAdjust(adValue, adapter);
            };
            // Raised when an impression is recorded for an ad.
            _bannerView.OnAdImpressionRecorded += () =>
            {
                Debug.Log(ADMOB_LABEL+"recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            _bannerView.OnAdClicked += () =>
            {
                Debug.Log(ADMOB_LABEL+"was clicked.");
                IsAdClicked = true;
            };
            // Raised when an ad opened full screen content.
            _bannerView.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log(ADMOB_LABEL+"full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            _bannerView.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log(ADMOB_LABEL+"full screen content closed.");
            };
        }
        public override void HideAd()
        {
            if (bannerAd != null && isShowing)
            {
                if (autoReload)
                {
                    if (IsCollapsible) this.RequestLoadAd();
                    else bannerAd.Hide();
                }
                else
                {
                    bannerAd.Destroy();
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
                    bannerAd.Hide();
                    isShowing = false;
                }
                bannerAd.Destroy();
            }
        }

        public override void ShowAd()
        {
            if (IsAvailable())
            {
                bannerAd.Show();
                isShowing = true;
            }
        }

        public override bool IsAvailable()
        {
            return bannerAd != null && isLoaded && !AdsUtilities.NoAds;
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