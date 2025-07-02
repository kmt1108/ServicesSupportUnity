using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Dktech.Services.Advertisement
{
    public enum NativeAdType
    {
        Small,
        Medium,
        FullScreen
    }
    public class NativeAdContent : MonoBehaviour
    {
        public Action OnClickCloseAd { get; set; }
#if admob_native_enabled
        [SerializeField] bool checkScale = true;
        [SerializeField] NativeAdType nativeType;
        [SerializeField] GameObject adBackground;
        [SerializeField] GameObject adLabel;
        [SerializeField] GameObject adChoices;
        [SerializeField] GameObject adIcon;
        [SerializeField] GameObject adImage;
        [SerializeField] GameObject adHeadline;
        [SerializeField] GameObject adBodyText;
        [SerializeField] GameObject avtText;
        [SerializeField] GameObject adCallToAction;
        [SerializeField] GameObject actionButton;
        [SerializeField] AdRate adRate;
        [SerializeField] int endlineHeadline = -1, endlineBody = 45;
        [SerializeField] GameObject collapButton;
        Vector3 dfScaleImage=Vector3.zero;
        NativeAd currentNative;
        public void SetData(NativeAd native)
        {
            currentNative = native;
            GoogleMobileAds.Api.NativeAd nativeAd = ((NativeAdAdmob)native).GetAd();
            if (nativeAd == null) return;
            if (checkScale)
            {
                checkScale = false;
                CheckAlightmentMatchScreen();
            }
            if (adImage && dfScaleImage == Vector3.zero)
            {
                dfScaleImage = adImage.transform.localScale;
            }
            //register headline
            string headline = nativeAd.GetHeadlineText();
            if (adHeadline && !string.IsNullOrEmpty(headline))
            {
                if (headline.Length > 25) headline = headline[..25] + "...";
                if (endlineHeadline > 0)
                {
                    int endl = endlineHeadline;
                    while (headline.Length > endl)
                    {
                        int breakLine = 0;
                        for (int i = endl; i > 0; i--)
                        {
                            if (headline[i] == ' ')
                            {
                                breakLine = i; break;
                            }
                        }
                        if (breakLine > 0)
                        {
                            headline = headline.Insert(breakLine + 1, "\n");
                            endl = breakLine + endlineHeadline + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                adHeadline.GetComponent<TextMesh>().text = headline;
                adHeadline.AddComponent<BoxCollider>();
                if (nativeAd.RegisterHeadlineTextGameObject(adHeadline))
                {
                    Debug.Log("Native register Headline success");
                }
                else
                {
                    Debug.Log("Native register Headline fail");
                }
            }

            //register icon
            Texture2D icon = nativeAd.GetIconTexture();
            if (adIcon && icon != null)
            {
                adIcon.GetComponent<Renderer>().material.mainTexture = icon;
                adIcon.AddComponent<BoxCollider>();
                if (nativeAd.RegisterIconImageGameObject(adIcon))
                {
                    Debug.Log("Native register Icon Image success");
                }
                else
                {
                    Debug.Log("Native register Icon Image fail");
                }
            }

            //register body text
            string bodyText = nativeAd.GetBodyText();
            if (adBodyText != null && !string.IsNullOrEmpty(bodyText))
            {
                if (bodyText.Length > 90)
                {
                    bodyText = bodyText[..90] + "...";
                }
                if (endlineBody > 0)
                {
                    int endl = endlineBody;
                    while (bodyText.Length > endl)
                    {
                        int breakLine = 0;
                        for (int i = endl; i > 0; i--)
                        {
                            if (bodyText[i] == ' ')
                            {
                                breakLine = i; break;
                            }
                        }
                        if (breakLine > 0)
                        {
                            bodyText = bodyText.Insert(breakLine + 1, "\n");
                            endl = breakLine + endlineBody + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                adBodyText.GetComponent<TextMesh>().text = bodyText;
                adBodyText.AddComponent<BoxCollider>();
                if (nativeAd.RegisterBodyTextGameObject(adBodyText))
                {
                    Debug.Log("Native register BodyText success");
                }
                else
                {
                    Debug.Log("Native register BodyText fail");
                }
            }

            //rgister adchoice
            Texture2D adchoice = nativeAd.GetAdChoicesLogoTexture();
            if (adChoices && adchoice != null)
            {
                adChoices.GetComponent<Renderer>().material.mainTexture = adchoice;
                adChoices.AddComponent<BoxCollider>();
                if (nativeAd.RegisterAdChoicesLogoGameObject(adChoices))
                {
                    Debug.Log("Native register Adchoice success");
                }
                else
                {
                    Debug.Log("Native register Adchoice fail");
                }
            }

            var listImage = nativeAd.GetImageTextures();
            if (adImage != null && listImage?.Count > 0)
            {
                //register image
                Texture2D image = listImage[0];
                Vector2 textureSize = new(image.width, image.height);
                if (dfScaleImage.x / dfScaleImage.y > textureSize.x / textureSize.y)
                {
                    adImage.transform.localScale = new Vector3(dfScaleImage.y * textureSize.x / textureSize.y, dfScaleImage.y, dfScaleImage.z);
                }
                else
                {
                    adImage.transform.localScale = new Vector3(dfScaleImage.x, dfScaleImage.x * textureSize.y / textureSize.x, dfScaleImage.z);
                }
                adImage.GetComponent<Renderer>().material.mainTexture = image;
                adImage.AddComponent<BoxCollider>();
                List<GameObject> img = new() { adImage };
                if (nativeAd.RegisterImageGameObjects(img) > 0)
                {
                    Debug.Log("Native register Image success");
                }
                else
                {
                    Debug.Log("Native register Image fail");
                }
            }

            //register advertist text
            string avt = nativeAd.GetAdvertiserText();
            if (avtText != null && !string.IsNullOrEmpty(avt))
            {
                avtText.GetComponent<TextMesh>().text = avt;
                avtText.AddComponent<BoxCollider>();
                if (nativeAd.RegisterAdvertiserTextGameObject(avtText))
                {
                    Debug.Log("Native register Advertist text success");
                }
                else
                {
                    Debug.Log("Native register Advertist text fail");
                }
            }

            //register rating
            double rate = nativeAd.GetStarRating();
            if (adRate != null && rate > 0)
            {
                adRate.SetRate((float)rate);
            }

            //register call to action
            adCallToAction.GetComponent<TextMesh>().text = nativeAd.GetCallToActionText();
            actionButton.AddComponent<BoxCollider>();
            if (nativeAd.RegisterCallToActionGameObject(actionButton))
            {
                Debug.Log("Native register Call to Action success");
            }
            else
            {
                Debug.Log("Native register Call to Action fail");
            }
            if (collapButton)
            {
                transform.parent.GetComponent<Animator>().Play("btn_collapse_on");
            }
            //show ad
            gameObject.SetActive(true);
        }
        private void CheckAlightmentMatchScreen()
        {
            var mainCanvas = transform.GetComponentInParent<CanvasScaler>();
            var refSize = mainCanvas.referenceResolution;
            var realSize = refSize;
            float deltaX = 0;
            float deltaY = 0;
            if (mainCanvas.screenMatchMode == CanvasScaler.ScreenMatchMode.Expand)
            {
                if ((float)Screen.height / Screen.width < refSize.y / refSize.x)
                {
                    realSize = new(realSize.y * Screen.width / Screen.height, realSize.y);
                    deltaX = realSize.x - refSize.x;
                }
                else
                {
                    realSize = new(realSize.x, realSize.x * Screen.height / Screen.width);
                    deltaY = realSize.y - refSize.y;
                }
            }
            if (adBackground != null)
            {
                switch (nativeType)
                {
                    case NativeAdType.Small:
                    case NativeAdType.Medium:
                        adBackground.transform.localScale = new(realSize.x, adBackground.transform.localScale.y, adBackground.transform.localScale.z);
                        break;
                    case NativeAdType.FullScreen:
                        adBackground.transform.localScale = new(realSize.x, realSize.y, adBackground.transform.localScale.z);
                        break;
                }
            }
            if (adIcon != null)
            {
                switch (nativeType)
                {
                    case NativeAdType.Small:
                    case NativeAdType.Medium:
                        adIcon.transform.localPosition = new(adIcon.transform.localPosition.x - (deltaX / 2), adIcon.transform.localPosition.y, adIcon.transform.localPosition.z);
                        break;
                    case NativeAdType.FullScreen:
                        adIcon.transform.localPosition = new(adIcon.transform.localPosition.x - (deltaX / 2), adIcon.transform.localPosition.y - (deltaY / 2), adIcon.transform.localPosition.z);
                        break;
                }
            }
            if (actionButton != null)
            {
                switch (nativeType)
                {
                    case NativeAdType.Small:
                        actionButton.transform.localPosition = new(actionButton.transform.localPosition.x + (deltaX / 2), actionButton.transform.localPosition.y, actionButton.transform.localPosition.z);
                        break;
                    case NativeAdType.Medium:
                        actionButton.transform.localScale = new(realSize.x - 60, actionButton.transform.localScale.y, actionButton.transform.localScale.z);
                        break;
                    case NativeAdType.FullScreen:
                        actionButton.transform.localScale = new(realSize.x - 60, actionButton.transform.localScale.y, actionButton.transform.localScale.z);
                        actionButton.transform.localPosition = new(actionButton.transform.localPosition.x - (deltaX / 2), actionButton.transform.localPosition.y - (deltaY / 2), actionButton.transform.localPosition.z);
                        break;
                }
            }
            if (adCallToAction != null)
            {
                adCallToAction.transform.localPosition = new(actionButton.transform.localPosition.x,actionButton.transform.localPosition.y,adCallToAction.transform.localPosition.z);
            }
            if (adHeadline != null)
            {
                if (nativeType == NativeAdType.FullScreen) {
                    adHeadline.transform.localPosition = new(adHeadline.transform.localPosition.x - (deltaX / 2), adHeadline.transform.localPosition.y - (deltaY / 2), adHeadline.transform.localPosition.z);
                }
                else
                {
                    adHeadline.transform.localPosition = new(adHeadline.transform.localPosition.x - (deltaX / 2), adHeadline.transform.localPosition.y, adHeadline.transform.localPosition.z);
                }
            }
            if (adBodyText != null)
            {
                if (nativeType == NativeAdType.FullScreen)
                {
                    adBodyText.transform.localPosition = new(adBodyText.transform.localPosition.x - (deltaX / 2), adBodyText.transform.localPosition.y - (deltaY / 2), adBodyText.transform.localPosition.z);
                }
                else
                {
                    adBodyText.transform.localPosition = new(adBodyText.transform.localPosition.x - (deltaX / 2), adBodyText.transform.localPosition.y, adBodyText.transform.localPosition.z);
                }
            }
            if (adImage != null)
            {
                switch (nativeType)
                {
                    case NativeAdType.Medium:
                        adImage.transform.localScale = new(adImage.transform.localScale.x + deltaX, adImage.transform.localScale.y, adImage.transform.localScale.z);
                        break;
                    case NativeAdType.FullScreen:
                        adImage.transform.localScale = new(adImage.transform.localScale.x + deltaX, adImage.transform.localScale.y + deltaY, adImage.transform.localScale.z);
                        adImage.transform.localPosition = new(adImage.transform.localPosition.x, adImage.transform.localPosition.y + (deltaY / 2), adImage.transform.localPosition.z);
                        break;
                }
            }
            if (adLabel != null)
            {
                if (nativeType != NativeAdType.FullScreen)
                {
                    adLabel.transform.localPosition = new(adLabel.transform.localPosition.x - (deltaX / 2), adLabel.transform.localPosition.y, adLabel.transform.localPosition.z);
                }
                else
                {
                    adLabel.transform.localPosition = new(adLabel.transform.localPosition.x - (deltaX / 2), adLabel.transform.localPosition.y - (deltaY / 2), adLabel.transform.localPosition.z);
                }
            }
            if (adChoices != null)
            {
                if (nativeType != NativeAdType.FullScreen)
                {
                    adChoices.transform.localPosition = new(adChoices.transform.localPosition.x + (deltaX / 2), adChoices.transform.localPosition.y, adChoices.transform.localPosition.z);
                }
                else
                {
                    adChoices.transform.localPosition = new(adChoices.transform.localPosition.x + (deltaX / 2), adChoices.transform.localPosition.y + (deltaY / 2), adChoices.transform.localPosition.z);
                }
            }
        }
        private void OnDisable()
        {
            if (currentNative != null)
            {
                currentNative.HideAd();
                if (currentNative.AutoReload) currentNative.LoadAd();
                currentNative = null;
            }
        }
        public void OnClickCloseNative()
        {
            OnClickCloseAd?.Invoke();
        }
#endif
    }
}