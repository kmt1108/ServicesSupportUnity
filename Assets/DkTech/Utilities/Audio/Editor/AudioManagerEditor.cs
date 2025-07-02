using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace Dktech.Services.Advertisement.Editor
{
    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            AudioManager audioManager = (AudioManager)target;
            if (GUILayout.Button("Update Audio Data"))
            {
                audioManager.SetMusicClip();
                UpdateEnum(audioManager.GetListAudioClip());
            }
        }
        private void UpdateEnum(List<AudioClip> listAudio)
        {
            string enumName = "AudioNames";
            string filePath = "Assets/DkTech/Utilities/Audio/AudioNames.cs";
            if (!File.Exists(filePath)){
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
                    for (int i = 0; i < listAudio.Count; i++)
                    {
                        var varName = listAudio[i].name.Replace(" ", "_").Replace("-", "_").Replace(".", "_").Replace("(", "").Replace(")", "");
                        if (char.IsDigit(varName[0]))
                        {
                            varName = "_" + varName; // Prefix with underscore if it starts with a digit
                        }
                        writer.WriteLine("public static string " + varName + " = \"" + listAudio[i].name + "\";");
                    }
                    writer.WriteLine("}");
                }
                Debug.Log($"DKTech SDK: Audio name is saved in {filePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"DKTech SDK: Save audio name failed: {e.StackTrace}");
            }
            AssetDatabase.Refresh();
        }
    }
}
