using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Dktech.Services.Editor
{
    public class DKTechIntegrationManagerWindow : EditorWindow
    {
        private GUIStyle styleTitle;
        private GUIStyle styleHeader;
        private GUIStyle styleHeader2;

        private void OnEnable()
        {
            styleTitle = new GUIStyle();
            styleTitle.fontSize = 15;
            styleTitle.fontStyle = FontStyle.Bold;
            styleTitle.normal.textColor = Color.white;
            styleTitle.alignment = TextAnchor.UpperLeft;

            styleHeader = new GUIStyle();
            styleHeader.fontSize = 13;
            styleHeader.fontStyle = FontStyle.Bold;
            styleHeader.normal.textColor = Color.red;
            styleHeader.alignment = TextAnchor.UpperLeft;

            styleHeader2 = new GUIStyle();
            styleHeader2.fontSize = 13;
            styleHeader2.fontStyle = FontStyle.Normal;
            styleHeader2.normal.textColor = Color.red;
            styleHeader2.alignment = TextAnchor.UpperLeft;
        }
        private void OnDisable()
        {
            AssetDatabase.SaveAssets();
        }
        public static void ShowWindow()
        {
            var window = EditorWindow.GetWindow<DKTechIntegrationManagerWindow>(utility: true, title: "DKTech Integrate Manager", focus: true);
            window.minSize = new Vector2(500, 500);
        }
        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("DKTECH SERVICE SETTINGS", styleTitle);
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("NETWORK", styleHeader);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Admob", GUILayout.Width(45));
            DKTechIntegrateSettings.Instance.useAdmob = EditorGUILayout.Toggle(DKTechIntegrateSettings.Instance.useAdmob, GUILayout.Width(15));
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("Applovin", GUILayout.Width(55));
            DKTechIntegrateSettings.Instance.useApplovin = EditorGUILayout.Toggle(DKTechIntegrateSettings.Instance.useApplovin, GUILayout.Width(15));
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("Ironsource", GUILayout.Width(65));
            DKTechIntegrateSettings.Instance.useIronsource = EditorGUILayout.Toggle(DKTechIntegrateSettings.Instance.useIronsource, GUILayout.Width(15));
            EditorGUILayout.Space(2);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);
            if (DKTechIntegrateSettings.Instance.useAdmob)
            {
                DKTechIntegrateSettings.Instance.useAdmobNative = EditorGUILayout.Toggle("Use Native Admob", DKTechIntegrateSettings.Instance.useAdmobNative, GUILayout.Width(15));
                DrawAdmobMediationProperties();
            }
            else
            {
                DisableAdmobMediationProperties();
            }
            EditorGUILayout.LabelField("OTHER SDK", styleHeader);
            EditorGUILayout.Space(10);
            DKTechIntegrateSettings.Instance.useAdjust = EditorGUILayout.Toggle("Adjust", DKTechIntegrateSettings.Instance.useAdjust, GUILayout.Width(15));
            DKTechIntegrateSettings.Instance.useFirebase = EditorGUILayout.Toggle("Firebase", DKTechIntegrateSettings.Instance.useFirebase, GUILayout.Width(15));
            DKTechIntegrateSettings.Instance.useFacebook = EditorGUILayout.Toggle("Facebook", DKTechIntegrateSettings.Instance.useFacebook, GUILayout.Width(15));
#if UNITY_ANDROID
            DKTechIntegrateSettings.Instance.useGoogleReview = EditorGUILayout.Toggle("Google Review", DKTechIntegrateSettings.Instance.useGoogleReview, GUILayout.Width(15));
            DKTechIntegrateSettings.Instance.useGoogleUpdate = EditorGUILayout.Toggle("Google Update", DKTechIntegrateSettings.Instance.useGoogleUpdate, GUILayout.Width(15));
#else
            DKTechIntegrateSettings.Instance.useGoogleReview = false;
            DKTechIntegrateSettings.Instance.useGoogleUpdate = false;
#endif

            EditorGUILayout.Space(22);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Update Define Symbols"), GUILayout.Width(150)))
            {
                DKTechIntegrateSettings.UpdateDefineSymbols();
            }
#if UNITY_ANDROID
            if (GUILayout.Button(new GUIContent("Generate Proguard File"), GUILayout.Width(150)))
            {
                ProguardGenerator.GenerateProguardFile();
            }
#endif
            EditorGUILayout.EndHorizontal();


            if (GUI.changed)
            {
                DKTechIntegrateSettings.Instance.SaveData();
            }
        }
        private void DisableAdmobMediationProperties()
        {
            DKTechIntegrateSettings.Instance.useAdmobMediationApplovin = false;
            DKTechIntegrateSettings.Instance.useAdmobMediationDTExchange = false;
            DKTechIntegrateSettings.Instance.useAdmobMediationInMobi = false;
            DKTechIntegrateSettings.Instance.useAdmobMediationIronsource = false;
            DKTechIntegrateSettings.Instance.useAdmobMediationLitOff = false;
            DKTechIntegrateSettings.Instance.useAdmobMediationMeta = false;
            DKTechIntegrateSettings.Instance.useAdmobMediationMintegral = false;
            DKTechIntegrateSettings.Instance.useAdmobMediationPangle = false;
            DKTechIntegrateSettings.Instance.useAdmobMediationUnityAds = false;
            DKTechIntegrateSettings.Instance.useAdmobNative = false;
        }

        private void DrawAdmobMediationProperties()
        {
            EditorGUILayout.LabelField("Admob Mediation", styleHeader);
            DKTechIntegrateSettings.Instance.useAdmobMediationApplovin = EditorGUILayout.Toggle("Applovin", DKTechIntegrateSettings.Instance.useAdmobMediationApplovin, GUILayout.Width(15));
            DKTechIntegrateSettings.Instance.useAdmobMediationDTExchange = EditorGUILayout.Toggle("DT Exchange", DKTechIntegrateSettings.Instance.useAdmobMediationDTExchange, GUILayout.Width(15));
            DKTechIntegrateSettings.Instance.useAdmobMediationInMobi = EditorGUILayout.Toggle("InMobi", DKTechIntegrateSettings.Instance.useAdmobMediationInMobi, GUILayout.Width(15));
            DKTechIntegrateSettings.Instance.useAdmobMediationIronsource = EditorGUILayout.Toggle("Ironsource", DKTechIntegrateSettings.Instance.useAdmobMediationIronsource, GUILayout.Width(15));
            DKTechIntegrateSettings.Instance.useAdmobMediationLitOff = EditorGUILayout.Toggle("LitOffMonetize", DKTechIntegrateSettings.Instance.useAdmobMediationLitOff, GUILayout.Width(15));
            DKTechIntegrateSettings.Instance.useAdmobMediationMeta = EditorGUILayout.Toggle("Meta", DKTechIntegrateSettings.Instance.useAdmobMediationMeta, GUILayout.Width(15));
            DKTechIntegrateSettings.Instance.useAdmobMediationMintegral = EditorGUILayout.Toggle("Mintegral", DKTechIntegrateSettings.Instance.useAdmobMediationMintegral, GUILayout.Width(15));
            DKTechIntegrateSettings.Instance.useAdmobMediationPangle = EditorGUILayout.Toggle("Pangle", DKTechIntegrateSettings.Instance.useAdmobMediationPangle, GUILayout.Width(15));
            DKTechIntegrateSettings.Instance.useAdmobMediationUnityAds = EditorGUILayout.Toggle("Unity Ads", DKTechIntegrateSettings.Instance.useAdmobMediationUnityAds, GUILayout.Width(15));
            EditorGUILayout.Space(10);
        }

    }
}