using Dktech.Services.Firebase;
using Dktech.Services;
#if firebase_enabled
using Firebase.RemoteConfig;
#endif
using System.Collections.Generic;

namespace Dktech.Services.Advertisement
{
    public static class BannerAdManager
    {
        private static BannerAd CurrentBanner { get; set; }
        private static readonly List<BannerAd> listBannerAd= new();
        public static void Initialize()
        {
            RegisterAdListener();
#if applovin_enabled
            RegisterBannerApplovinEventHandlers();
#endif
        }
        private static void RegisterAdListener()
        {
            ServicesManager.OnApplicationPauseEvent += (paused) =>
            {
                OnApplicationPause(paused);
            };
        }
        public static void RequestAd(List<BannerAdRequest> listAdRequest)
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
                if (AdsUtilities.IsTestMode) listAdRequest[i].CreateBannerTestAd();
                switch (listAdRequest[i].adNetwork)
                {
#if admob_enabled
                    case AdNetwork.Admob:
                        BannerAdAdmob bannerAdmob = new();
                        listBannerAd.Add(bannerAdmob);
                        bannerAdmob.Setup(listAdRequest[i]);
                        break;
#endif
#if applovin_enabled
                    case AdNetwork.Applovin:
                        BannerAdApplovin bannerAlv = new();
                        listBannerAd.Add(bannerAlv);
                        bannerAlv.Setup(listAdRequest[i]);
                        break;
#endif
#if ironsource_enabled
                    case AdNetwork.IronSource:
                        BannerAdIronSource bannerIrs = new();
                        listBannerAd.Add(bannerIrs);
                        bannerIrs.Setup(listAdRequest[i]);
                        break;
#endif
                    default:
                        listAdRequest.RemoveAt(i);
                        break;
                }

            }
        }
        public static void ShowBanner(string adName)
        {
            if (CurrentBanner != null && CurrentBanner.Name == adName && CurrentBanner.IsShowing) return;
            BannerAd bannerAd = listBannerAd.Find(bn => bn.Name == adName);
            if (bannerAd?.IsLoaded == true || bannerAd?.IsCollapsible == false)
            {
                HideCurrentBanner();
                CurrentBanner = bannerAd;
                CurrentBanner.ShowAd();
            }
        }
        public static void HideCurrentBanner()
        {
            if (CurrentBanner?.IsShowing == true)
            {
                CurrentBanner.HideAd();
                CurrentBanner = null;
            }
        }
        public static void LoadAndShowBanner(string adName)
        {
            BannerAd bannerAd = listBannerAd.Find(bn => bn.Name == adName);
            if (bannerAd !=null)
            {
                if (CurrentBanner != bannerAd)
                {
                    HideCurrentBanner();
                }
                CurrentBanner = bannerAd;
                CurrentBanner.LoadAndShowAd();
            }
        }
        private static void OnApplicationPause(bool pause)
        {
#if admob_enabled
            if (!pause && CurrentBanner != null && CurrentBanner.AdNetwork == AdNetwork.Admob && CurrentBanner.IsCollapsible && CurrentBanner.IsShowing && CurrentBanner.IsLoaded)
            {
                if (InterstitialAd.IsShowing || RewardedAd.IsShowing || AppOpenAd.IsShowing || Rating.OutFromRate)
                    return;
                CurrentBanner.LoadAndShowAd();
            }
#endif
        }
        public static bool Contain(string name)
        {
            return listBannerAd.Count > 0 && listBannerAd.Find(x => x.Name == name) != null;
        }
        public static bool CheckAdReadyToShow(string name)
        {
            BannerAd bannerAd = listBannerAd.Find(bn => bn.Name == name);
            return bannerAd?.IsLoaded==true;
        }
        #region Applovin Methods
#if applovin_enabled
        private static bool registerApplovinEventHandlers;
        private static void RegisterBannerApplovinEventHandlers()
        {
            if (!registerApplovinEventHandlers)
            {
                registerApplovinEventHandlers = true;
                // Attach callback
                MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
                MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
                MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
                MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
                MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
                MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;
            }

        }
        private static void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) => FindApplovinAd(adUnitId).OnBannerAdLoadedEvent(adUnitId, adInfo);
        private static void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) => FindApplovinAd(adUnitId).OnBannerAdLoadFailedEvent(adUnitId, errorInfo);
        private static void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) => FindApplovinAd(adUnitId).OnBannerAdClickedEvent(adUnitId, adInfo);
        private static void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) => FindApplovinAd (adUnitId).OnBannerAdRevenuePaidEvent(adUnitId, adInfo);
        private static void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) => FindApplovinAd(adUnitId).OnBannerAdExpandedEvent(adUnitId, adInfo);
        private static void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) => FindApplovinAd(adUnitId).OnBannerAdCollapsedEvent(adUnitId, adInfo);
        private static BannerAdApplovin FindApplovinAd(string adUnitId)
        {
            BannerAd ad = listBannerAd.Find(ad => ad.AdNetwork == AdNetwork.Applovin && ad.AdID == adUnitId);
            return (BannerAdApplovin)ad;
        }
#endif
        #endregion
    }
}