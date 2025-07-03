using Dktech.Services;
using Dktech.Services.Advertisement;
using UnityEngine;
using UnityEngine.UI;

public class TestShowAds : MonoBehaviour
{
    [SerializeField] Button btnShowBanner, btnShowInter, btnShowNative, btnShowReward, btnHideBanner, btnShowInterNative, btnShowRate;
    [SerializeField] InputField bannerName, interName, rewardName, nativeName;
    [SerializeField] NativeAdContent nativeAdContent;
    // Start is called before the first frame update
    void Start()
    {
        btnShowBanner.onClick.AddListener(() => BannerAdManager.LoadAndShowBanner(bannerName.text));
        btnShowInter.onClick.AddListener(() =>
        {
            InterstitialAdManager.ShowInterstitial(interName.text,
            actShowed: () =>
            {
                Debug.Log("inter show success!");
            }
            , actClosed: (showed) =>
            {
                Debug.Log("inter closed: " + (showed ? "show success" : "show fail"));
            });
        });
        btnShowReward.onClick.AddListener(() =>
        {
            RewardedAdManager.ShowRewarded(rewardName.text, (earned) =>
            {
                if (earned)
                {
                    Debug.Log("earned reward");
                }
                else
                {
                    Debug.Log("reward fail");
                }
            });
        });
        btnHideBanner.onClick.AddListener(() => BannerAdManager.HideCurrentBanner());
        btnShowNative.onClick.AddListener(() =>
        {
            //AdsUtilities.Banner.HideCurrentBanner();
            NativeAdManager.ShowNative(nativeName.text, nativeAdContent,
            actShowed: () => Debug.Log("native show success!"),
            actClosed: (showed) => Debug.Log("native closed: " + (showed ? "show success" : "show fail")));
        });
        btnShowInterNative.onClick.AddListener(() =>
        {
            InterstitialAdManager.ShowInterstitial(interName.text,
            actShowed: () =>
            {
                if (NativeAdManager.Contain(nativeName.text))
                {
                    NativeAdManager.ShowNative(nativeName.text, nativeAdContent,
                        actShowed: () => Debug.Log("native show success!"),
                        actClosed: (showed) => Debug.Log("native closed: " + (showed ? "show success" : "show fail")));
                }
                else
                {
                    Debug.Log("native not found: " + nativeName.text);
                }
            },
            actClosed: (showed) => Debug.Log("inter closed: " + (showed ? "show success" : "show fail")));
        });
        btnShowRate.onClick.AddListener(() =>
        {
            ServicesManager.instance.ShowRateUI();
        });
    }
}
