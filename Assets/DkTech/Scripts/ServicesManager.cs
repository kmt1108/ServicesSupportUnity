#if google_update_enabled && !UNITY_EDITOR
using Google.Play.AppUpdate;
using Google.Play.Common;
using System.Collections;
#endif
using Dktech.Services.Advertisement;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Dktech.Services
{
    public class ServicesManager : SingletonBehaviour<ServicesManager>
    {
        const string RATE_UI_PATH = "UI/Rate";
        const string TOAST_UI_PATH = "UI/Toast";
        const string NO_INTERNET_UI_PATH = "UI/NoInternet";
        const string LOADING_AD_PATH = "UI/LoadingAd";
        public static Action<bool> OnApplicationPauseEvent { get; set; }

        [SerializeField] string emailRate;
#if google_update_enabled && !UNITY_EDITOR
        public static bool CheckUpdate { get; set; }
        private AppUpdateManager _appUpdateManager;
        private AppUpdateInfo _appUpdateInfo;
        Action updateCompleteCallback;
#endif

        Rating rateUI;
        GameObject toastUI;
        GameObject noInternet;
        GameObject loadingAd;
        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            #if google_update_enabled && !UNITY_EDITOR
            _appUpdateManager = new AppUpdateManager();
#endif
        }

        public void ShowRateUI()
        {
            if (!rateUI)
            {
                var rating = Resources.Load<GameObject>(RATE_UI_PATH);
                if (rating)
                {
                    rateUI = Instantiate(rating,transform).GetComponent<Rating>();
                    rateUI.SetContactEmail(emailRate);
                }
                else
                {
                    Debug.LogError("RateUI prefab not found at path: " + RATE_UI_PATH);
                }
            }
            else
            {
                rateUI.transform.SetAsLastSibling();
                rateUI.gameObject.SetActive(true);
            }
        }
        public void ShowNoInternetUI(bool isShow)
        {
            if (!noInternet)
            {
                if (!isShow) return;
                noInternet = Instantiate(Resources.Load<GameObject>(NO_INTERNET_UI_PATH), transform);
                noInternet.GetComponentInChildren<Button>().onClick.AddListener(OpenWifiSetting);
                return;
            }
            noInternet.SetActive(isShow);
        }
        public void ShowLoadingAd(bool isShow)
        {
            if (!loadingAd)
            {
                if (!isShow) return;
                loadingAd = Instantiate(Resources.Load<GameObject>(LOADING_AD_PATH), transform);
                return;
            }
            loadingAd.SetActive(isShow);
        }
        public void ShowToastOnUI(string message, float time = 1f)
        {
            if(toastUI==null)
            {
                var toast = Resources.Load<GameObject>(TOAST_UI_PATH);
                if (toast)
                {
                    toastUI = Instantiate(toast, transform);
                }
                else
                {
                    Debug.LogError("Toast prefab not found at path: " + TOAST_UI_PATH);
                    return;
                }
            }
            GameObject t = Instantiate(toastUI,transform);
            t.GetComponent<Text>().text = message;
            t.GetComponent<Canvas>().worldCamera = Camera.main;
            Destroy(t, time);
        }
        public void OnNetworkConnectedChanged(string status)
        {
            NetworkManager.OnNetworkConnectedChanged(status == "Connected");
            // You can now broadcast this to the rest of your game
        }
        private void OnApplicationPause(bool pause)
        {
            OnApplicationPauseEvent?.Invoke(pause);
        }
        #region Open Wifi Setting
        public void OpenWifiSetting()
        {
            try
            {
                Rating.OutFromRate = true;
#if UNITY_ANDROID
                using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    string packageName = currentActivityObject.Call<string>("getPackageName");

                    using (var uriClass = new AndroidJavaClass("android.net.Uri"))
                    using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
                    using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.WIFI_SETTINGS"))
                    {
                        intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                        intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                        currentActivityObject.Call("startActivity", intentObject);
                    }
                }
#elif UNITY_IOS
            Application.OpenURL("App-prefs:WIFI");
#endif
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        #endregion

        #region In-App Update
#if google_update_enabled && !UNITY_EDITOR
        public void StartCheckUpdate(Action action)
        {
            updateCompleteCallback = action;
            StartCoroutine(CheckForUpdate());
        }

        private IEnumerator CheckForUpdate()
        {
            PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation = _appUpdateManager.GetAppUpdateInfo();
            // Wait until the asynchronous operation completes.
            yield return appUpdateInfoOperation;

            if (appUpdateInfoOperation.IsSuccessful)
            {
                Debug.Log("Update System: Check Update Successfull!");
                _appUpdateInfo = appUpdateInfoOperation.GetResult();
                Debug.Log("Update System:" + _appUpdateInfo.ToString());
                // Check AppUpdateInfo's UpdateAvailability, UpdatePriority,
                // IsUpdateTypeAllowed(), ... and decide whether to ask the user
                // to start an in-app update.
                if (_appUpdateInfo == null) yield break;

                if (_appUpdateInfo.UpdateAvailability != UpdateAvailability.UpdateNotAvailable && _appUpdateInfo.UpdateAvailability != UpdateAvailability.Unknown)
                {
                    Debug.Log("Update System: Has update available");
                    if (_appUpdateInfo.IsUpdateTypeAllowed(AppUpdateOptions.ImmediateAppUpdateOptions()))
                    {
                        yield return StartImmediateUpdate();
                    }
                    else if (_appUpdateInfo.IsUpdateTypeAllowed(AppUpdateOptions.FlexibleAppUpdateOptions()))
                    {
                        yield return StartFlexibleUpdate();
                    }
                }
                else
                {
                    Debug.Log("Update System: No update available");
                    updateCompleteCallback?.Invoke();
                    updateCompleteCallback = null;
                }

            }
            else
            {
                updateCompleteCallback?.Invoke();
                updateCompleteCallback = null;
                Debug.Log("Update System: " + appUpdateInfoOperation.Error);
                // Log appUpdateInfoOperation.Error.
            }
        }

        private IEnumerator StartFlexibleUpdate()
        {
            Debug.Log("Update System: Starting FlexibleUpdate");
            var startUpdateRequest = _appUpdateManager.StartUpdate(_appUpdateInfo, AppUpdateOptions.FlexibleAppUpdateOptions());
            yield return startUpdateRequest;
            if (startUpdateRequest.Status == AppUpdateStatus.Downloaded)
            {
                yield return CompleteFlexibleUpdate();
            }
            else
            {
                Debug.LogError("Flexible Update Failed: " + startUpdateRequest.Error);
                updateCompleteCallback?.Invoke();
                updateCompleteCallback = null;
            }
        }
        IEnumerator CompleteFlexibleUpdate()
        {
            yield return _appUpdateManager.CompleteUpdate();

            // If the update completes successfully, then the app restarts and this line
            // is never reached. If this line is reached, then handle the failure (e.g. by
            // logging result.Error or by displaying a message to the user).
        }

        private IEnumerator StartImmediateUpdate()
        {
            Debug.Log("Update System: Starting ImmediateUpdate");
            var startUpdateRequest = _appUpdateManager.StartUpdate(_appUpdateInfo, AppUpdateOptions.ImmediateAppUpdateOptions());
            yield return startUpdateRequest;
            if (startUpdateRequest.Error != AppUpdateErrorCode.NoError)
            {
                Debug.LogError("Update System: Immediate update failed: " + startUpdateRequest.Error);

            }
            else
            {
                Debug.LogError("Update System: Immediate update complete!");

            }
        }
#endif
        #endregion
    }
}