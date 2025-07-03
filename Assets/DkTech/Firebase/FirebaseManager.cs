#if firebase_enabled
using Dktech.Services.Advertisement;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using Firebase.Messaging;
using Firebase.RemoteConfig;
#endif
#if adjust_enabled
using AdjustSdk;
#endif
#if facebook_enabled
using Facebook.Unity;
#endif
#if ironsource_enabled
using Unity.Services.LevelPlay;
#endif
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Dktech.Services.Firebase
{
    public enum RemoteConfigFetchState { Waiting, Success, Failure }
    public static class FirebaseManager
    {
        public static Action OnFetchCompleted { get; set; }
        public static Action OnFetchFailed { get; set; }
        public static bool InitComplete { get; set; }
        public static RemoteConfigFetchState FetchState { get; set; } = RemoteConfigFetchState.Waiting;
        public static FirebaseSettings settings;
        static readonly Dictionary<string, object> defaults = new();

        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            LoadSettings();
#if firebase_enabled
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    InitComplete = true;
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    // Crashlytics will use the DefaultInstance, as well
                    // this ensures that Crashlytics is initialized.
                    FirebaseApp app = FirebaseApp.DefaultInstance;
                    // Set a flag here for indicating that your project is ready to use Firebase.
                    InitRemoteConfig();
                    FirebaseMessaging.TokenReceived += OnTokenReceive;
                    FirebaseMessaging.MessageReceived += OnMessageReceived;
                }
                else
                {
                    Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
#endif
        }

        private static void LoadSettings()
        {
            settings = Resources.Load<FirebaseSettings>("FirebaseSettings");
            if (settings == null)
            {
                Debug.LogError("DKTech SDK: Firebase settings not found. Please select taskbar DKTech/Firebase Settings to create one");
            }
        }
#if firebase_enabled
        #region Message
        private static void OnTokenReceive(object sender, TokenReceivedEventArgs e)
        {
            if (settings.GetTokenFirebase)
            {
                GUIUtility.systemCopyBuffer = e.Token;
                Toast.ShowToastUI("copied Token Firebase to Clipboard", 1f);
            }
        }
        private static void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
        }
        #endregion

        #region Remote Config
        private static void InitRemoteConfig()
        {
            // These are the values that are used if we haven't fetched data from the
            // server
            // yet, or if we ask for values that the server doesn't have:
            defaults.Clear();
            defaults.Add(settings.ConfigTimeWaitingInter.key, 30);
            defaults.Add("LEVEL_CMP", "-1");
            defaults.Add(settings.ConfigCheckInternet.key, settings.ConfigCheckInternet.value);
            defaults.Add(settings.ConfigCheckUpdate.key, settings.ConfigCheckUpdate.value);
            if (settings.listRequest?.Count > 0)
            {
                foreach (var item in settings.listRequest)
                {
                    defaults.Add(item.key, item.value);
                }
            }

            FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
              .ContinueWithOnMainThread(task =>
              {
                  FetchDataAsync();
              });
        }

        private static Task FetchDataAsync()
        {
            Task fetchTask =
            FirebaseRemoteConfig.DefaultInstance.FetchAsync(
                TimeSpan.Zero);
            return fetchTask.ContinueWithOnMainThread(FetchComplete);
        }
        private static void FetchComplete(Task fetchTask)
        {
            if (fetchTask.IsCanceled)
            {
                FetchState = RemoteConfigFetchState.Failure;
                Debug.Log("Fetch canceled.");
            }
            else if (fetchTask.IsFaulted)
            {
                FetchState = RemoteConfigFetchState.Failure;
                Debug.Log("Fetch encountered an error.");
            }
            else if (fetchTask.IsCompleted)
            {
                Debug.Log("Fetch completed successfully!");
            }
            ConfigInfo info = null;
            try
            {
                info = FirebaseRemoteConfig.DefaultInstance.Info;
            }
            catch
            {
                FetchState = RemoteConfigFetchState.Failure;
                OnFetchFailed?.Invoke();
                return;
            }
            switch (info.LastFetchStatus)
            {
                case LastFetchStatus.Success:
                    _ = FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
                    .ContinueWithOnMainThread(task =>
                    {
                        FetchState = RemoteConfigFetchState.Success;
                        InterstitialAdManager.TimeWaitingInter = (int)FirebaseRemoteConfig.DefaultInstance.GetValue(settings.ConfigTimeWaitingInter.key).LongValue;
                        AdsUtilities.SettingShowCMP = FirebaseRemoteConfig.DefaultInstance.GetValue("LEVEL_CMP").StringValue.Split(",").AsEnumerable().Select(int.Parse).ToList();
                        NetworkManager.SetCheckInternet(FirebaseRemoteConfig.DefaultInstance.GetValue(settings.ConfigCheckInternet.key).BooleanValue);
#if google_update_enabled && !UNITY_EDITOR
                        if (FirebaseRemoteConfig.DefaultInstance.GetValue(settings.ConfigCheckUpdate.key).BooleanValue)
                        {
                            ServicesManager.instance.StartCheckUpdate(() =>
                            {
                                AdsUtilities.InitLoadAds();
                            });
                        }
                        else
                        {
                            AdsUtilities.InitLoadAds();
                        }
#else
                        AdsUtilities.InitLoadAds();
#endif
                        OnFetchCompleted?.Invoke();
                    });
                    break;
                case LastFetchStatus.Failure:
                    FetchState = RemoteConfigFetchState.Failure;
                    OnFetchFailed?.Invoke();
                    switch (info.LastFetchFailureReason)
                    {

                        case FetchFailureReason.Error:
                            Debug.Log("Fetch failed for unknown reason");
                            break;
                        case FetchFailureReason.Throttled:
                            Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                            break;
                    }
                    break;
                case LastFetchStatus.Pending:
                    FetchState = RemoteConfigFetchState.Failure;
                    Debug.Log("Latest Fetch call still pending.");
                    OnFetchFailed?.Invoke();
                    break;
            }
        }

        public static void CheckReloadRemoteConfig()
        {
            if (FetchState == RemoteConfigFetchState.Failure)
            {
                InitRemoteConfig();
            }
        }
        #endregion
