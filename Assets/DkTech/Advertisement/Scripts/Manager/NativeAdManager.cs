using Dktech.Services.Firebase;
using Dktech.Services;
#if firebase_enabled
using Firebase.RemoteConfig;
#endif
using System;
using System.Collections.Generic;

namespace Dktech.Services.Advertisement
{
    public static class NativeAdManager
    {
        private static readonly List<NativeAd> listNativeAd=new();
        public static void Initialize()
        {
            RegisterAdListener();
        }
        private static void RegisterAdListener()
        {
            ServicesManager.OnApplicationPauseEvent += (paused) =>
            {
                OnApplicationPause(paused);
            };
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
                if (AdsUtilities.IsTestMode) listAdRequest[i].CreateNativeTestAd();
                switch (listAdRequest[i].adNetwork)
                {
#if admob_enabled
                    case AdNetwork.Admob:
                        NativeAdAdmob nativeAdmob = new();
                        listNativeAd.Add(nativeAdmob);
                        nativeAdmob.Setup(listAdRequest[i]);
                        break;
#endif
                    default:
                        listAdRequest.RemoveAt(i);
                        break;

                }
            }
        }
        public static void ShowNative(string name, NativeAdContent content, Action actShowed = null, Action<bool> actClosed = null)
        {
            NativeAd nativeAd = listNativeAd.Find(native => native.Name == name);
            if (nativeAd != null)
            {
                if (nativeAd.IsShowing)
                {
                    nativeAd.HideAd();
                    if (nativeAd.AutoReload) nativeAd.RequestLoadAd();
                }
                nativeAd.ShowAd(content, actShowed, actClosed);
            }
            else
            {
                actClosed?.Invoke(false);
            }
        }
        public static void HideNative(string name)
        {
            NativeAd nativeAd = listNativeAd.Find(native=>native.Name == name);
            if (nativeAd?.IsShowing == true)
            {
                nativeAd.HideAd();
                if (nativeAd.AutoReload) nativeAd.RequestLoadAd();
            }
        }
        private static void OnApplicationPause(bool pause)
        {
            if (!pause && listNativeAd?.Count > 0)
            {
                if (InterstitialAd.IsShowing || RewardedAd.IsShowing || AppOpenAd.IsShowing || Rating.OutFromRate)
                    return;
                listNativeAd.FindAll(x => x.IsShowing).ForEach(x => x.LoadAd());
            }
        }
        public static bool Contain(string name)
        {
            return listNativeAd?.Count > 0 && listNativeAd.Find(x => x.Name == name) != null;
        }
    }
}