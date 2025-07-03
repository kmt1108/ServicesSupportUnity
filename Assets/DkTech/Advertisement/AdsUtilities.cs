using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Dktech.Services.Firebase;
#if facebook_enabled
using Facebook.Unity;
#endif
#if admob_enabled
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
#endif
#if ironsource_enabled
using Unity.Services.LevelPlay;
#endif

namespace Dktech.Services.Advertisement
{
    public static class AdsUtilities
    {
        #region Event Listener
        public static Action OnLoadPrimaryAdCompleted { get; set; }
        #endregion

        #region Common Properties
        internal static bool IsLoadingGame { get; set;} = true;
        internal static bool FirstOpen { get; set; }
        internal static List<int> SettingShowCMP { get; set; }
        public static bool WaitingInBackgroundShowOnresume { get; set; }
        public static bool NoAds { get; set; }
        public static bool CheckTestDevice { get; set; }
        public static bool IsTestMode { get; set; }
        public static bool IsTestDevive { get; set;}
        public static bool FreeReward { get; set;}
        public static bool CMP_ACCEPT { get { return PlayerPrefs.GetInt("cmp_accept", -1) > 0; } set { PlayerPrefs.SetInt("cmp_accept", value ? 1 : -1); } }
        public static int NetworkCount
        {
            get
            {
                int count = 0;
#if admob_enabled
                count++;
#endif
#if applovin_enabled
                count++;
#endif
#if ironsource_enabled
                count++;
#endif
                return count;
            }
        }
        //Count network initialized complete
        private static int initCompleteNetwork = 0;
        #endregion Common Properties

        #region RemoteConfig Properties

        //Check load ads is started
        private static bool loadAdsIsStarted;
        //Count ads priority loaded
        private static int adsPriorityLoadCount = 0;
        #endregion

        #region Advertisement Properties
        private static AdvertisementSettings settings;
        #endregion

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            LoadSettings();
            InitAdsManager();
#if admob_enabled
            MobileAds.RaiseAdEventsOnUnityMainThread = true;
            MobileAds.SetiOSAppPauseOnBackground(true);
#endif
            FirstOpen = !PlayerPrefs.HasKey("first_open");
            if (FirstOpen) PlayerPrefs.SetInt("first_open", 1);
#if admob_enabled
            SetUpMediationAdmob();
#endif
#if facebook_enabled
            if (!FB.IsInitialized)
            {
                // Initialize the Facebook SDK
                FB.Init(InitCallback, OnHideUnity);
            }
            else
            {
                // Already initialized, signal an app activation App Event
                FB.ActivateApp();
            }
#endif
            InitAdNetwork();

#if admob_enabled
            if (!CMP_ACCEPT) StarCMP_Admod();
#endif
        }
        private static void LoadSettings()
        {
            settings = Resources.Load<AdvertisementSettings>("AdvertisementSettings");
            if (settings == null)
            {
                Debug.LogError("DKTech SDK: Advertisement settings not found. Please select taskbar DKTech/Advertisement Settings to create one");
                return;
            }
            NoAds = settings.NoAds;
            CheckTestDevice = settings.CheckTestDevice;
            IsTestMode = settings.IsTestMode;
            FreeReward = settings.FreeReward;
        }

        private static void SetUpMediationAdmob()
        {
#if admob_applovin_enabled
            GoogleMobileAds.Mediation.AppLovin.Api.AppLovin.SetHasUserConsent(true);
            GoogleMobileAds.Mediation.AppLovin.Api.AppLovin.SetDoNotSell(true);
#endif
#if admob_dtexchange_enabled
            GoogleMobileAds.Mediation.DTExchange.Api.DTExchange.SetGDPRConsent(true);
            GoogleMobileAds.Mediation.DTExchange.Api.DTExchange.SetGDPRConsentString("myGDPRConsentString");
            GoogleMobileAds.Mediation.DTExchange.Api.DTExchange.SetCCPAString("myCCPAConsentString");
            // You can also clear CCPA consent information using the following method:
            GoogleMobileAds.Mediation.DTExchange.Api.DTExchange.ClearCCPAString();
#endif
#if admob_inmobi_enabled
            Dictionary<string, string> consentObject = new()
            {
                { "gdpr_consent_available", "true" },
                { "gdpr", "1" }
            };
            GoogleMobileAds.Mediation.InMobi.Api.InMobi.UpdateGDPRConsent(consentObject);
#endif
#if admob_ironsource_enabled
            GoogleMobileAds.Mediation.IronSource.Api.IronSource.SetConsent(true);
            GoogleMobileAds.Mediation.IronSource.Api.IronSource.SetMetaData("do_not_sell", "true");
#endif
#if admob_litoff_enabled
            GoogleMobileAds.Mediation.LiftoffMonetize.Api.LiftoffMonetize.SetGDPRStatus(true, "v1.0.0");
#if UNITY_IPHONE
            GoogleMobileAds.Mediation.LiftoffMonetize.Api.LiftoffMonetize.SetGDPRMessageVersion("v1.0.0");
#endif
            GoogleMobileAds.Mediation.LiftoffMonetize.Api.LiftoffMonetize.SetCCPAStatus(true);
#endif
#if admob_unityads_enabled
            GoogleMobileAds.Mediation.UnityAds.Api.UnityAds.SetConsentMetaData("gdpr.consent", true);
            GoogleMobileAds.Mediation.UnityAds.Api.UnityAds.SetConsentMetaData("privacy.consent", true);
#endif
        }

