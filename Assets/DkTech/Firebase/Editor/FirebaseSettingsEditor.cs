#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Dktech.Services.Firebase.Editor
{
    [CustomEditor(typeof(FirebaseSettings))]
    public class FirebaseSettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            FirebaseSettings firebaseManager = (FirebaseSettings)target;

            if (GUILayout.Button("Update Firebase Keys"))
            {
                UpdateEnum(firebaseManager);
            }
        }
        string adName;
        private void UpdateEnum(FirebaseSettings firebaseManager)
        {
            string enumName = "RemoteKeys";
            string filePath = "Assets/DkTech/Firebase/RemoteKey.cs"; // Path to the file where you want to save the enum
            if (!File.Exists(filePath))
            {
                //Create the directory if it doesn't exist
                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("public class " + enumName);
                    writer.WriteLine("{");
                    for (int i = 0; i < firebaseManager.listRequest.Count; i++)
                    {
                        writer.WriteLine("public static string " + firebaseManager.listRequest[i].name + " = \"" + (firebaseManager.listRequest[i].key != "" ? firebaseManager.listRequest[i].key : firebaseManager.listRequest[i].name) + "\";");
                    }
                    writer.WriteLine("}");
                }
                Debug.Log($"DKTech SDK: Remote config is saved in {filePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"DKTech SDK: Save remote config data failed: {e.StackTrace}");
            }
            AssetDatabase.Refresh();
        }
    }
}
#endif