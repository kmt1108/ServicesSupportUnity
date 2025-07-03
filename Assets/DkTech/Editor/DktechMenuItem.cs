using UnityEditor;
using UnityEngine;
using UnityEditor.ShortcutManagement;
using Dktech.Services.Firebase;
using Dktech.Services.Advertisement;

namespace Dktech.Services.Editor
{
    public class DktechMenuItem
    {
        [MenuItem("DKTech/Integration Manager",priority = 1)]
        private static void IntegrationManager()
        {
            ShowIntegrationManager();
        }
        [MenuItem("DKTech/Firebase Settings",priority = 2)]
        private static void OpenFirebaseSettings()
        {
            Selection.activeObject = FirebaseSettings.LoadInstance();
        }
        
        [MenuItem("DKTech/Advertisement Settings", priority = 3)]
        private static void OpenAdvertisementSettings()
        {
            Selection.activeObject = AdvertisementSettings.LoadInstance();
        }

        [MenuItem("DKTech/DKTech Tool/Scene Manager",priority = 4)]
        private static void SceneController()
        {
            ShowScenesManager();
        }

        [MenuItem("DKTech/Documentation",priority = 5)]
        private static void Documentation()
        {
            Application.OpenURL("https://github.com/kmt1108/ServicesSupportUnity/blob/main/README.md");
        }

        private static void ShowIntegrationManager()
        {
            DKTechIntegrationManagerWindow.ShowWindow();
        }

        private static void ShowScenesManager()
        {
            DKTechScenesController.ShowWindow();
        }

    }
}