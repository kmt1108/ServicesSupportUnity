#if admob_enabled
using Dktech.Services.Firebase;
using GoogleMobileAds.Api;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dktech.Services.Advertisement
{
    public class NativeAdAdmob : NativeAd
    {
        private static bool checkedTestDevice;
#if admob_native_enabled
        GoogleMobileAds.Api.NativeAd nativeAd;
        NativeAdContent currentContent;
#endif
#if admob_native_enabled
        public override void LoadAd()
        {
            if (waitingDelayReload) return;
            DestroyAd();
            isLoaded = false;
            AdLoader adLoader = new AdLoader.Builder(currentID)
            .ForNativeAd()
            .Build();
            adLoader.OnNativeAdLoaded += HandleNativeAdLoaded;
            adLoader.OnAdFailedToLoad += HandleNativeAdFailedToLoad;
            adLoader.OnNativeAdClicked += HandleNativeClicked;
            adLoader.LoadAd(new AdRequest());
            Debug.Log($"{ADMOB_LABEL} loading: {name}, id:{currentID}");

        }
        private void HandleNativeClicked(object sender, EventArgs e)
        {
            Debug.Log(ADMOB_LABEL + "was clicked.");
            IsAdClicked = true;
        }

        private void HandleNativeAdFailedToLoad(object sender, GoogleMobileAds.Api.AdFailedToLoadEventArgs args)
        {
            this.LoadAdComplete();
            Debug.LogError($"{ADMOB_LABEL} failed to load {name} with error : {args.LoadAdError}");
            CheckSwitchID();
            retryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
            DelayLoadAd((int)retryDelay * 1000);
        }
        private void HandleNativeAdLoaded(object sender, NativeAdEventArgs args)
        {
            this.LoadAdComplete();
            Debug.Log($"{ADMOB_LABEL} loaded {name} with response : {args.nativeAd.GetResponseInfo()}");
            nativeAd = args.nativeAd;
            if (!checkedTestDevice && AdsUtilities.CheckTestDevice)
            {
                checkedTestDevice = true;
                AdsUtilities.IsTestDevive = CheckTestDevice(nativeAd);
            }
            nativeAd.OnPaidEvent += OnPaidEvent;
            isLoaded = true;
            OnAdLoaded?.Invoke(this);
            retryAttempt = 0;
            if (isShowing && currentContent != null)
            {
                Debug.Log(ADMOB_LABEL + "register " + name);
                currentContent.SetData(this);
            }
        }
        private void OnPaidEvent(object sender, AdValueEventArgs e)
        {
            Debug.Log(String.Format(ADMOB_LABEL + "paid {0} {1}.",
                e.AdValue.Value,
                e.AdValue.CurrencyCode));
            string adapter = "Admob";
            try
            {
                adapter = nativeAd.GetResponseInfo().GetLoadedAdapterResponseInfo().AdSourceName;
            }
            catch (Exception)
            {
                Debug.LogWarning("DKTech SDK: Can't get Ad Network");
            }
            FirebaseManager.SendRevAdjust(e.AdValue, adapter);
            FirebaseManager.SendRevFacebook(e.AdValue);
        }
        private bool CheckTestDevice(GoogleMobileAds.Api.NativeAd ad)
        {
#if UNITY_ANDROID
            List<string> translations = new List<string>
            {
                "Quảngcáothửnghiệm",
                "测试广告",         // Chinese (Simplified)
                "Anunciodeprueba", // Spanish
                "TestAd",          // English
                "परीक्षणविज्ञापन",    // Hindi
                "إعلانتجريبي",       // Arabic
                "পরীক্ষাবিজ্ঞাপন",     // Bengali
                "Anúnciodeteste",  // Portuguese
                "Тестоваяреклама",   // Russian
                "ٹیسٹاشتہار",         // Urdu
                "테스트광고",         // Korean
                "Iklanujicoba"      // Indonesian
            };
#elif UNITY_IPHONE
            List<string> translations = new List<string>
            {
                "Chếđộkiểmtra",
                "测试模式",         // Chinese (Simplified)
                "Mododeprueba", // Spanish
                "Testmode",          // English
                "परीक्षणमोड",    // Hindi
                "وضعالاختبار",//Arabic 
                "পরীক্ষামোড",     // Bengali
                "Mododeteste",  // Portuguese
                "Тестовыйрежим",   // Russian
                "ٹیسٹوضع",            //Urdu
                "테스트모드",         // Korean
                "Modeujicoba"  // Indonesian
            };
#else
            List<string> translations = new List<string>(); 
#endif
            return translations.Contains(ad.GetHeadlineText().Replace(" ", "").Split(":")[0]);
        }
        public override void DestroyAd()
        {
            if (nativeAd != null)
            {
                nativeAd.Destroy();
            }
        }

        public override void ShowAd()
        {
            throw new NotImplementedException();
        }

        public override bool IsAvailable()
        {
            return nativeAd != null && isLoaded && !AdsUtilities.NoAds;
        }

        public override void ShowAd(NativeAdContent nativeAdContent, Action actShowed = null, Action<bool> actClosed = null)
        {
            actionClosed = actClosed;
            actionShowed = actShowed;
            isShowing = true;
            currentContent = nativeAdContent;
            //show shimmer before
            nativeAdContent.gameObject.SetActive(false);
            nativeAdContent.transform.parent.gameObject.SetActive(true);
            actionShowed?.Invoke();
            nativeAdContent.OnClickCloseAd = null;
            nativeAdContent.OnClickCloseAd += () =>
            {
                HideAd();
                actionClosed?.Invoke(true);
            };
            if (IsAvailable())
            {
                Debug.Log(ADMOB_LABEL + "register " + name);
                nativeAdContent.SetData(this);
            }
        }


        public override void HideAd()
        {
            if (isShowing)
            {
                isShowing = false;
                if (currentContent != null)
                {
                    currentContent.gameObject.SetActive(false);
                    currentContent.transform.parent.gameObject.SetActive(false);
                    currentContent = null;
                }
            }
        }

        public GoogleMobileAds.Api.NativeAd GetAd()
        {
            return nativeAd;
        }
#else
        public override void DestroyAd()
        {
            throw new NotImplementedException();
        }

        public override void HideAd()
        {
            throw new NotImplementedException();
        }

        public override bool IsAvailable()
        {
            throw new NotImplementedException();
        }

        public override void LoadAd()
        {
            throw new NotImplementedException();
        }

        public override void ShowAd(NativeAdContent nativeAdContent, Action actShowed = null, Action<bool> actClosed = null)
        {
            throw new NotImplementedException();
        }

        public override void ShowAd()
        {
            throw new NotImplementedException();
        }
#endif
    }
}