        private static void InitAdsManager()
        {
            if (settings.AdsOrderPriority.listBannerOrder.Count + settings.AdsOrderNormal.listBannerOrder.Count > 0) BannerAdManager.Initialize();
            if (settings.AdsOrderPriority.listInterOrder.Count + settings.AdsOrderNormal.listInterOrder.Count > 0) InterstitialAdManager.Initialize();
            if (settings.AdsOrderPriority.listRewardedOrder.Count + settings.AdsOrderNormal.listRewardedOrder.Count > 0) RewardedAdManager.Initialize();
            if (settings.AdsOrderPriority.listAOAOrder.Count + settings.AdsOrderNormal.listAOAOrder.Count > 0) AppOpenAdManager.Initialize();
#if admob_native_enabled
            if (settings.AdsOrderPriority.listNativeOrder.Count + settings.AdsOrderNormal.listNativeOrder.Count > 0) NativeAdManager.Initialize();
#endif
        }
        private static void InitAdNetwork()
        {
#if admob_enabled
            InitAdmob();
#endif
#if applovin_enabled
            InitApplovin();
#endif
#if ironsource_enabled
            InitIronsource();
#endif
        }
        public static void InitLoadAds()
        {
            if (!CMP_ACCEPT || loadAdsIsStarted || FirebaseManager.FetchState != RemoteConfigFetchState.Success || initCompleteNetwork < NetworkCount) return;
            loadAdsIsStarted = true;
            LoadAdPriority();
            if (!IsLoadingGame)
            {
                LoadAdNormal();
            }
        }
        private static void LoadAdPriority()
        {
            AdBase.OnAdLoaded += CheckLoadAdPriority;
            if (!settings.NoAds)
            {
                if (settings.AdsOrderPriority.listBannerOrder.Count > 0)
                {
                    BannerAdManager.RequestAd(settings.AdsOrderPriority.listBannerOrder);
                }
                if (settings.AdsOrderPriority.listInterOrder.Count > 0)
                {
                    InterstitialAdManager.RequestAd(settings.AdsOrderPriority.listInterOrder);
                }
                if (settings.AdsOrderPriority.listAOAOrder.Count > 0)
                {
                    AppOpenAdManager.RequestAd(settings.AdsOrderPriority.listAOAOrder);
                }
#if admob_native_enabled
                if (settings.AdsOrderPriority.listNativeOrder.Count > 0)
                {
                    NativeAdManager.RequestAd(settings.AdsOrderPriority.listNativeOrder);
                }
#endif
                if (settings.AdsOrderPriority.listNativeOverlayOrder.Count > 0)
                {
                    NativeOverlayAdManager.RequestAd(settings.AdsOrderPriority.listNativeOverlayOrder);
                }
            }
            if (settings.AdsOrderPriority.listRewardedOrder.Count > 0)
            {
                RewardedAdManager.RequestAd(settings.AdsOrderPriority.listRewardedOrder);
            }
        }
        public static void LoadAdNormal()
        {
            if (!loadAdsIsStarted) return;
            AdBase.OnAdLoaded -= CheckLoadAdPriority;
            if (!settings.NoAds)
            {
                if (settings.AdsOrderNormal.listBannerOrder.Count > 0)
                {
                    BannerAdManager.RequestAd(settings.AdsOrderNormal.listBannerOrder);
                }
                if (settings.AdsOrderNormal.listInterOrder.Count > 0)
                {
                    InterstitialAdManager.RequestAd(settings.AdsOrderNormal.listInterOrder);
                }
                if (settings.AdsOrderNormal.listAOAOrder.Count > 0)
                {
                    AppOpenAdManager.RequestAd(settings.AdsOrderNormal.listAOAOrder);
                }
#if admob_native_enabled
                if (settings.AdsOrderNormal.listNativeOrder.Count > 0)
                {
                    NativeAdManager.RequestAd(settings.AdsOrderNormal.listNativeOrder);
                }
#endif
                if (settings.AdsOrderNormal.listNativeOverlayOrder.Count > 0)
                {
                    NativeOverlayAdManager.RequestAd(settings.AdsOrderNormal.listNativeOverlayOrder);
                }
            }
            if (settings.AdsOrderNormal.listRewardedOrder.Count > 0)
            {
                RewardedAdManager.RequestAd(settings.AdsOrderNormal.listRewardedOrder);
            }
        }