#endif

        #region Get Config Methods
        public static int GetRemoteConfigIntValue(string key)
        {
#if firebase_enabled
            if (FetchState == RemoteConfigFetchState.Success && FirebaseRemoteConfig.DefaultInstance.Keys.Contains(key))
            {
                return (int)FirebaseRemoteConfig.DefaultInstance.GetValue(key).LongValue;
            }
#endif
            var rq = settings.listRequest.Find(x => x.key == key);
            if (rq != null) return rq.IntValue;
            else return 0;
        }
        public static bool GetRemoteConfigBooleanValue(string key)
        {
#if firebase_enabled
            if (FetchState == RemoteConfigFetchState.Success && FirebaseRemoteConfig.DefaultInstance.Keys.Contains(key))
            {
                return FirebaseRemoteConfig.DefaultInstance.GetValue(key).BooleanValue;
            }
#endif
            Debug.Log($"GetRemoteConfigBooleanValue: {key} - FetchState: {FetchState}");
            var rq = settings.listRequest.Find(x => x.key == key);
            if (rq != null) return rq.BooleanValue;
            throw new FormatException($"Key {key} not exist");
        }
        public static string GetRemoteConfigStringValue(string key)
        {
#if firebase_enabled
            if (FetchState == RemoteConfigFetchState.Success && FirebaseRemoteConfig.DefaultInstance.Keys.Contains(key))
            {
                return FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
            }
#endif
            var rq = settings.listRequest.Find(x => x.key == key);
            if (rq != null) return rq.StringValue;
            throw new FormatException($"Key {key} not exist");
        }
        public static double GetRemoteConfigDoubleValue(string key)
        {
#if firebase_enabled
            if (FetchState == RemoteConfigFetchState.Success && FirebaseRemoteConfig.DefaultInstance.Keys.Contains(key))
            {
                return FirebaseRemoteConfig.DefaultInstance.GetValue(key).DoubleValue;
            }
#endif
            var rq = settings.listRequest.Find(x => x.key == key);
            if (rq != null) return rq.DoubleValue;
            throw new FormatException($"Key {key} not exist");
        }
        public static int[] GetRemoteConfigIntArrayValue(string key)
        {
#if firebase_enabled
            if (FetchState == RemoteConfigFetchState.Success && FirebaseRemoteConfig.DefaultInstance.Keys.Contains(key))
            {
                return FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue.Split(",").AsEnumerable().Select(int.Parse).ToArray();
            }
#endif
            var rq = settings.listRequest.Find(x => x.key == key);
            if (rq != null) return rq.StringValue.Split(",").AsEnumerable().Select(int.Parse).ToArray();
            throw new FormatException($"Key {key} not exist");
        }
        public static string[] GetRemoteConfigStringArrayValue(string key)
        {
#if firebase_enabled
            if (FetchState == RemoteConfigFetchState.Success && FirebaseRemoteConfig.DefaultInstance.Keys.Contains(key))
            {
                return FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue.Split(",");
            }
#endif
            var rq = settings.listRequest.Find(x => x.key == key);
            if (rq != null) return rq.StringValue.Split(",");
            throw new FormatException($"Key {key} not exist");
        }
        public static bool GetDefaultCheckInternet() => settings.ConfigCheckInternet.BooleanValue;
        #endregion

        #region Log Event Firebasse & Adjust & Facebook
        public static void TrackEvent(string eventName)
        {
            Debug.LogWarning(eventName);
#if !UNITY_EDITOR && firebase_enabled
        if(InitComplete)
            FirebaseAnalytics.LogEvent(eventName);
#endif
        }
