#if admob_enabled
using Dktech.Services.Firebase;
using GoogleMobileAds.Api;
#endif
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Dktech.Services.Advertisement
{
    public class NativeOverlayAdAdmob : NativeOverlayAd
    {
#if admob_enabled
        GoogleMobileAds.Api.NativeOverlayAd nativeOverlayAd;
#endif

#if admob_enabled
        public override void LoadAd()
        {
            // Clean up the old ad before loading a new one.
            if (waitingDelayReload) return;
            if (nativeOverlayAd != null)
            {
                DestroyAd();
            }
            // Create a request used to load the ad.
            var adRequest = new AdRequest();

            // Optional: Define native ad options.
            var options = new NativeAdOptions
            {
                AdChoicesPlacement = AdChoicesPlacement.TopRightCorner,
                MediaAspectRatio = MediaAspectRatio.Any,
            };

            // Send the request to load the ad.
            GoogleMobileAds.Api.NativeOverlayAd.Load(currentID, adRequest, options,
                (GoogleMobileAds.Api.NativeOverlayAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    this.LoadAdComplete();
                    Debug.LogError($"{ADMOB_LABEL} failed to load {name} with error : {error}");
                    if (HaveInternet) FirebaseManager.TrackEvent("NATIVE_OVERLAY_ADS_LOAD_FAIL");
                    //showToastOnUiThread("ad load fail");
                    CheckSwitchID();
                    retryAttempt++;
                    double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
                    DelayLoadAd((int)retryDelay*1000);
                    return;
                }

                // The ad should always be non-null if the error is null, but
                // double-check to avoid a crash.
                if (ad == null)
                {
                    this.LoadAdComplete();
                    Debug.LogError(ADMOB_LABEL+"loaded null ad and null error.");
                    if (HaveInternet) FirebaseManager.TrackEvent("NATIVE_OVERLAY_ADS_LOAD_FAIL");
                    //showToastOnUiThread("ad load fail");
                    CheckSwitchID();
                    retryAttempt++;
                    double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
                    DelayLoadAd((int)retryDelay * 1000);
                    return;
                }

                // The operation completed successfully.
                this.LoadAdComplete();
                Debug.Log($"{ADMOB_LABEL} loaded {name} with response : {ad.GetResponseInfo()}");
                nativeOverlayAd = ad;
                RenderAd();
                // Register to ad events to extend functionality.
                RegisterEventHandlers(ad);
                OnAdLoaded?.Invoke(this);
            });
            Debug.Log($"{ADMOB_LABEL} loading: {name}, id:{currentID}");
        }

        private void RegisterEventHandlers(GoogleMobileAds.Api.NativeOverlayAd nativeOverlay)
        {
            // Raised when the ad is estimated to have earned money.
            nativeOverlay.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format(ADMOB_LABEL + "paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
                string adapter = "Admob";
                try
                {
                    adapter = nativeOverlayAd.GetResponseInfo().GetLoadedAdapterResponseInfo().AdSourceName;
                }
                catch (Exception)
                {
                    Debug.LogWarning("DKTech SDK: Can't get Ad Network");
                }
                FirebaseManager.SendRevAdjust(adValue, adapter);
                FirebaseManager.SendRevFacebook(adValue);
            };
            // Raised when an impression is recorded for an ad.
            nativeOverlay.OnAdImpressionRecorded += () =>
            {
                Debug.Log(ADMOB_LABEL + "recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            nativeOverlay.OnAdClicked += () =>
            {
                Debug.Log(ADMOB_LABEL + "was clicked.");
                IsAdClicked = true;
            };
            // Raised when an ad opened full screen content.
            nativeOverlay.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log(ADMOB_LABEL + "full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            nativeOverlay.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log(ADMOB_LABEL + "full screen content closed.");
                this.RequestLoadAd();
            };
        }
        public void RenderAd()
        {
            if (nativeOverlayAd != null)
            {
                Debug.Log(ADMOB_LABEL+"Rendering Native Overlay ad "+name);
                nativeOverlayAd.RenderTemplate(style.ToNativeTemplateStyle(), position switch { BannerPosition.Top => AdPosition.Top, BannerPosition.Bottom => AdPosition.Bottom, _ => AdPosition.Center });
            }
        }


        public override void HideAd()
        {
            if (nativeOverlayAd != null)
            {
                Debug.Log("Hiding Native Overlay ad.");
                nativeOverlayAd.Hide();
            }
        }

        public override void DestroyAd()
        {
            if (nativeOverlayAd != null)
            {
                Debug.Log("Destroying native overlay ad.");
                nativeOverlayAd.Destroy();
                nativeOverlayAd = null;
            }
        }

        public override void ShowAd()
        {
            if (IsAvailable())
            {
                Debug.Log("Showing Native Overlay ad.");
                nativeOverlayAd.Show();
            }
        }

        public override bool IsAvailable()
        {
            return nativeOverlayAd != null&&!AdsUtilities.NoAds;
        }
#else
        public override void HideAd()
        {
            throw new NotImplementedException();
        }

        public override void LoadAd()
        {
            throw new NotImplementedException();
        }

        public override void ShowAd()
        {
            throw new NotImplementedException();
        }

        public override void DestroyAd()
        {
            throw new NotImplementedException();
        }

        public override bool IsAvailable()
        {
            throw new NotImplementedException();
        }

#endif
    }
}