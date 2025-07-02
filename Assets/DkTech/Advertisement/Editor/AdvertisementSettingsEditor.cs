#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Dktech.Services.Advertisement.Editor
{
    [CustomEditor(typeof(AdvertisementSettings))]
    public class AdvertisementSettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            AdvertisementSettings settings = (AdvertisementSettings)target;

            if (GUILayout.Button("Update Ads Keys"))
            {
                UpdateEnum(settings);
            }
        }

        private void UpdateEnum(AdvertisementSettings settings)
        {
            string filePath = "Assets/DkTech/Advertisement/AdsKey.cs"; // Path to the file where you want to save the enum
            if (!File.Exists(filePath))
            {
                //Create the directory if it doesn't exist
                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            List<string> enumAdsKey = new List<string>();
            #region Add List AOA
            foreach (AppOpenAdRequest adPriority in settings.AdsOrderPriority.listAOAOrder)
            {
                enumAdsKey.Add(adPriority.Name);
            }
            foreach (AppOpenAdRequest adNormal in settings.AdsOrderNormal.listAOAOrder)
            {
                enumAdsKey.Add(adNormal.Name);
            }
            #endregion
            #region Add List Banner
            foreach (BannerAdRequest adPriority in settings.AdsOrderPriority.listBannerOrder)
            {
                enumAdsKey.Add(adPriority.Name);
            }
            foreach (BannerAdRequest adNormal in settings.AdsOrderNormal.listBannerOrder)
            {
                enumAdsKey.Add(adNormal.Name);
            }
            #endregion
            #region Add List Inter
            foreach (AdRequestInfo adPriority in settings.AdsOrderPriority.listInterOrder)
            {
                enumAdsKey.Add(adPriority.Name);
            }
            foreach (AdRequestInfo adNormal in settings.AdsOrderNormal.listInterOrder)
            {
                enumAdsKey.Add(adNormal.Name);
            }
            #endregion
            #region Add List Reward
            foreach (AdRequestInfo adPriority in settings.AdsOrderPriority.listRewardedOrder)
            {
                enumAdsKey.Add(adPriority.Name);
            }
            foreach (AdRequestInfo adNormal in settings.AdsOrderNormal.listRewardedOrder)
            {
                enumAdsKey.Add(adNormal.Name);
            }
            #endregion
            #region Add List Native
            foreach (AdRequestInfo adPriority in settings.AdsOrderPriority.listNativeOrder)
            {
                enumAdsKey.Add(adPriority.Name);
            }
            foreach (AdRequestInfo adNormal in settings.AdsOrderNormal.listNativeOrder)
            {
                enumAdsKey.Add(adNormal.Name);
            }
            #endregion
            #region Add List Native Overlay
            foreach (AdRequestInfo adPriority in settings.AdsOrderPriority.listNativeOverlayOrder)
            {
                enumAdsKey.Add(adPriority.Name);
            }
            foreach (AdRequestInfo adNormal in settings.AdsOrderNormal.listNativeOverlayOrder)
            {
                enumAdsKey.Add(adNormal.Name);
            }
            #endregion
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    /// <summary>
                    ///
                    /// </summary>
                    writer.WriteLine("public class AdsKey");
                    writer.WriteLine("{");
                    for (int i = 0; i < enumAdsKey.Count; i++)
                    {
                        writer.WriteLine("public static string " + enumAdsKey[i] + " = \"" + enumAdsKey[i] + "\";");
                    }
                    writer.WriteLine("}");
                }
                Debug.Log($"DKTech SDK: Ads config data is saved in {filePath}");
            }catch(Exception e)
            {
                Debug.LogError($"DKTech SDK: Save ads config data failed: {e.StackTrace}");
            }

            AssetDatabase.Refresh();
        }
    }
}
#endif