#if applovin_enabled
        public static void SendRevFirebase(MaxSdkBase.AdInfo adInfo)
        {
#if !UNITY_EDITOR && firebase_enabled
        if(!InitComplete) return;
        var impressionParameters = new[] {
            new Parameter("ad_platform", "AppLovin"),
            new Parameter("ad_source", adInfo.NetworkName),
            new Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
            new Parameter("ad_format", adInfo.Placement),
            new Parameter("value", adInfo.Revenue),
            new Parameter("currency", "USD"), // All Applovin revenue is sent in USD
            };
        FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
        Debug.LogError("DKTech SDK: Send revenue Applovin to Firebase: " + adInfo.Revenue);
#endif
        }
        public static void SendRevAdjust(MaxSdkBase.AdInfo adInfo)
        {
#if !UNITY_EDITOR && adjust_enabled
            var adRevenue = new AdjustAdRevenue("applovin_max_sdk");
            adRevenue.SetRevenue(adInfo.Revenue, "USD");
            adRevenue.AdRevenueNetwork = adInfo.NetworkName;
            adRevenue.AdRevenueUnit = adInfo.AdUnitIdentifier;
            adRevenue.AdRevenuePlacement = adInfo.Placement;

            Adjust.TrackAdRevenue(adRevenue);
            Debug.LogError("DKTech SDK: Send revenue Applovin to Adjust: " + adInfo.Revenue);
#endif
        }

        public static void SendRevFacebook(MaxSdkBase.AdInfo adInfo)
        {
#if !UNITY_EDITOR && facebook_enabled
            var param = new Dictionary<string, object>();
            param[AppEventParameterName.Currency] = "USD";
            FB.LogAppEvent("AdImpression", (float)adInfo.Revenue, param);
#endif
        }
