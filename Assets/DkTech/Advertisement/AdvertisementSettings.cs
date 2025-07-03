using UnityEditor;
using UnityEngine;

namespace Dktech.Services.Advertisement
{
    public class AdvertisementSettings : ScriptableObject
    {
        private const string AdvertisementSettingsResDir = "Assets/DkTech/Advertisement/Resources";
        private const string AdvertisementSettingsFile = "AdvertisementSettings";
        private const string AdvertisementSettingsFileExtension = ".asset";
        #region IronSource Propreties
#if ironsource_enabled
        [Header("IRONSOURCE PROPERTIES")]
        [SerializeField] private string ironsourceAppKey;
#endif
        #endregion

        #region Ad Request Properties
        [Header("REQUEST ADS INFO")]
        [SerializeField] private LoadAdGroup adsOrderPriority;
        [SerializeField] private LoadAdGroup adsOrderNormal;
        #endregion

        #region Common Properties
        [Header("COMMON PROPERTIES")]
        [SerializeField] private bool isTestMode;
        [SerializeField] private bool noAds, freeReward;
        [Header("ONRESUME")]
        [SerializeField] private bool waitingInBackgroundShowOnresume, checkTestDevice;
        #endregion

#if UNITY_EDITOR
        public static AdvertisementSettings LoadInstance()
        {
            //Read from resources.
            var instance = Resources.Load<AdvertisementSettings>(AdvertisementSettingsFile);

            //Create instance if null.
            if (instance == null)
            {
                System.IO.Directory.CreateDirectory(AdvertisementSettingsResDir);
                instance = CreateInstance<AdvertisementSettings>();
                string assetPath = System.IO.Path.Combine(AdvertisementSettingsResDir,AdvertisementSettingsFile + AdvertisementSettingsFileExtension);
                AssetDatabase.CreateAsset(instance, assetPath);
                AssetDatabase.SaveAssets();
            }
            return instance;
        }
#endif
        public LoadAdGroup AdsOrderPriority
        {
            get => adsOrderPriority;
            set => adsOrderPriority = value;
        }
        public LoadAdGroup AdsOrderNormal
        {
            get => adsOrderNormal;
            set => adsOrderNormal = value;
        }
        public bool IsTestMode
        {
            get => isTestMode;
            set => isTestMode = value;
        }
        public bool NoAds
        {
            get => noAds;
            set => noAds = value;
        }
        public bool FreeReward
        {
            get => freeReward;
            set => freeReward = value;
        }
        public bool WaitingInBackgroundShowOnresume
        {
            get => waitingInBackgroundShowOnresume;
            set => waitingInBackgroundShowOnresume = value;
        }
        public bool CheckTestDevice
        {
            get => checkTestDevice;
            set => checkTestDevice = value;
        }
#if ironsource_enabled
        public string IronsourceAppKey
        {
            get => ironsourceAppKey;
            set => ironsourceAppKey = value;
        }
#endif


    }
}
