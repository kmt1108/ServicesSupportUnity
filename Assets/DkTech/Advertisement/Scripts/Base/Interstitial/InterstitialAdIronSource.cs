#if ironsource_enabled
using com.unity3d.mediation;
using Dktech.Services.Firebase;
#endif
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Dktech.Services.Advertisement
{
    public class InterstitialAdIronSource : InterstitialAd
    {
#if ironsource_enabled
        LevelPlayInterstitialAd interstitialAd;
#endif

#if ironsource_enabled
        public override void DestroyAd()
        {
            if (interstitialAd != null)
            {
                interstitialAd.DestroyAd();
                interstitialAd = null;
            }
        }

        public override void LoadAd()
        {
            // Clean up the old ad before loading a new one.
            DestroyAd();
            interstitialAd = new LevelPlayInterstitialAd(currentID);
            RegisterIntersIronSourceEventHandlers();
            interstitialAd.LoadAd();
            Debug.Log($"{IRONSOURCE_LABEL} loading: {name}, id:{currentID}");
        }
        private void RegisterIntersIronSourceEventHandlers()
        {
            // Raised when an ad is loaded into the banner view.
            interstitialAd.OnAdLoaded += (info) =>
            {
                Debug.Log($"{IRONSOURCE_LABEL} loaded {name} with response : {info}");
                OnAdLoaded?.Invoke(this);
                retryAttempt = 0;
            };
            // Raised when the ad is estimated to have earned money.
            interstitialAd.OnAdLoadFailed += (error) =>
            {
                Debug.LogError($"{IRONSOURCE_LABEL} failed to load {name} with error : {error}");
                if (HaveInternet) FirebaseManager.TrackEvent("INTERSTITIAL_ADS_LOAD_FAIL");
                CheckSwitchID();
                retryAttempt++;
                double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
                DelayLoadAd((int)retryDelay*1000);
            };
            // Raised when a click is recorded for an ad.
            interstitialAd.OnAdClicked += (info) =>
            {
                Debug.Log(IRONSOURCE_LABEL + "was clicked.");
            };
            // Raised when an ad opened full screen content.
            interstitialAd.OnAdDisplayed += (info) =>
            {
                Debug.Log(IRONSOURCE_LABEL + "full screen content opened.");
                IsShowing = true;
                OnAdShowed?.Invoke();
            };
            // Raised when the ad closed full screen content.
            interstitialAd.OnAdClosed += (info) =>
            {
                Debug.Log(IRONSOURCE_LABEL + "full screen content closed.");
                OnAdClosed?.Invoke();
                if(autoReload) LoadAd();
            };
            // Raised when the ad failed to open full screen content.
            interstitialAd.OnAdDisplayFailed += (error) =>
            {
                Debug.LogError(IRONSOURCE_LABEL+"failed to open full screen content with error : " + error);
                OnAdShowFailed?.Invoke();
                if(autoReload) LoadAd();
                IsShowing = false;
            };
        }

        public override void ShowAd()
        {
            if (IsAvailable())
            {
                OnStartShowAd?.Invoke();
                interstitialAd.ShowAd();
                IsShowing = true;
                FirebaseManager.TrackEvent("INTERSTITIAL_ADS");
            }else OnAdShowFailed?.Invoke();
        }

        public override bool IsAvailable()
        {
            return !InterstitialAdManager.WaitingForInter && interstitialAd.IsAdReady() && !AdsUtilities.NoAds;
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