#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class PostBuildSystemFramework
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget != BuildTarget.iOS)
            return;

        // Path to the Xcode project
        string projPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);

        // Load the project
        PBXProject proj = new PBXProject();
        proj.ReadFromFile(projPath);

        // Get target GUIDs
#if UNITY_2019_3_OR_NEWER
        string targetGuid = proj.GetUnityMainTargetGuid();
        string frameworkTargetGuid = proj.GetUnityFrameworkTargetGuid();
#else
        string targetGuid = proj.TargetGuidByName("Unity-iPhone");
        string frameworkTargetGuid = targetGuid;
#endif

        // Add SystemConfiguration.framework (non-optional)
        proj.AddFrameworkToProject(frameworkTargetGuid, "SystemConfiguration.framework", false);

        // Save the project
        proj.WriteToFile(projPath);
    }
}
#endif