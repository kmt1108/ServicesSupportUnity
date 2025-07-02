using Dktech.Services.Firebase;
using Dktech.Services;
#if firebase_enabled
using Firebase.RemoteConfig;
#endif
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dktech.Services.Advertisement
{
    public static class AppOpenAdManager
    {
        private static readonly List<AppOpenAd> listAppOpenAd=new();
        public static void Initialize()
        {
            RegisterAdListner();
#if applovin_enabled
            RegisterAppOpenAdApplovinEventHandlers();
#endif
        }
        private static void RegisterAdListner()
        {
            AppOpenAd.OnStartShowAd += ShowLoadingPanel;
            AppOpenAd.OnAdShow += HideLoadingPanel;
            AppOpenAd.OnAdShowFailed += HideLoadingPanel;
            AppOpenAd.OnAdClosed += () =>
            {
                HideLoadingPanel();
                DelayHiddenAd();
            };
            ServicesManager.OnApplicationPauseEvent += (paused) =>
            {
                OnApplicationPause(paused);
            };

        }
        public static void RequestAd(List<AppOpenAdRequest> listAdRequest)
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
                if (AdsUtilities.IsTestMode) listAdRequest[i].CreateAppOpenTestAd();
                switch (listAdRequest[i].adNetwork)
                {
#if admob_enabled
                    case AdNetwork.Admob:
                        AppOpenAdAdmob aoaAdmob = new();
                        listAppOpenAd.Add(aoaAdmob);
                        aoaAdmob.Setup(listAdRequest[i]);
                        break;
#endif
#if applovin_enabled
                    case AdNetwork.Applovin:
                        AppOpenAdApplovin aoaAlv = new();
                        listAppOpenAd.Add(aoaAlv);
                        aoaAlv.Setup(listAdRequest[i]);
                        break;
#endif
                    default:
                        listAdRequest.RemoveAt(i);
                        break;
                }
            }
        }

#if applovin_enabled
        #region Applovin Methods
        private static bool registerApplovinEventHandlers;
        private static void RegisterAppOpenAdApplovinEventHandlers()
        {
            if (!registerApplovinEventHandlers)
            {
                registerApplovinEventHandlers = true;
                // Attach callback
                MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += OnAppOpenLoadedEvent;
                MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += OnAppOpenLoadFailedEvent;
                MaxSdkCallbacks.AppOpen.OnAdDisplayedEvent += OnAppOpenDisplayedEvent;
                MaxSdkCallbacks.AppOpen.OnAdDisplayFailedEvent += OnAppOpenFailedToDisplaydEvent;
                MaxSdkCallbacks.AppOpen.OnAdClickedEvent += OnAppOpenClickedEvent;
                MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenHiddenEvent;
                MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += OnAppOpenAdRevenuePaidEvent;
            }

        }

        //Applovin Interstitials callbacks
        private static void OnAppOpenLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) => FindApplovinAd(adUnitId).OnAppOpenLoadedEvent(adUnitId, adInfo);
        private static void OnAppOpenLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) => FindApplovinAd(adUnitId).OnAppOpenLoadFailedEvent(adUnitId, errorInfo);
        private static void OnAppOpenDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) => FindApplovinAd(adUnitId).OnAppOpenDisplayedEvent(adUnitId, adInfo);
        private static void OnAppOpenFailedToDisplaydEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo) => FindApplovinAd(adUnitId).OnAppOpenFailedToDisplaydEvent(adUnitId, errorInfo, adInfo);
        private static void OnAppOpenClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) => FindApplovinAd(adUnitId).OnAppOpenClickedEvent(adUnitId, adInfo);
        private static void OnAppOpenHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) => FindApplovinAd(adUnitId).OnAppOpenHiddenEvent(adUnitId, adInfo);
        private static void OnAppOpenAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) => FindApplovinAd(adUnitId).OnAppOpenAdRevenuePaidEvent(adUnitId, adInfo);
        private static AppOpenAdApplovin FindApplovinAd(string adUnitId)
        {
            AppOpenAd ad = listAppOpenAd.Find(ad => ad.AdNetwork == AdNetwork.Applovin && ad.AdID == adUnitId);
            return ad as AppOpenAdApplovin;
        }

        #endregion
#endif
        private static void ShowLoadingPanel()
        {
            AdsUtilities.ShowLoadingPanel(true);
        }
        private static void HideLoadingPanel()
        {
             AdsUtilities.ShowLoadingPanel(false);
        }
        private static bool checkingOnresume;
        private static CancellationTokenSource cancelCheckOnresume;
        private static async void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                if (checkingOnresume) cancelCheckOnresume?.Cancel();
                if ((DateTime.UtcNow - timeOutGame).TotalSeconds > 15 || !AdsUtilities.WaitingInBackgroundShowOnresume)
                {
                    cancelCheckOnresume = new();
                    await DelayCheckOnresume(cancelCheckOnresume.Token);
                }
            }
            else
            {
                timeOutGame = DateTime.UtcNow;
            }
        }
        private static DateTime timeOutGame;
        private static async Task DelayCheckOnresume(CancellationToken cancellationToken)
        {
            checkingOnresume = true;
            await Task.Delay(300);
            cancellationToken.ThrowIfCancellationRequested();
            if (BannerAd.IsAdClicked || NativeAd.IsAdClicked || NativeOverlayAd.IsAdClicked || Rating.OutFromRate)
            {
                if (BannerAd.IsAdClicked)
                {
                    BannerAd.IsAdClicked = false;
                }
                if (NativeAd.IsAdClicked)
                {
                    NativeAd.IsAdClicked = false;
                }
                if (NativeOverlayAd.IsAdClicked)
                {
                    NativeOverlayAd.IsAdClicked = false;
                }
                if (Rating.OutFromRate)
                {
                    Rating.OutFromRate = false;
                }
                checkingOnresume = false;
                return;
            }
            if (AppOpenAd.IsShowing || InterstitialAd.IsShowing || RewardedAd.IsShowing)
            {
                checkingOnresume = false;
                return;
            }
            ShowOnResumeAd();
            checkingOnresume = false;
        }
        public static void ShowAppOpenAd()
        {
            var appopen = listAppOpenAd.Find(aoa => aoa.Position == AppOpenPosition.AppOpen);
            appopen?.ShowAd();
        }
        public static void ShowOnResumeAd()
        {
            var onresume = listAppOpenAd.Find(aoa => aoa.Position == AppOpenPosition.OnResume);
            onresume?.ShowAd();
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
            if (hiddenCount == 0) AppOpenAd.IsShowing = false;
        }
        public static bool Contain(string name)
        {
            return listAppOpenAd.Count > 0 && listAppOpenAd.Find(x => x.Name == name) != null;
        }
    }
}