        private static void CheckLoadAdPriority(AdBase ad)
        {
            if (++adsPriorityLoadCount == settings.AdsOrderPriority.TotalAdsCount && IsLoadingGame)
            {
                OnLoadPrimaryAdCompleted?.Invoke();
                Debug.Log("DKTech: All priority ads loaded");
            }                                                                                                      
        }

        #region Facebook Method
#if facebook_enabled
        private static void InitCallback()
        {
            if (FB.IsInitialized)
            {
                // Signal an app activation App Event
                FB.ActivateApp();
                // Continue with Facebook SDK
                // ...
            }
            else
            {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        }

        private static void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }
#endif
        #endregion

        #region Admob Method

#if admob_enabled

        private static void InitAdmob()
        {
            MobileAds.Initialize((initStatus) =>
            {
                Debug.Log("DKTech: Admob initialization completed");
                initCompleteNetwork++;
                Dictionary<string, AdapterStatus> map = initStatus.getAdapterStatusMap();
                foreach (KeyValuePair<string, AdapterStatus> keyValuePair in map)
                {
                    string className = keyValuePair.Key;
                    AdapterStatus status = keyValuePair.Value;
                    switch (status.InitializationState)
                    {
                        case AdapterState.NotReady:
                            // The adapter initialization did not complete.
                            Debug.Log("Adapter: " + className + " not ready.");
                            break;

                        case AdapterState.Ready:
                            // The adapter was successfully initialized.
                            Debug.Log("Adapter: " + className + " is initialized.");
                            break;
                    }
                }
                InitLoadAds();
            });
        }
#endif

        #endregion Admob Method

        #region Applovin Method

#if applovin_enabled
        private static void InitApplovin()
        {
            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
            {
                initCompleteNetwork++;
                InitLoadAds();
                Debug.Log("DKTech: Applovin initialization completed");
                // AppLovin SDK is initialized, start loading ads

                //ShowAdIfReady();
                //MaxSdk.ShowMediationDebugger();
            };
            MaxSdk.InitializeSdk();
        }
#endif

        #endregion

        #region Ironsource Method
#if ironsource_enabled
        private static void InitIronsource()
        {
                LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
                LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
                if (IsTestMode)
                {
#if UNITY_ANDROID
                    settings.IronsourceAppKey = "85460dcd";
#elif UNITY_IPHONE
                    settings.IronsourceAppKey = "8545d445";
#endif
                }
                LevelPlay.OnImpressionDataReady += ImpressionDataReadyEvent;
                LevelPlay.Init(settings.IronsourceAppKey);
                LevelPlay.ValidateIntegration();

        }
        private static void ImpressionDataReadyEvent(LevelPlayImpressionData impressionData)
        {
            if (IsTestMode) return;
            FirebaseManager.SendRevFirebase(impressionData);
            FirebaseManager.SendRevAdjust(impressionData);
            FirebaseManager.SendRevFacebook(impressionData);
        }

        private static void SdkInitializationFailedEvent(LevelPlayInitError error)
        {
            Debug.Log("DKTech: Ironsource initialization failed");
        }

        private static void SdkInitializationCompletedEvent(LevelPlayConfiguration configuration)
        {
            Debug.Log("DKTech: Ironsource initialization completed");
            initCompleteNetwork++;
            InitLoadAds();
        }
#endif
#endregion

        #region Common Method

