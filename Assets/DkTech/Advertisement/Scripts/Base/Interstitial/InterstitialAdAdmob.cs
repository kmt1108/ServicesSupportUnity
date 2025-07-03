#if admob_enabled
using Dktech.Services.Firebase;
using GoogleMobileAds.Api;
#endif
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Dktech.Services.Advertisement
{
    public class InterstitialAdAdmob : InterstitialAd
    {
#if admob_enabled
        GoogleMobileAds.Api.InterstitialAd interstitialAd;
#endif

#if admob_enabled
        public override void DestroyAd()
        {
            if (interstitialAd != null)
            {
                interstitialAd.Destroy();
                interstitialAd = null;
            }
        }

        public override void LoadAd()
        {
            // Clean up the old ad before loading a new one.
            DestroyAd();
            var adRequest = new AdRequest();
            // send the request to load the ad.
            GoogleMobileAds.Api.InterstitialAd.Load(currentID, adRequest,
                (GoogleMobileAds.Api.InterstitialAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        this.LoadAdComplete();
                        Debug.LogError($"{ADMOB_LABEL} failed to load {name} with error : {error}");
                        if (HaveInternet) FirebaseManager.TrackEvent("INTERSTITIAL_ADS_LOAD_FAIL");
                        CheckSwitchID();
                        retryAttempt++;
                        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
                        DelayLoadAd((int)retryDelay*1000);
                        return;
                    }
                    // App intersitial ad is loaded.
                    this.LoadAdComplete();
                    Debug.Log($"{ADMOB_LABEL} loaded {name} with response : {ad.GetResponseInfo()}");
                    interstitialAd = ad;
                    RegisterIntersAdmobEventHandlers(interstitialAd);
                    OnAdLoaded?.Invoke(this);
                    retryAttempt = 0;
                });
            Debug.Log($"{ADMOB_LABEL} loading: {name}, id:{currentID}");
        }
        private void RegisterIntersAdmobEventHandlers(GoogleMobileAds.Api.InterstitialAd ad)
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
                OnAdShowed?.Invoke();
                IsShowing = true;
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log(ADMOB_LABEL + "full screen content closed.");
                OnAdClosed?.Invoke();
                if (autoReload) this.RequestLoadAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError(ADMOB_LABEL+"failed to open full screen content with error : " + error);
                OnAdShowFailed?.Invoke();
                if (autoReload) this.RequestLoadAd();
                IsShowing = false;
            };
        }

        public override void ShowAd()
        {
            if (IsAvailable())
            {
                OnStartShowAd?.Invoke();
                interstitialAd.Show();
                IsShowing = true;
                FirebaseManager.TrackEvent("INTERSTITIAL_ADS");
            }else OnAdShowFailed?.Invoke();
        }

        public override bool IsAvailable()
        {
            return !InterstitialAdManager.WaitingForInter && interstitialAd?.CanShowAd() == true && !AdsUtilities.NoAds;
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