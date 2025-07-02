using Dktech.Services.Firebase;
#if firebase_enabled
using Firebase.RemoteConfig;
#endif
using System.Collections.Generic;

namespace Dktech.Services.Advertisement
{
    public static class NativeOverlayAdManager
    {
        private static readonly List<NativeOverlayAd> listNativeOverlayAd=new();
        public static void RequestAd(List<NativeOverlaysAdRequest> listAdRequest)
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
                if (AdsUtilities.IsTestMode) listAdRequest[i].CreateNativeVideoTestAd();
                switch (listAdRequest[i].adNetwork)
                {
#if admob_enabled
                    case AdNetwork.Admob:
                        NativeOverlayAdAdmob nativeAdmob = new();
                        listNativeOverlayAd.Add(nativeAdmob);
                        nativeAdmob.Setup(listAdRequest[i]);
                        break;
                    #endif
                    default:
                        listAdRequest.RemoveAt(i);
                        break;
                }
            }
        }
        public static void ShowNativeOverlay(string name)
        {
            NativeOverlayAd nativeOverlayAd = listNativeOverlayAd.Find(native => native.Name == name);
            nativeOverlayAd?.ShowAd();
        }
        public static bool Contain(string name)
        {
            return listNativeOverlayAd.Count > 0 && listNativeOverlayAd.Find(x => x.Name == name) != null;
        }
    }
}