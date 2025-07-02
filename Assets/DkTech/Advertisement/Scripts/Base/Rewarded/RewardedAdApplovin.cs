using Dktech.Services.Firebase;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Dktech.Services.Advertisement
{
    public class RewardedAdApplovin : RewardedAd
    {
#if applovin_enabled
        public override void DestroyAd()
        {
            throw new NotImplementedException();
        }

        public override void LoadAd()
        {
            // Load the first rewarded ad
            MaxSdk.LoadRewardedAd(currentID);
            Debug.Log($"{APPLOVIN_LABEL} loading: {name}, id:{currentID}");

        }

        internal void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.
            this.LoadAdComplete();
            Debug.Log($"{APPLOVIN_LABEL} loaded {name} with response : {adInfo}");
            OnAdLoaded?.Invoke(this);
            // Reset retry attempt
            retryAttempt = 0;
        }

        internal void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Rewarded ad failed to load
            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).
            this.LoadAdComplete();
            Debug.LogError($"{APPLOVIN_LABEL} failed to load {name} with error : {errorInfo}");
            if (HaveInternet) FirebaseManager.TrackEvent("REWARDED_ADS_LOAD_FAIL");
            CheckSwitchID();
            retryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
            DelayLoadAd((int)retryDelay * 1000);
        }

        internal void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log(APPLOVIN_LABEL + adUnitId + "full screen content opened.");
            IsShowing = true;
        }

        internal void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
            Debug.LogError(APPLOVIN_LABEL + "failed to open full screen content with error : " + errorInfo);
            IsShowing = false;
            if (autoReload) this.RequestLoadAd();
        }

        internal void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log(APPLOVIN_LABEL + "was clicked.");

        }

        internal void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log(APPLOVIN_LABEL + "full screen content closed.");
            rewardedAction?.Invoke(rewardEarned);
            rewardedAction = null;
            if (autoReload) this.RequestLoadAd();
            OnAdClosed?.Invoke();
        }

        internal void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log(APPLOVIN_LABEL + "receive rewarded");

            // The rewarded ad displayed and the user should receive the reward.
            rewardEarned = true;
        }

        internal void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log(String.Format(APPLOVIN_LABEL + "paid {0}.",
                    adInfo.Revenue));
            FirebaseManager.SendRevFirebase(adInfo);
            FirebaseManager.SendRevAdjust(adInfo);
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
                rewardEarned = false;
                IsShowing = true;
                MaxSdk.ShowRewardedAd(id);
                FirebaseManager.TrackEvent("VIDEO_REWARD");
            }
            else
            {
                Toast.instance.ShowToastUI("No ads avaiable!");
                if (rewardedAction != null)
                {
                    rewardedAction?.Invoke(false);
                    rewardedAction = null;
                }
            }
        }

        public override bool IsAvailable()
        {
            return MaxSdk.IsRewardedAdReady(id);
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