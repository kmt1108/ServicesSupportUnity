using Dktech.Services.Firebase;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Dktech.Services.Advertisement
{
    public class InterstitialAdApplovin : InterstitialAd
    {
#if applovin_enabled
        public override void LoadAd()
        {
            // Load the first interstitial
            MaxSdk.LoadInterstitial(currentID);
            Debug.Log($"{APPLOVIN_LABEL} loading: {name}, id:{currentID}");
        }
        //Applovin Interstitials callbacks
        internal void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'
            this.LoadAdComplete();
            Debug.Log($"{APPLOVIN_LABEL} loaded {name} with response : {adInfo}");
            OnAdLoaded?.Invoke(this);
            // Reset retry attempt
            retryAttempt = 0;
        }
        internal void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Interstitial ad failed to load
            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)
            // GUIUtility.systemCopyBuffer = "code: " + errorInfo.Code + " message: " + errorInfo.Message;
            this.LoadAdComplete();
            Debug.LogError($"{APPLOVIN_LABEL} failed to load {name} with error : {errorInfo}");
            if (HaveInternet) FirebaseManager.TrackEvent("INTERSTITIAL_ADS_LOAD_FAIL");
            CheckSwitchID();
            retryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
            DelayLoadAd((int)retryDelay*1000);
        }
        internal void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log(APPLOVIN_LABEL + adUnitId + "full screen content opened.");
            IsShowing = true;
            OnAdShowed?.Invoke();
        }
        internal void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
            Debug.LogError(APPLOVIN_LABEL + "failed to open full screen content with error : " + errorInfo);
            OnAdShowFailed?.Invoke();
            if (autoReload) this.RequestLoadAd();
            IsShowing = false;
        }
        internal void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log(APPLOVIN_LABEL + "was clicked.");
        }
        internal void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log(APPLOVIN_LABEL + "full screen content closed.");
            OnAdClosed?.Invoke();
            if (autoReload) this.RequestLoadAd();
        }
        internal void OnInterAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {

            Debug.Log(String.Format(APPLOVIN_LABEL + "paid {0}.",
                    adInfo.Revenue));
            FirebaseManager.SendRevFirebase(adInfo);
            FirebaseManager.SendRevAdjust(adInfo);
        }

        public override void ShowAd()
        {
            if (IsAvailable())
            {
                IsShowing = true;
                OnStartShowAd?.Invoke();
                MaxSdk.ShowInterstitial(id);
                FirebaseManager.TrackEvent("INTERSTITIAL_ADS");
            }
            else OnAdShowFailed?.Invoke();
        }
        public override void DestroyAd()
        {
            throw new NotImplementedException();
        }

        public override bool IsAvailable()
        {
            return !InterstitialAdManager.WaitingForInter && MaxSdk.IsInterstitialReady(id) && !AdsUtilities.NoAds;
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