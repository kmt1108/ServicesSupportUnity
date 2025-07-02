using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Dktech.Services.Advertisement
{
    public class LoadingUI : MonoBehaviour
    {
        [Description("Slider or Image")]
        [SerializeField] GameObject loadingBar;
        [SerializeField] Text percent;
        [SerializeField] string interSplashKey = "INTER_SPLASH";
        [SerializeField] float timeWaiting = 5;
        public bool IsLoading { get; set; }
        Coroutine loadingCoroutine;
        [SerializeField] UnityEvent loadingCompleteEvent = new();
        Slider loadingSlider;
        Image loadingImage;
        private void Start()
        {
            IsLoading = true;
            AdsUtilities.OnLoadPrimaryAdCompleted += CheckLoadingGameCompleted;
            loadingSlider = loadingBar.GetComponent<Slider>();
            if (!loadingSlider)
            {
                loadingImage = loadingBar.GetComponent<Image>();
            }
            loadingCoroutine = StartCoroutine(LoadingPop());
            loadingCompleteEvent.AddListener(AdsUtilities.LoadAdNormal);
        }
        IEnumerator LoadingPop()
        {
            if (AdsUtilities.NoAds)
            {
                if (loadingSlider) loadingSlider.value = 1f;
                else loadingImage.fillAmount = 1f;
                if (percent != null) percent.text = "Loading 100%";
                yield return new WaitForSeconds(0.1f);
                AdsUtilities.FirstOpen = false;
                IsLoading = false;
                AdsUtilities.IsLoadingGame = false;
                loadingCompleteEvent?.Invoke();
            }
            if(Application.internetReachability == NetworkReachability.NotReachable)
            {
                timeWaiting = 3;
            }
            float t = 0;
            while (t <= timeWaiting)
            {
                yield return new WaitForSeconds(timeWaiting / 100f);
                t += timeWaiting / 100f;
                if (loadingSlider) loadingSlider.value = t / timeWaiting;
                else loadingImage.fillAmount = t / timeWaiting;
                if (percent != null) percent.text = "Loading " + ((int)(t / timeWaiting * 100)).ToString() + "%";
            }
            CheckLoadingGameCompleted();
        }
        public void CheckLoadingGameCompleted()
        {
            Debug.Log("Loading Completed");
            if (loadingCoroutine != null) StopCoroutine(loadingCoroutine);
            if (!AdsUtilities.NoAds)
            {
                IsLoading = false;
                AdsUtilities.IsLoadingGame = false;
                AppOpenAdManager.ShowAppOpenAd();
                if (InterstitialAdManager.Contain(interSplashKey))
                {
                    InterstitialAdManager.ShowInterstitial(interSplashKey);
                }
            }
            StartCoroutine(DelayNextScene());
            
        }
        IEnumerator DelayNextScene()
        {
            if (loadingSlider) loadingSlider.value = 1f;
            else loadingImage.fillAmount = 1f;
            if (percent != null) percent.text = "Loading 100%";
            yield return new WaitForSeconds(0.1f);
            AdsUtilities.FirstOpen = false;
            loadingCompleteEvent?.Invoke();
        }
        private void OnDestroy()
        {
            AdsUtilities.OnLoadPrimaryAdCompleted -= CheckLoadingGameCompleted;
        }
    }
}