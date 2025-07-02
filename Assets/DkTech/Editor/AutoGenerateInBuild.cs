#if UNITY_ANDROID && UNITY_EDITOR
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class PluginManifestPreprocessor : IPreprocessBuildWithReport
{
    // Thứ tự chạy callback (muốn chạy sớm thì để 0)
    public int callbackOrder => 0;

    // Gọi trước khi bắt đầu build (APK/AAB)
    public void OnPreprocessBuild(BuildReport report)
    {
#if admob_enabled || firebase_enabled
        string manifestPath = Path.Combine(
             Application.dataPath, "Plugins", "Android", "AndroidManifest.xml"
         );
        if (!File.Exists(manifestPath)) return;

        var xmlDoc = new XmlDocument();
        xmlDoc.Load(manifestPath);

        var androidNs = "http://schemas.android.com/apk/res/android";
        XmlNode manifestNode = xmlDoc.SelectSingleNode("/manifest");
        XmlNode appNode = xmlDoc.SelectSingleNode("/manifest/application");
        if (manifestNode == null || appNode == null)
        {
            return;
        }

        // Tạo element <uses-permission>
        bool already = false;
        foreach (XmlNode child in manifestNode.ChildNodes)
        {
            if (child.Name == "uses-permission"
                && ((XmlElement)child).GetAttribute("name", androidNs) == "android.permission.VIBRATE")
            {
                already = true;
                break;
            }
        }
        if (!already)
        {
            XmlElement perm = xmlDoc.CreateElement("uses-permission");
            perm.SetAttribute("name", androidNs, "android.permission.VIBRATE");
            // Chèn ngay sau node <application>
            manifestNode.InsertAfter(perm, appNode);

            xmlDoc.Save(manifestPath);
            AssetDatabase.Refresh();
        }
#endif
    }
}
#endif
