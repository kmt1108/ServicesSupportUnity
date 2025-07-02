#if admob_enabled
using Dktech.Services.Firebase;
using GoogleMobileAds.Api;
#endif
using System;
using UnityEngine;

namespace Dktech.Services.Advertisement
{
    public class RewardedAdAdmob : RewardedAd
    {
#if admob_enabled
        GoogleMobileAds.Api.RewardedAd rewardedAd;
#endif

#if admob_enabled
        public override void DestroyAd()
        {
            if (rewardedAd != null)
            {
                rewardedAd.Destroy();
                rewardedAd = null;
            }
        }

        public override void LoadAd()
        {
            // Clean up the old ad before loading a new one.
            DestroyAd();
            var adRequest = new AdRequest();
            // send the request to load the ad.
            GoogleMobileAds.Api.RewardedAd.Load(currentID, adRequest,
                (GoogleMobileAds.Api.RewardedAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        this.LoadAdComplete();
                        Debug.LogError($"{ADMOB_LABEL} failed to load {name} with error : {error}");
                        if (HaveInternet) FirebaseManager.TrackEvent("REWARDED_ADS_LOAD_FAIL");
                        CheckSwitchID();
                        retryAttempt++;
                        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
                        DelayLoadAd((int)retryDelay * 1000);
                        return;
                    }
                    this.LoadAdComplete();
                    Debug.Log($"{ADMOB_LABEL} loaded {name} with response : {ad.GetResponseInfo()}");
                    rewardedAd = ad;
                    RegisterRewardedAdmobEventHandlers(rewardedAd);
                    OnAdLoaded?.Invoke(this);
                    retryAttempt = 0;
                });
            Debug.Log($"{ADMOB_LABEL} loading: {name}, id:{currentID}");

        }
        private void RegisterRewardedAdmobEventHandlers(GoogleMobileAds.Api.RewardedAd ad)
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
                Debug.LogError(ADMOB_LABEL + "failed to open full screen content with error : " + error);
                IsShowing = false;
                if (rewardedAction != null)
                {
                    rewardedAction?.Invoke(false);
                    rewardedAction = null;
                }
                if (autoReload) this.RequestLoadAd();
            };
        }

        public override void ShowAd(Action<bool> rewardedAction)
        {
            RewardedAd.rewardedAction = rewardedAction;
            ShowAd();
        }

        public override void ShowAd()
        {
            if (IsAvailable())
            {
                IsShowing = true;
                rewardedAd.Show((reward) =>
                {
                    if (rewardedAction != null)
                    {
                        rewardedAction?.Invoke(true);
                        rewardedAction = null;
                    }
                });
                FirebaseManager.TrackEvent("VIDEO_REWARD");
            }
            else
            {
                Toast.ShowToastUI("No ads avaiable!");
                if (rewardedAction != null)
                {
                    rewardedAction?.Invoke(false);
                    rewardedAction = null;
                }
            }
        }

        public override bool IsAvailable()
        {
            return rewardedAd?.CanShowAd() == true;
        }
#else
        public override void ShowAd(Action<bool> rewardedAction)
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

#endif
    }
}