        internal static void ShowLoadingPanel(bool isShow)
        {
            ServicesManager.instance.ShowLoadingAd(isShow);
        }

        #endregion

        #region ==========================CMP ADMOD 8.6.0========================
#if admob_enabled
        private static void StarCMP_Admod()
        {
            Debug.LogError("CMP: START");
            if (CMP_ACCEPT)
            {
                InitLoadAds();
                return;
            }
            if (CanShowAds() && CanShowPersonalizedAds())
            {
                Debug.LogError("CMP: START 01");
#if ironsource_enabled
                LevelPlay.SetConsent(isCMPConsent());
#endif
                CMP_ACCEPT = true;
                InitLoadAds();
            }
            else if (CanShowAds())
            {
                Debug.LogError("CMP: START 02");
#if ironsource_enabled
                IronSource.Agent.setConsent(isCMPConsent());
#endif
                CMP_ACCEPT = true;
                InitLoadAds();
            }
            else
            {
                ConsentRequestParameters request = new ConsentRequestParameters
                {
                    TagForUnderAgeOfConsent = false,
                };
                Debug.Log("CMP: ConsentInformation.Update");
                ConsentInformation.Update(request, OnConsentInfoUpdated);
            }
        }
        public static void CheckReloadCMP_With_Level(int level, Action<bool> actionResult)
        {
            if (CMP_ACCEPT) return;
            if (!CanShowAds() || !CanShowPersonalizedAds())
            {
                if (isCheckLevel_With_CMP_Firebase(level))
                {
                    actionResult(true);
                    actionResult = null;
                    ResetCMP_Admod();
                }
                else
                {
                    actionResult(false);
                    actionResult = null;
                }
            }
        }
        public static void CheckReloadCMP()
        {
            if (!CanShowAds() || !CanShowPersonalizedAds())
            {
                ResetCMP_Admod();
            }
        }
        private static bool isCheckLevel_With_CMP_Firebase(int level)
        {
            bool isResultShow_Cmp = false;
            for (int i = 0; i < SettingShowCMP.Count; i++)
            {
                if (SettingShowCMP[i] == level)
                {
                    isResultShow_Cmp = true;
                }
            }
            return isResultShow_Cmp;
        }
        public static void ResetCMP_Admod()
        {
            ConsentInformation.Reset();
            ConsentRequestParameters request = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false,
            };
            ConsentInformation.Update(request, OnConsentInfoUpdated);
        }
        private static void OnConsentInfoUpdated(FormError consentError)
        {
            if (consentError != null)
            {
                // Handle the error.
                Debug.LogError(consentError);
                // Về lý thuyết thì ko được Init chỗ này, nhưng test thì mình cứ Init
                CMP_ACCEPT = true;
                InitLoadAds();
                return;
            }
            // If the error is null, the consent information state was updated.
            // You are now ready to check if a form is available.
            ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
            {
                if (formError != null)
                {
                    // Consent gathering failed.
                    // Về lý thuyết thì ko được Init chỗ này, nhưng test thì mình cứ Init
                    InitLoadAds();
                    CMP_ACCEPT = true;
                    UnityEngine.Debug.LogError(consentError);
                    return;
                }
                // Consent has been gathered.
                if (ConsentInformation.CanRequestAds())
                {
                    //Request an ad.
                    Debug.LogError("CMP: START 03");
                    CMP_ACCEPT = true;
                    InitLoadAds();
                }
                else
                {
                    //show at level
                    //không load ad
                }
            });
        }
        private static bool isCMPConsent()
        {
            string myConsent = PlayerPrefs.GetString("IABTCF_AddtlConsent", "");
            Debug.LogError("isCMPConsent: myConsent :" + myConsent);
            if (myConsent.Contains("2878"))
            {
                Debug.LogError("isCMPConsent: OK");
                return true;
            }
            else
            {
                return false;
            }
        }
        // Check if GDPR applies
        private static bool IsGDPR()
        {
            // Replace with appropriate PlayerPrefs or other storage method
            int gdpr = PlayerPrefs.GetInt("IABTCF_gdprApplies", 0);
            return gdpr == 1;
        }
        // Check if consent is given for non-personalized ads
        private static bool CanShowAds()
        {
            string purposeConsent = PlayerPrefs.GetString("IABTCF_PurposeConsents", "") ?? "";
            string vendorConsent = PlayerPrefs.GetString("IABTCF_VendorConsents", "") ?? "";
            string vendorLI = PlayerPrefs.GetString("IABTCF_VendorLegitimateInterests", "") ?? "";
            string purposeLI = PlayerPrefs.GetString("IABTCF_PurposeLegitimateInterests", "") ?? "";
            int googleId = 755;
            bool hasGoogleVendorConsent = HasAttribute(vendorConsent, googleId);
            bool hasGoogleVendorLI = HasAttribute(vendorLI, googleId);
            Debug.LogError("CMP: purposeConsent: " + purposeConsent + " _vendorConsent: " + vendorConsent + "   _vendorLI: " + vendorLI + "  _purposeLI: " + purposeLI);
            Debug.LogError("CMP: hasGoogleVendorConsent: " + hasGoogleVendorConsent + "  _hasGoogleVendorLI: " + hasGoogleVendorLI);
            Debug.LogError("CMP: HasConsentFor: " + HasConsentFor(new int[] { 1 }, purposeConsent, hasGoogleVendorConsent));
            Debug.LogError("CMP: HasConsentOrLegitimateInterestFor: " + HasConsentOrLegitimateInterestFor(new int[] { 2, 7, 9, 10 }, purposeConsent, purposeLI, hasGoogleVendorConsent, hasGoogleVendorLI));
            // Minimum required for at least non-personalized ads
            return HasConsentFor(new int[] { 1 }, purposeConsent, hasGoogleVendorConsent)
                && HasConsentOrLegitimateInterestFor(new int[] { 2, 7, 9, 10 }, purposeConsent, purposeLI, hasGoogleVendorConsent, hasGoogleVendorLI);
        }
        // Check if consent is given for personalized ads
        private static bool CanShowPersonalizedAds()
        {
            string purposeConsent = PlayerPrefs.GetString("IABTCF_PurposeConsents", "") ?? "";
            string vendorConsent = PlayerPrefs.GetString("IABTCF_VendorConsents", "") ?? "";
            string vendorLI = PlayerPrefs.GetString("IABTCF_VendorLegitimateInterests", "") ?? "";
            string purposeLI = PlayerPrefs.GetString("IABTCF_PurposeLegitimateInterests", "") ?? "";
            int googleId = 755;
            bool hasGoogleVendorConsent = HasAttribute(vendorConsent, googleId);
            bool hasGoogleVendorLI = HasAttribute(vendorLI, googleId);
            Debug.LogError("CMP:VIP purposeConsent: " + purposeConsent + " _vendorConsent: " + vendorConsent + "   _vendorLI: " + vendorLI + "  _purposeLI: " + purposeLI);
            Debug.LogError("CMP:VIP hasGoogleVendorConsent: " + hasGoogleVendorConsent + "  _hasGoogleVendorLI: " + hasGoogleVendorLI);
            Debug.LogError("CMP:VIP HasConsentFor: " + HasConsentFor(new int[] { 1 }, purposeConsent, hasGoogleVendorConsent));
            Debug.LogError("CMP:VIP HasConsentOrLegitimateInterestFor: " + HasConsentOrLegitimateInterestFor(new int[] { 2, 7, 9, 10 }, purposeConsent, purposeLI, hasGoogleVendorConsent, hasGoogleVendorLI));
            return HasConsentFor(new int[] { 1, 3, 4 }, purposeConsent, hasGoogleVendorConsent)
                && HasConsentOrLegitimateInterestFor(new int[] { 2, 7, 9, 10 }, purposeConsent, purposeLI, hasGoogleVendorConsent, hasGoogleVendorLI);
        }
        // Check if a binary string has a "1" at position "index" (1-based)
        private static bool HasAttribute(string input, int index)
        {
            return input.Length >= index && input[index - 1] == '1';
        }
        // Check if consent is given for a list of purposes
        private static bool HasConsentFor(int[] purposes, string purposeConsent, bool hasVendorConsent)
        {
            return purposes.All(p => HasAttribute(purposeConsent, p)) && hasVendorConsent;
        }
        // Check if a vendor either has consent or legitimate interest for a list of purposes
        private static bool HasConsentOrLegitimateInterestFor(int[] purposes, string purposeConsent, string purposeLI, bool hasVendorConsent, bool hasVendorLI)
        {
            return purposes.All(p =>
                (HasAttribute(purposeLI, p) && hasVendorLI) ||
                (HasAttribute(purposeConsent, p) && hasVendorConsent)
            );
        }
#endif
        #endregion
    }
}