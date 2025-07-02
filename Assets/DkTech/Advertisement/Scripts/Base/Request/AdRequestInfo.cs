using System;
using UnityEngine;
namespace Dktech.Services.Advertisement
{
    [Serializable]
    public class AdRequestInfo
    {
        [SerializeField] internal string adName;
        [SerializeField] internal string adRCKey;
        [SerializeField] internal AdNetwork adNetwork;
        [SerializeField] internal string adID;
        [SerializeField] internal string adID2;
        [SerializeField] internal bool autoReload;
        public string Name => adName;
        public AdRequestInfo(string adName, string adRCKey, AdNetwork adNetwork, string adID, string adID2, bool autoReload)
        {
            this.adName = adName;
            this.adRCKey = adRCKey;
            this.adNetwork = adNetwork;
            this.adID = adID;
            this.adID2 = adID2;
            this.autoReload = autoReload;
        }

        public virtual void UpdateInfo(string[] request)
        {
            adNetwork = (AdNetwork)int.Parse(request[0]);
            adID = request[1];
            adID2 = request[2];
        }
        public void CreateInterstitialTestAd()
        {
            switch (adNetwork)
            {
#if admob_enabled
                case AdNetwork.Admob:
                    adID = InterstitialTest;
                    adID2 = InterstitialTest;
                    break;
#endif
#if applovin_enabled
                case AdNetwork.Applovin:
                    adNetwork = AdNetwork.Admob;
                    adID = InterstitialTest;
                    adID2 = InterstitialTest;
                    break;
#endif
#if ironsource_enabled
                case AdNetwork.IronSource:
#if UNITY_ANDROID
                    adID = "aeyqi3vqlv6o8sh9";
                    adID2 = "aeyqi3vqlv6o8sh9";
#elif UNITY_IPHONE
                    adID = "wmgt0712uuux8ju4";
                    adID2 = "wmgt0712uuux8ju4";
#endif
                    break;
#endif
                default:
                    adNetwork = AdNetwork.None;
                    break;

            }
        }
        public void CreateRewardedTestAd()
        {
            if (adNetwork != AdNetwork.None)
            {
#if admob_enabled
                adNetwork = AdNetwork.Admob;
#else
                adNetwork = AdNetwork.None;
#endif
            }
            adID = RewardedTest;
            adID2 = RewardedTest;
        }
        public void CreateAppOpenTestAd()
        {
            if (adNetwork != AdNetwork.None)
            {
#if admob_enabled
                adNetwork = AdNetwork.Admob;
#else
                adNetwork = AdNetwork.None;
#endif
            }
            adID = AppOpenTest;
            adID2 = AppOpenTest;
        }
        public void CreateNativeTestAd()
        {
            if (adNetwork != AdNetwork.None)
            {
#if admob_enabled
                adNetwork = AdNetwork.Admob;
#else
                adNetwork = AdNetwork.None;
#endif
            }
            adID = NativeAdvancedTest;
            adID2 = NativeAdvancedTest;
        }
        public void CreateNativeVideoTestAd()
        {
            if (adNetwork != AdNetwork.None)
            {
#if admob_enabled
                adNetwork = AdNetwork.Admob;
#else
                adNetwork = AdNetwork.None;
#endif
            }
            adID = NativeAdvancedVideoTest;
            adID2 = NativeAdvancedVideoTest;
        }
        public void CreateBannerTestAd()
        {
            switch (adNetwork)
            {
#if admob_enabled
                case AdNetwork.Admob:
                    adID = AdaptiveBanner;
                    adID2 = AdaptiveBanner;
                    break;
#endif
#if applovin_enabled
                case AdNetwork.Applovin:
                    adNetwork = AdNetwork.Admob;
                    adID = AdaptiveBanner;
                    adID2 = AdaptiveBanner;
                    break;
#endif
#if ironsource_enabled
                case AdNetwork.IronSource:
#if UNITY_ANDROID
                    adID = "thnfvcsog13bhn08";
                    adID2 = "thnfvcsog13bhn08";
#elif UNITY_IPHONE
                    adID = "iep3rxsyp9na3rw8";
                    adID2 = "iep3rxsyp9na3rw8";
                    #endif
                    break;
#endif
                default:
                    adNetwork = AdNetwork.None;
                    break;

            }
            
        }
        
        //test id from google
        protected const string AppOpenTest = "ca-app-pub-3940256099942544/9257395921";
        protected const string BannerTest = "ca-app-pub-3940256099942544/6300978111";
        protected const string AdaptiveBanner = "ca-app-pub-3940256099942544/9214589741";
        protected const string InterstitialTest = "ca-app-pub-3940256099942544/1033173712";
        protected const string InterstitialVideoTest = "ca-app-pub-3940256099942544/8691691433";
        protected const string RewardedTest = "ca-app-pub-3940256099942544/5224354917";
        protected const string RewardedInterstitialTest = "ca-app-pub-3940256099942544/5354046379";
        protected const string NativeAdvancedTest = "ca-app-pub-3940256099942544/2247696110";
        protected const string NativeAdvancedVideoTest = "ca-app-pub-3940256099942544/1044960115";
        protected const string BannerTestIronsource = "ca-app-pub-3940256099942544/1044960115";
        protected const string InterstitialTestIronsource = "ca-app-pub-3940256099942544/1044960115";
        
    }
}
