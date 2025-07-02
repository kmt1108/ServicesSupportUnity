using Dktech.Services.Firebase;
using System;
using UnityEngine;

namespace Dktech.Services.Advertisement
{
    public class AppOpenAdApplovin : AppOpenAd
    {
#if applovin_enabled
        public override void LoadAd()
        {
            // Load the first App Open Ads
            MaxSdk.LoadAppOpenAd(currentID);
            Debug.Log($"{APPLOVIN_LABEL} loading: {name}, id:{currentID}");
        }

        internal void OnAppOpenLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            //Aoa loaded
            this.LoadAdComplete();
            Debug.Log($"{APPLOVIN_LABEL} loaded {name} with response : {adInfo}");
            OnAdLoaded?.Invoke(this);
            retryAttempt = 0;
        }
        internal void OnAppOpenLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            this.LoadAdComplete();
            Debug.LogError($"{APPLOVIN_LABEL} failed to load {name} with error : {errorInfo}");
            CheckSwitchID();
            retryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
            DelayLoadAd((int)retryDelay*1000);
        }
        internal void OnAppOpenDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo info)
        {
            Debug.Log(APPLOVIN_LABEL + adUnitId + "full screen content opened.");
            IsShowing = true;
            OnAdShow?.Invoke();
        }
        internal void OnAppOpenFailedToDisplaydEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            Debug.LogError(APPLOVIN_LABEL + "failed to open full screen content with error : " + errorInfo);
            IsShowing = false;
            OnAdShowFailed?.Invoke();
            if (autoReload) this.RequestLoadAd();
        }
        internal void OnAppOpenClickedEvent(string adUnitId, MaxSdkBase.AdInfo info)
        {
            Debug.Log(APPLOVIN_LABEL + "was clicked.");
        }
        public void OnAppOpenHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log(APPLOVIN_LABEL + "full screen content closed.");
            OnAdClosed?.Invoke();
            if (autoReload) this.RequestLoadAd();
        }
        internal void OnAppOpenAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
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
                MaxSdk.ShowAppOpenAd(id);
                FirebaseManager.TrackEvent(name+"_ADS");
            }
        }
        public override void DestroyAd()
        {
            throw new NotImplementedException();
        }

        public override bool IsAvailable()
        {
            return MaxSdk.IsAppOpenAdReady(id) && !AdsUtilities.NoAds;
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