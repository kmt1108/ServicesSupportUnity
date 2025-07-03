#if ironsource_enabled
using com.unity3d.mediation;
using Dktech.Services.Firebase;
#endif
using System;
using UnityEngine;

namespace Dktech.Services.Advertisement
{
    public class RewardedAdIronSource : RewardedAd
    {
#if ironsource_enabled
        LevelPlayRewardedAd rewardedAd;
        bool adClosed;
#endif

#if ironsource_enabled
        public override void DestroyAd()
        {
            if (rewardedAd != null)
            {
                rewardedAd.DestroyAd();
                rewardedAd = null;
            }
        }

        public override void LoadAd()
        {
            // Clean up the old ad before loading a new one.
            DestroyAd();
            rewardedAd = new LevelPlayRewardedAd(currentID);
            RegisterRewardedIronSourceEventHandlers();
            // send the request to load the ad.
            rewardedAd.LoadAd();
            Debug.Log($"{IRONSOURCE_LABEL} loading: {name}, id:{currentID}");
            
        }
        private void RegisterRewardedIronSourceEventHandlers()
        {
            // Raised when the ad is estimated to have earned money.
            rewardedAd.OnAdLoaded += (info) =>
            {
                Debug.Log($"{IRONSOURCE_LABEL} loaded {name} with response : {info}");
                OnAdLoaded?.Invoke(this);
            };
            // Raised when an impression is recorded for an ad.
            rewardedAd.OnAdLoadFailed += (error) =>
            {
                Debug.LogError($"{IRONSOURCE_LABEL} failed to load {name} with error : {error}");
                if (HaveInternet) FirebaseManager.TrackEvent("REWARDED_ADS_LOAD_FAIL");
                CheckSwitchID();
                retryAttempt++;
                double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
                DelayLoadAd((int)retryDelay * 1000);
            };
            // Raised when a click is recorded for an ad.
            rewardedAd.OnAdClicked += (info) =>
            {
                Debug.Log(IRONSOURCE_LABEL + "was clicked.");
            };
            // Raised when an ad opened full screen content.
            rewardedAd.OnAdDisplayed += (info) =>
            {
                Debug.Log(IRONSOURCE_LABEL + "full screen content opened.");
                IsShowing = true;
            };
            // Raised when the ad closed full screen content.
            rewardedAd.OnAdClosed += (info) =>
            {
                Debug.Log(IRONSOURCE_LABEL + "full screen content closed.");
                if (rewardEarned)
                {
                    rewardedAction?.Invoke(true);
                    rewardedAction = null;
                }
                else
                {
                    adClosed = true;
                }
                OnAdClosed?.Invoke();
                if (autoReload) LoadAd();
            };
            // Raised when the ad eared rewarded
            rewardedAd.OnAdRewarded += (info,reward) =>
            {
                Debug.Log(IRONSOURCE_LABEL + "receive rewarded.");
                if (adClosed)
                {
                    rewardedAction?.Invoke(true);
                    rewardedAction = null;
                }
                else
                {
                    rewardEarned = true;
                }
            };
            // Raised when the ad failed to open full screen content.
            rewardedAd.OnAdDisplayFailed += (error) =>
            {
                Debug.LogError(IRONSOURCE_LABEL + "failed to open full screen content with error : " + error);
                IsShowing = false;
                if (rewardedAction != null)
                {
                    rewardedAction?.Invoke(false);
                    rewardedAction = null;
                }
                if (autoReload) LoadAd();
            };
        }

        public override void ShowAd(Action<bool> rewardedAction)
        {
            RewardedAd.rewardedAction = rewardedAction;
            adClosed = false;
            rewardEarned = false;
            ShowAd();
        }

        public override void ShowAd()
        {
            if (IsAvailable())
            {
                IsShowing = true;
                rewardedAd.ShowAd();
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
            return rewardedAd.IsAdReady();
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