#endif
#if ironsource_enabled
        public static void SendRevFirebase(LevelPlayImpressionData impressionData)
        {
#if !UNITY_EDITOR && firebase_enabled
            if (!InitComplete || impressionData == null) return;
            var impressionParameters = new[] {
                new Parameter("ad_platform", "ironSource"),
                new Parameter("ad_source", impressionData.AdNetwork),
                new Parameter("ad_unit_name", impressionData.MediationAdUnitName),
                new Parameter("ad_format", impressionData.AdFormat),
                new Parameter("currency","USD"),
                new Parameter("value", impressionData.Revenue.Value)
            };
            FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
            Debug.LogError("DKTech SDK: Send revenue Ironsource to Firebase: " + impressionData.Revenue.Value);
#endif
        }
        
        public static void SendRevFacebook(LevelPlayImpressionData impressionData)
        {
#if !UNITY_EDITOR && facebook_enabled
            var param = new Dictionary<string, object>();
            param[AppEventParameterName.Currency] = "USD";
            FB.LogAppEvent("AdImpression", impressionData.Revenue.Value, param);
#endif
        }
        public static void SendRevAdjust(LevelPlayImpressionData impressionData)
        {
#if !UNITY_EDITOR && adjust_enabled
            if (impressionData is null)
            {
                return;
            }
            AdjustAdRevenue adjustAdRevenue = new("ironsource_sdk");
            adjustAdRevenue.SetRevenue(impressionData.Revenue.Value, "USD");
            // optional fields
            adjustAdRevenue.AdRevenueNetwork = impressionData.AdNetwork;
            adjustAdRevenue.AdRevenueUnit = impressionData.MediationAdUnitName;
            adjustAdRevenue.AdRevenuePlacement = impressionData.Placement;
            // Send Adjust ad revenue
            Adjust.TrackAdRevenue(adjustAdRevenue);
            Debug.LogError("DKTech SDK: Send revenue Ironsource to Adjust: " + impressionData.Revenue.Value);
#endif

        }
#endif
#if admob_enabled
        public static void SendRevAdjust(GoogleMobileAds.Api.AdValue adValue, string adSourceName)
        {
#if !UNITY_EDITOR && adjust_enabled

            // send ad revenue info to Adjust
            AdjustAdRevenue adRevenue = new("admob_sdk");
            adRevenue.SetRevenue(adValue.Value / 1000000f, adValue.CurrencyCode);
            adRevenue.AdRevenueNetwork = "Admob";
            Adjust.TrackAdRevenue(adRevenue);
            Debug.LogError("DKTech SDK: Send revenue Admob to Adjust: " + (adValue.Value / 1000000f));
#endif
        }
        public static void SendRevFacebook(GoogleMobileAds.Api.AdValue adValue)
        {
#if !UNITY_EDITOR && facebook_enabled
            var param = new Dictionary<string, object>();
            param[AppEventParameterName.Currency] = "USD";
            FB.LogAppEvent("AdImpression", adValue.Value / 1000000f, param);
#endif
        }
        public static void SendRevFirebase(GoogleMobileAds.Api.AdValue adValue, string adSourceName)
        {
#if !UNITY_EDITOR && firebase_enabled
        if (!InitComplete) return;
        double revenue = adValue.Value;
        Parameter[] impressionParameters = new[]
        {
                new Parameter("ad_platform", "AdMob"),
                new Parameter("ad_source", adSourceName),
                new Parameter("value", revenue / 1000000f),
                new Parameter("currency", "USD"),
            };
        FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
        Debug.LogError("DKTech SDK: Send revenue Admob to Firebase: " + (adValue.Value / 1000000f));
#endif
        }
#endif
#endregion
    }
    [Serializable]
    public class RemoteConfigRequest
    {
        internal static Regex booleanTruePattern = new Regex("^(1|true|t|yes|y|on)$", RegexOptions.IgnoreCase);

        internal static Regex booleanFalsePattern = new Regex("^(0|false|f|no|n|off|)$", RegexOptions.IgnoreCase);
        public string name;
        public string key;
        public string value;
        public bool BooleanValue
        {
            get
            {
                string stringValue = StringValue;
                if (booleanTruePattern.IsMatch(stringValue))
                {
                    return true;
                }

                if (booleanFalsePattern.IsMatch(stringValue))
                {
                    return false;
                }

                throw new FormatException($"ConfigValue '{stringValue}' is not a boolean value");
            }
        }

        public double DoubleValue => Convert.ToDouble(StringValue, CultureInfo.InvariantCulture);

        public int IntValue => Convert.ToInt32(StringValue, CultureInfo.InvariantCulture);

        public string StringValue => value;
    }
}