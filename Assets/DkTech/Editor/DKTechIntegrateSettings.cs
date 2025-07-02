using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Dktech.Services.Editor
{
    public class DKTechIntegrateSettings : ScriptableObject
    {
        public const string SettingsExportPath = "DKTech/Resources/IntegrateSettings.asset";
        public const string gradlePath = "Assets/Plugins/Android/gradleTemplate.properties";
        public const string define_symbol_admob = "admob_enabled";
        public const string define_symbol_applovin = "applovin_enabled";
        public const string define_symbol_ironsource = "ironsource_enabled";
        public const string define_symbol_adjust = "adjust_enabled";
        public const string define_symbol_firebase = "firebase_enabled";
        public const string define_symbol_facebook = "facebook_enabled";
        public const string define_symbol_google_review = "google_review_enabled";
        public const string define_symbol_google_update = "google_update_enabled";
        public const string define_symbol_admob_native = "admob_native_enabled";
        public const string define_symbol_admob_applovin = "admob_applovin_enabled";
        public const string define_symbol_admob_ironsource = "admob_ironsource_enabled";
        public const string define_symbol_admob_inmobi = "admob_inmobi_enabled";
        public const string define_symbol_admob_mintegral = "admob_mintegral_enabled";
        public const string define_symbol_admob_unityads = "admob_unityads_enabled";
        public const string define_symbol_admob_meta = "admob_meta_enabled";
        public const string define_symbol_admob_litoff = "admob_litoff_enabled";
        public const string define_symbol_admob_dtexchange = "admob_dtexchange_enabled";
        public const string define_symbol_admob_pangle = "admob_pangle_enabled";
        static DKTechIntegrateSettings instance;
        public bool useAdmob;
        public bool useAdmobNative;
        public bool useApplovin;
        public bool useIronsource;
        public bool useFirebase;
        public bool useFacebook;
        public bool useAdjust;
        public bool useGoogleReview;
        public bool useGoogleUpdate;
        public bool useAdmobMediationApplovin;
        public bool useAdmobMediationIronsource;
        public bool useAdmobMediationInMobi;
        public bool useAdmobMediationMintegral;
        public bool useAdmobMediationUnityAds;
        public bool useAdmobMediationMeta;
        public bool useAdmobMediationLitOff;
        public bool useAdmobMediationDTExchange;
        public bool useAdmobMediationPangle;
        public bool showActiveScenes;
        public bool showUnactiveScenes;
        public bool showAllSceneInProject;
        public string folderPath = "Assets";
        public static DKTechIntegrateSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    string settingsFilePath;
                    // Note: Can't use absolute path when calling `CreateAsset`. Should use relative path to Assets/ directory.
                    settingsFilePath = Path.Combine("Assets", SettingsExportPath);

                    var dktechDir = Path.Combine(Application.dataPath, "DkTech");
                    if (!Directory.Exists(dktechDir))
                    {
                        Directory.CreateDirectory(dktechDir);
                    }
                    var settingsDir = Path.GetDirectoryName(settingsFilePath);
                    if (!Directory.Exists(settingsDir))
                    {
                        Directory.CreateDirectory(settingsDir);
                    }

                    instance = AssetDatabase.LoadAssetAtPath<DKTechIntegrateSettings>(settingsFilePath);
                    if (instance != null) return instance;

                    instance = CreateInstance<DKTechIntegrateSettings>();
                    AssetDatabase.CreateAsset(instance, settingsFilePath);
                }

                return instance;
            }
        }
        public void SaveData()
        {
            EditorUtility.SetDirty(instance);
        }

        public static void UpdateDefineSymbols()
        {
#if UNITY_ANDROID
            NamedBuildTarget target = NamedBuildTarget.Android;
#elif UNITY_IOS
        NamedBuildTarget target = NamedBuildTarget.iOS;
#else
        NamedBuildTarget target = NamedBuildTarget.Standalone;
#endif

            string defineSymbols = PlayerSettings.GetScriptingDefineSymbols(target);

            List<string> symbols;
            if (string.IsNullOrWhiteSpace(defineSymbols))
            {
                symbols = new List<string>();
            }
            else
            {
                symbols = defineSymbols.Split(";").ToList();
            }
            //check current define symbols contain admob define symbols
            if (Instance.useAdmob && !symbols.Contains(define_symbol_admob)) symbols.Add(define_symbol_admob);
            else if (!Instance.useAdmob && symbols.Contains(define_symbol_admob)) symbols.Remove(define_symbol_admob);

            //check current define symbols contain applovin define symbols
            if (Instance.useApplovin && !symbols.Contains(define_symbol_applovin)) symbols.Add(define_symbol_applovin);
            else if (!Instance.useApplovin && symbols.Contains(define_symbol_applovin)) symbols.Remove(define_symbol_applovin);

            //check current define symbols contain ironsource define symbols
            if (Instance.useIronsource)
            {
                if (!symbols.Contains(define_symbol_ironsource)) symbols.Add(define_symbol_ironsource);
#if UNITY_ANDROID
                if (!File.Exists(gradlePath))
                {
                    Debug.LogError("Couldn't find gradleTemplate.properties file. Please enabled Custom Gradle Properties Template in Player Setting and Update Define Symbols again!");
                }
                else
                {
                    var gradleLines = File.ReadAllLines(gradlePath).ToList();
                    if (!gradleLines.Contains("android.enableDexingArtifactTransform=false"))
                    {
                        gradleLines.Add("android.enableDexingArtifactTransform=false");
                        File.WriteAllText(gradlePath, string.Join("\n", gradleLines.ToArray()) + "\n");
                    }
                }
#endif
            }
            else if (!Instance.useIronsource && symbols.Contains(define_symbol_ironsource)) symbols.Remove(define_symbol_ironsource);


            //check current define symbols contain Adjust define symbols
            if (Instance.useAdjust && !symbols.Contains(define_symbol_adjust)) symbols.Add(define_symbol_adjust);
            else if (!Instance.useAdjust && symbols.Contains(define_symbol_adjust)) symbols.Remove(define_symbol_adjust);

            //check current define symbols contain firebase define symbols
            if (Instance.useFirebase && !symbols.Contains(define_symbol_firebase)) symbols.Add(define_symbol_firebase);
            else if (!Instance.useFirebase && symbols.Contains(define_symbol_firebase)) symbols.Remove(define_symbol_firebase);

            //check current define symbols contain facebook define symbols
            if (Instance.useFacebook && !symbols.Contains(define_symbol_facebook)) symbols.Add(define_symbol_facebook);
            else if (!Instance.useFacebook && symbols.Contains(define_symbol_facebook)) symbols.Remove(define_symbol_facebook);

            //check current define symbols contain native admob define symbols
            if (Instance.useAdmobNative && !symbols.Contains(define_symbol_admob_native)) symbols.Add(define_symbol_admob_native);
            else if (!Instance.useAdmobNative && symbols.Contains(define_symbol_admob_native)) symbols.Remove(define_symbol_admob_native);

            //check current define symbols contain google review define symbols
            if (Instance.useGoogleReview && !symbols.Contains(define_symbol_google_review)) symbols.Add(define_symbol_google_review);
            else if (!Instance.useGoogleReview && symbols.Contains(define_symbol_google_review)) symbols.Remove(define_symbol_google_review);

            //check current define symbols contain google review define symbols
            if (Instance.useGoogleUpdate && !symbols.Contains(define_symbol_google_update)) symbols.Add(define_symbol_google_update);
            else if (!Instance.useGoogleUpdate && symbols.Contains(define_symbol_google_update)) symbols.Remove(define_symbol_google_update);

            //check current define symbols contain admob mediation applovin define symbols
            if (Instance.useAdmobMediationApplovin && !symbols.Contains(define_symbol_admob_applovin)) symbols.Add(define_symbol_admob_applovin);
            else if (!Instance.useAdmobMediationApplovin && symbols.Contains(define_symbol_admob_applovin)) symbols.Remove(define_symbol_admob_applovin);

            //check current define symbols contain admob mediation dtexchange define symbols
            if (Instance.useAdmobMediationDTExchange && !symbols.Contains(define_symbol_admob_dtexchange)) symbols.Add(define_symbol_admob_dtexchange);
            else if (!Instance.useAdmobMediationDTExchange && symbols.Contains(define_symbol_admob_dtexchange)) symbols.Remove(define_symbol_admob_dtexchange);

            //check current define symbols contain admob mediation inmobi define symbols
            if (Instance.useAdmobMediationInMobi && !symbols.Contains(define_symbol_admob_inmobi)) symbols.Add(define_symbol_admob_inmobi);
            else if (!Instance.useAdmobMediationInMobi && symbols.Contains(define_symbol_admob_inmobi)) symbols.Remove(define_symbol_admob_inmobi);

            //check current define symbols contain admob mediation ironsource define symbols
            if (Instance.useAdmobMediationIronsource && !symbols.Contains(define_symbol_admob_ironsource)) symbols.Add(define_symbol_admob_ironsource);
            else if (!Instance.useAdmobMediationIronsource && symbols.Contains(define_symbol_admob_ironsource)) symbols.Remove(define_symbol_admob_ironsource);

            //check current define symbols contain admob mediation litoff define symbols
            if (Instance.useAdmobMediationLitOff && !symbols.Contains(define_symbol_admob_litoff)) symbols.Add(define_symbol_admob_litoff);
            else if (!Instance.useAdmobMediationLitOff && symbols.Contains(define_symbol_admob_litoff)) symbols.Remove(define_symbol_admob_litoff);

            //check current define symbols contain admob mediation meta define symbols
            if (Instance.useAdmobMediationMeta && !symbols.Contains(define_symbol_admob_meta)) symbols.Add(define_symbol_admob_meta);
            else if (!Instance.useAdmobMediationMeta && symbols.Contains(define_symbol_admob_meta)) symbols.Remove(define_symbol_admob_meta);

            //check current define symbols contain admob mediation mintegral define symbols
            if (Instance.useAdmobMediationMintegral && !symbols.Contains(define_symbol_admob_mintegral)) symbols.Add(define_symbol_admob_mintegral);
            else if (!Instance.useAdmobMediationMintegral && symbols.Contains(define_symbol_admob_mintegral)) symbols.Remove(define_symbol_admob_mintegral);

            //check current define symbols contain admob mediation pangle define symbols
            if (Instance.useAdmobMediationPangle && !symbols.Contains(define_symbol_admob_pangle)) symbols.Add(define_symbol_admob_pangle);
            else if (!Instance.useAdmobMediationPangle && symbols.Contains(define_symbol_admob_pangle)) symbols.Remove(define_symbol_admob_pangle);

            //check current define symbols contain admob mediation unity ads define symbols
            if (Instance.useAdmobMediationUnityAds && !symbols.Contains(define_symbol_admob_unityads)) symbols.Add(define_symbol_admob_unityads);
            else if (!Instance.useAdmobMediationUnityAds && symbols.Contains(define_symbol_admob_unityads)) symbols.Remove(define_symbol_admob_unityads);

            PlayerSettings.SetScriptingDefineSymbols(target, string.Join(";", symbols));
        }
    }
}