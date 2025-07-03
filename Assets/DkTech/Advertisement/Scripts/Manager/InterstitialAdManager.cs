using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using Dktech.Services.Firebase;
#if firebase_enabled
using Firebase.RemoteConfig;
#endif

namespace Dktech.Services.Advertisement
{
    public static class InterstitialAdManager
    {
        public static bool WaitingForInter { get; set; }
        public static int TimeWaitingInter { get; set; } = 30;
        private static readonly List<InterstitialAd> listInterAd = new();
        private static Action<bool> actionClosed;
        private static Action actionShowed;
        public static void Initialize()
        {
            RegisterAdListener();
#if applovin_enabled
            RegisterIntersApplovinEventHandlers();
#endif
        }
        private static void RegisterAdListener()
        {
            WaitingForInter = false;
            InterstitialAd.OnStartShowAd += ShowLoadingPanel;
            InterstitialAd.OnAdShowed += OnAdShowed;
            InterstitialAd.OnAdClosed += OnAdClosed;
            InterstitialAd.OnAdShowFailed += OnAdShowFailed;
        }
        public static void RequestAd(List<AdRequestInfo> listAdRequest)
        {
            for (int i = listAdRequest.Count - 1; i >= 0; i--)
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
                if (AdsUtilities.IsTestMode) listAdRequest[i].CreateInterstitialTestAd();
                switch (listAdRequest[i].adNetwork)
                {
#if admob_enabled
                    case AdNetwork.Admob:
                        InterstitialAdAdmob interAdmob = new();
                        listInterAd.Add(interAdmob);
                        interAdmob.Setup(listAdRequest[i]);
                        break;
#endif
#if applovin_enabled
                    case AdNetwork.Applovin:
                        InterstitialAdApplovin interAlv = new();
                        listInterAd.Add(interAlv);
                        interAlv.Setup(listAdRequest[i]);
                        break;
#endif
#if ironsource_enabled
                    case AdNetwork.IronSource:
                        InterstitialAdIronSource interIrs = new();
                        listInterAd.Add(interIrs);
                        interIrs.Setup(listAdRequest[i]);
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
        private static void RegisterIntersApplovinEventHandlers()
        {
            if (!registerApplovinEventHandlers)
            {
                registerApplovinEventHandlers = true;
                // Attach callback
                MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
                MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
                MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
                MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
                MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterAdRevenuePaidEvent;
                MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
                MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
            }

        }

        //Applovin Interstitials callbacks
        private static void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) => FindApplovinAd(adUnitId).OnInterstitialLoadedEvent(adUnitId, adInfo);
        private static void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) => FindApplovinAd(adUnitId).OnInterstitialLoadFailedEvent(adUnitId, errorInfo);
        private static void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) => FindApplovinAd(adUnitId).OnInterstitialDisplayedEvent(adUnitId, adInfo);
        private static void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo) => FindApplovinAd(adUnitId).OnInterstitialAdFailedToDisplayEvent(adUnitId, errorInfo, adInfo);
        private static void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) => FindApplovinAd(adUnitId).OnInterstitialClickedEvent(adUnitId, adInfo);
        private static void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) => FindApplovinAd(adUnitId).OnInterstitialHiddenEvent(adUnitId, adInfo);
        private static void OnInterAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) => FindApplovinAd(adUnitId).OnInterAdRevenuePaidEvent(adUnitId, adInfo);
        private static InterstitialAdApplovin FindApplovinAd(string adUnitId)
        {
            InterstitialAd ad = listInterAd.Find(ad => ad.AdNetwork == AdNetwork.Applovin && ad.AdID == adUnitId);
            return (InterstitialAdApplovin)ad;
        }
#endif
        #endregion
        public static void ShowInterstitial(string adName, Action actShowed = null, Action<bool> actClosed = null)
        {
            actionShowed = actShowed;
            actionClosed = actClosed;
            InterstitialAd interstitialAd = listInterAd.Find(it => it.Name == adName);
            if (interstitialAd != null)
            {
                interstitialAd.ShowAd();
            }
            else
            {
                if (actionClosed != null)
                {
                    actionClosed?.Invoke(false);
                    actionClosed = null;
                }
            }
        }
        public static void ShowInterstitialWithoutWaiting(string adName, Action actShowed = null, Action<bool> actClosed = null)
        {
            actionShowed = actShowed;
            actionClosed = actClosed;
            InterstitialAd interstitialAd = listInterAd.Find(it => it.Name == adName);
            if (interstitialAd != null)
            {
                SuspendWaitingForInter();
                interstitialAd.ShowAd();
            }
            else
            {
                if (actionClosed != null)
                {
                    actionClosed?.Invoke(false);
                    actionClosed = null;
                }
            }
        }
        private static void OnAdShowFailed()
        {
            HideLoadingPanel();
            if (actionClosed != null)
            {
                actionClosed?.Invoke(false);
                actionClosed = null;
            }
        }
        private static void OnAdShowed()
        {
            HideLoadingPanel();
            if (actionShowed != null)
            {
                actionShowed?.Invoke();
                actionShowed = null;
            }
        }
        private static void OnAdClosed()
        {
            HideLoadingPanel();
            if (actionClosed != null)
            {
                actionClosed?.Invoke(true);
                actionClosed = null;
            }
            RestartWaitForInter();
            DelayHiddenAd();
        }
        private static void ShowLoadingPanel()
        {
            AdsUtilities.ShowLoadingPanel(true);
        }
        private static void HideLoadingPanel()
        {
            AdsUtilities.ShowLoadingPanel(false);
        }
        private static CancellationTokenSource cancelWaiting;
        private static async void RestartWaitForInter()
        {
            try
            {
                cancelWaiting = new();
                await RestartingWaitForInter(cancelWaiting.Token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("DKTech SDK: RestartWaitForInter cancelled");
            }

        }
        private static async Task RestartingWaitForInter(CancellationToken cancellationToken)
        {
            WaitingForInter = true;
            await Task.Delay(TimeWaitingInter * 1000);
            cancellationToken.ThrowIfCancellationRequested();
            WaitingForInter = false;
        }
        private static void SuspendWaitingForInter()
        {
            cancelWaiting?.Cancel();
            WaitingForInter = false;
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
            if (hiddenCount == 0) InterstitialAd.IsShowing = false;
        }
        public static bool Contain(string name)
        {
            return listInterAd.Count > 0 && listInterAd.Find(x => x.Name == name) != null;
        }
    }
}