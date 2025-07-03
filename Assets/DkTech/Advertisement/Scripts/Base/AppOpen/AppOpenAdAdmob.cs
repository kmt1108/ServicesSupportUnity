#if admob_enabled
using Dktech.Services.Firebase;
using GoogleMobileAds.Api;
#endif
using System;
using UnityEngine;

namespace Dktech.Services.Advertisement
{
    public class AppOpenAdAdmob : AppOpenAd
    {
#if admob_enabled
        GoogleMobileAds.Api.AppOpenAd appOpenAd;
#endif
        private DateTime _expireTime;
#if admob_enabled
        public override void DestroyAd()
        {
            if (appOpenAd != null)
            {
                appOpenAd.Destroy();
                appOpenAd = null;
            }
        }

        public override void LoadAd()
        {
            if (IsAvailable())
            {
                DestroyAd();
            }
            var adRequest = new AdRequest();
            // Load an app open ad for portrait orientation
            GoogleMobileAds.Api.AppOpenAd.Load(currentID, adRequest, ((appOpenAd, error) =>
            {
                if (error != null)
                {
                    this.LoadAdComplete();
                    // Handle the error.
                    Debug.LogError(ADMOB_LABEL + "failed to load an ad " +
                                       "with error : " + error);
                    CheckSwitchID();
                    retryAttempt++;
                    double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
                    DelayLoadAd((int)retryDelay*1000);
                    return;
                }
                // App open ad is loaded.
                this.LoadAdComplete();
                Debug.Log(ADMOB_LABEL + "loaded with response : " + appOpenAd.GetResponseInfo());
                this.appOpenAd = appOpenAd;
                RegisterAOAAdmobEventHandlers(appOpenAd);
                _expireTime = DateTime.Now + TimeSpan.FromHours(4);
                OnAdLoaded?.Invoke(this);
                retryAttempt = 0;
            }));
            Debug.Log(ADMOB_LABEL + "loading: " + name + ", id:" + id);

        }
        private void RegisterAOAAdmobEventHandlers(GoogleMobileAds.Api.AppOpenAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format(ADMOB_LABEL + "paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
                string adapter = "Admob";
                try
                {
                    adapter = ad.GetResponseInfo().GetLoadedAdapterResponseInfo().AdSourceName;
                }
                catch (Exception)
                {
                    Debug.LogWarning("DKTech SDK: Can't get Ad Network");
                }
                FirebaseManager.SendRevAdjust(adValue, adapter);
                FirebaseManager.SendRevFacebook(adValue);
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log(ADMOB_LABEL + "recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log(ADMOB_LABEL + "was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log(ADMOB_LABEL + "full screen content opened.");
                OnAdShow?.Invoke();
                IsShowing = true;
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log(ADMOB_LABEL + "full screen content closed.");
                OnAdClosed?.Invoke();
                appOpenAd = null;
                if (autoReload) this.RequestLoadAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError(ADMOB_LABEL + "failed to open full screen content with error : " + error);
                // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
                OnAdShowFailed?.Invoke();
                appOpenAd = null;
                if (autoReload) this.RequestLoadAd();
            };
        }
        public override void ShowAd()
        {
            if (IsAvailable())
            {
                OnStartShowAd?.Invoke();
                appOpenAd.Show();
                IsShowing = true;
                FirebaseManager.TrackEvent(name+"_ADS");
            }
        }

        public override bool IsAvailable()
        {
            return appOpenAd?.CanShowAd() == true && DateTime.Now < _expireTime && !AdsUtilities.NoAds;
        }
#else
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

#endif
    }
}