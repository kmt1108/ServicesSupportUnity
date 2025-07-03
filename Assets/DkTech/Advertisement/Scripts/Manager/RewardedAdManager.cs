using Dktech.Services.Firebase;
#if firebase_enabled
using Firebase.RemoteConfig;
#endif
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dktech.Services.Advertisement
{
    public static class RewardedAdManager
    {
        private static readonly List<RewardedAd> listRewardedAd=new();
        public static void Initialize()
        {
            RegisterAdListener();
#if applovin_enabled
            RegisterRewardedApplovinEventHandlers();
#endif
        }
        private static void RegisterAdListener()
        {
            RewardedAd.OnAdClosed += DelayHiddenAd;
        }
        public static void RequestAd(List<AdRequestInfo> listAdRequest)
        {
            for (int i=listAdRequest.Count-1;i>=0;i--)
            {
#if firebase_enabled
                if (FirebaseManager.FetchState == RemoteConfigFetchState.Success)
                {
                    string[] request = FirebaseRemoteConfig.DefaultInstance.GetValue(listAdRequest[i].adRCKey != "" ? listAdRequest[i].adRCKey : listAdRequest[i].adName).StringValue.Split(",");
                    if (request?.Length >= 3)
                    {
                        listAdRequest[i].UpdateInfo(request);
                    }
                }
#endif
                if (AdsUtilities.IsTestMode) listAdRequest[i].CreateRewardedTestAd();
                switch (listAdRequest[i].adNetwork)
                {
#if admob_enabled
                    case AdNetwork.Admob:
                        RewardedAdAdmob rewardAdmob = new();
                        listRewardedAd.Add(rewardAdmob);
                        rewardAdmob.Setup(listAdRequest[i]);
                        break;
#endif
#if applovin_enabled
                    case AdNetwork.Applovin:
                        RewardedAdApplovin rewardAlv = new();
                        listRewardedAd.Add(rewardAlv);
                        rewardAlv.Setup(listAdRequest[i]);
                        break;
#endif
#if ironsource_enabled
                    case AdNetwork.IronSource:
                        RewardedAdIronSource rewardIrs = new();
                        listRewardedAd.Add(rewardIrs);
                        rewardIrs.Setup(listAdRequest[i]);
                        break;
#endif
                    default:
                        listAdRequest.RemoveAt(i);
                        break;
                }
            }
        }
        #region Applovin Methods
#if applovin_enabled
        private static bool registerApplovinEventHandlers;
        private static void RegisterRewardedApplovinEventHandlers()
        {
            if (!registerApplovinEventHandlers)
            {
                registerApplovinEventHandlers = true;
                // Attach callback
                MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
                MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
                MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
                MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
                MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
                MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
                MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
                MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
            }

        }

        //Applovin Rewarded callbacks

        private static void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) => FindApplovinAd(adUnitId).OnRewardedAdLoadedEvent(adUnitId, adInfo);

        private static void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) => FindApplovinAd(adUnitId).OnRewardedAdLoadFailedEvent(adUnitId, errorInfo);

        private static void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        => FindApplovinAd(adUnitId).OnRewardedAdDisplayedEvent(adUnitId, adInfo);

        private static void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        => FindApplovinAd(adUnitId).OnRewardedAdFailedToDisplayEvent(adUnitId, errorInfo, adInfo);

        private static void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) => FindApplovinAd(adUnitId).OnRewardedAdClickedEvent(adUnitId, adInfo);

        private static void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) => FindApplovinAd(adUnitId).OnRewardedAdHiddenEvent(adUnitId, adInfo);

        private static void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo) => FindApplovinAd(adUnitId).OnRewardedAdReceivedRewardEvent(adUnitId, reward, adInfo);

        private static void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) => FindApplovinAd(adUnitId).OnRewardedAdRevenuePaidEvent(adUnitId, adInfo);
        private static RewardedAdApplovin FindApplovinAd(string adUnitId)
        {
            RewardedAd ad = listRewardedAd.Find(ad => ad.AdNetwork == AdNetwork.Applovin && ad.AdID == adUnitId);
            return ad as RewardedAdApplovin;
        }

        #endif
        #endregion
        public static void ShowRewarded(string adName, Action<bool> action)
        {
            if (AdsUtilities.FreeReward)
            {
                action.Invoke(true);
                return;
            }
            RewardedAd rewardedAd = listRewardedAd.Find(rw => rw.Name == adName);
            rewardedAd?.ShowAd(action);
        }

        private static int hiddenCount;
        private static void DelayHiddenAd()
        {
            hiddenCount++;
            WaitingHiddenAd(2000);
        }
        private static async void WaitingHiddenAd(int milisecond)
        {
            await Task.Delay(milisecond);
            hiddenCount--;
            if (hiddenCount == 0) RewardedAd.IsShowing = false;
        }
        public static bool Contain(string name)
        {
            return listRewardedAd.Count > 0 && listRewardedAd.Find(x => x.Name == name) != null;
        }

    }
}