using System;
using UnityEngine;

namespace Dktech.Services.Advertisement
{
    [Serializable]
    public class NativeStyle
    {
        [Tooltip("small/medium")]
        [SerializeField] internal string TemplateId;

        [SerializeField] internal Color MainBackgroundColor;

        [SerializeField] internal NativeTextStyle PrimaryText;

        [SerializeField] internal NativeTextStyle SecondaryText;

        [SerializeField] internal NativeTextStyle TertiaryText;

        [SerializeField] internal NativeTextStyle CallToActionText;
#if admob_enabled
        public GoogleMobileAds.Api.NativeTemplateStyle ToNativeTemplateStyle()
        {
            return new GoogleMobileAds.Api.NativeTemplateStyle
            {
                TemplateId = TemplateId,
                MainBackgroundColor = MainBackgroundColor,
                PrimaryText = new GoogleMobileAds.Api.NativeTemplateTextStyle
                {
                    BackgroundColor = PrimaryText.BackgroundColor,
                    TextColor = PrimaryText.TextColor,
                    FontSize = PrimaryText.FontSize,
                    Style = (GoogleMobileAds.Api.NativeTemplateFontStyle)PrimaryText.Style
                },
                SecondaryText = new GoogleMobileAds.Api.NativeTemplateTextStyle
                {
                    BackgroundColor = SecondaryText.BackgroundColor,
                    TextColor = SecondaryText.TextColor,
                    FontSize = SecondaryText.FontSize,
                    Style = (GoogleMobileAds.Api.NativeTemplateFontStyle)SecondaryText.Style
                },
                TertiaryText = new GoogleMobileAds.Api.NativeTemplateTextStyle
                {
                    BackgroundColor = TertiaryText.BackgroundColor,
                    TextColor = TertiaryText.TextColor,
                    FontSize = TertiaryText.FontSize,
                    Style = (GoogleMobileAds.Api.NativeTemplateFontStyle)TertiaryText.Style
                },
                CallToActionText = new GoogleMobileAds.Api.NativeTemplateTextStyle
                {
                    BackgroundColor = PrimaryText.BackgroundColor,
                    TextColor = PrimaryText.TextColor,
                    FontSize = PrimaryText.FontSize,
                    Style = (GoogleMobileAds.Api.NativeTemplateFontStyle)PrimaryText.Style

                }
            };
        }
#endif
        [Serializable]
        public class NativeTextStyle
        {
            [SerializeField] internal Color BackgroundColor=Color.clear;

            [SerializeField] internal Color TextColor=Color.white;

            [SerializeField] internal int FontSize=10;

            [SerializeField] internal NativeFontStyle Style=NativeFontStyle.Normal;
        }
        [Serializable]
        public enum NativeFontStyle
        {
            Normal,
            Bold,
            Italic,
            Monospace
        }
    }
}
