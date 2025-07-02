using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Dktech.Services.Editor
{
    public class DKTechScenesController : EditorWindow
    {
        public static void ShowWindow()
        {
            var window = EditorWindow.GetWindow<DKTechScenesController>(utility: false, title: "Scenes Manager", focus: true);
            window.minSize = new Vector2(220, 220);
        }

        private Vector2 scrollPosition, sPActive, sPUnactive, spAll;

        private void OnEnable()
        {
            ShowScenesInBuild();
            if (!string.IsNullOrEmpty(DKTechIntegrateSettings.Instance.folderPath))
            {
                LoadScenesFromFolder(DKTechIntegrateSettings.Instance.folderPath);
            }
        }

        private void OnGUI()
        {
            checkNewList();

            DKTechIntegrateSettings.Instance.showActiveScenes = EditorGUILayout.Toggle("Show Active Scenes", DKTechIntegrateSettings.Instance.showActiveScenes);
            DKTechIntegrateSettings.Instance.showUnactiveScenes = EditorGUILayout.Toggle("Show Unactive Scenes", DKTechIntegrateSettings.Instance.showUnactiveScenes);
            DKTechIntegrateSettings.Instance.showAllSceneInProject = EditorGUILayout.Toggle("Show All Scene In Project", DKTechIntegrateSettings.Instance.showAllSceneInProject);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            if (DKTechIntegrateSettings.Instance.showActiveScenes)
            {
                #region Active Scene
                GUILayout.Space(22);
                GUI.contentColor = Color.green;
                GUILayout.Label("Open Active Scene: ", EditorStyles.helpBox, GUILayout.Width(440));
                GUI.contentColor = Color.white;
                sPActive = EditorGUILayout.BeginScrollView(sPActive, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                foreach (var sceneDes in ActiveScenesManager)
                {
                    GUILayout.Space(6);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(sceneDes.name, GUILayout.Width(220)))
                    {
                        OpenScene(sceneDes.path);
                    }
                    ScenesToBuildSettings(sceneDes, true);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                GUILayout.Space(22);
                #endregion
            }

            if (DKTechIntegrateSettings.Instance.showUnactiveScenes)
            {
                #region Unactive Scene
                GUI.contentColor = Color.red;
                GUILayout.Label("Open Unactive Scene: ", EditorStyles.helpBox, GUILayout.Width(440));
                GUI.contentColor = Color.white;
                sPUnactive = EditorGUILayout.BeginScrollView(sPUnactive, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                foreach (var sceneDes in UnactiveScenesManager)
                {
                    GUILayout.Space(6);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(sceneDes.name, GUILayout.Width(220)))
                    {
                        OpenScene(sceneDes.path);
                    }
                    ScenesToBuildSettings(sceneDes);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                GUILayout.Space(22);
                #endregion
            }

            if (DKTechIntegrateSettings.Instance.showAllSceneInProject)
            {
                #region  Scenes In Project
                GUILayout.Label("Scene Collector", EditorStyles.boldLabel, GUILayout.Width(440));
                EditorGUILayout.LabelField("Selected Folder:", DKTechIntegrateSettings.Instance.folderPath, GUILayout.Width(440));
                if (GUILayout.Button("Select Folder"))
                {
                    DKTechIntegrateSettings.Instance.folderPath = EditorUtility.OpenFolderPanel("Select Folder Containing Scenes", "", "");

                    if (!string.IsNullOrEmpty(DKTechIntegrateSettings.Instance.folderPath))
                    {
                        LoadScenesFromFolder(DKTechIntegrateSettings.Instance.folderPath);
                    }
                }
                GUILayout.Space(6);
                GUI.contentColor = Color.yellow;
                GUILayout.Label("Open Scene In Project: ", EditorStyles.helpBox, GUILayout.Width(440));
                GUI.contentColor = Color.white;
                spAll = EditorGUILayout.BeginScrollView(spAll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                foreach (var sceneDes in ScenesInProjectManager)
                {
                    GUILayout.Space(6);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(sceneDes.name, GUILayout.Width(220)))
                    {
                        OpenScene(sceneDes.path);
                    }
                    ScenesToBuildSettings(sceneDes);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                GUILayout.Space(22);
                #endregion
            }

            EditorGUILayout.EndScrollView();
        }

        private EditorBuildSettingsScene[] oldList;
        private void checkNewList()
        {
            if (oldList != EditorBuildSettings.scenes)
            {
                ShowScenesInBuild();
            }
        }

        private List<ScenesDes> ActiveScenesManager = new List<ScenesDes>();
        private List<ScenesDes> UnactiveScenesManager = new List<ScenesDes>();
        private void ShowScenesInBuild()
        {
            // Lấy danh sách các scenes từ Build Settings
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            oldList = scenes;
            ActiveScenesManager.Clear();
            UnactiveScenesManager.Clear();
            // Hiển thị danh sách trong Console

            foreach (var scene in scenes)
            {
                ScenesDes scenesDes = new ScenesDes();
                scenesDes.name = Path.GetFileNameWithoutExtension(scene.path);
                scenesDes.path = scene.path;
                if (scene.enabled)
                    ActiveScenesManager.Add(scenesDes);
                else
                    UnactiveScenesManager.Add(scenesDes);
            }

        }

        private List<ScenesDes> ScenesInProjectManager = new List<ScenesDes>();
        void LoadScenesFromFolder(string path)
        {
            ScenesInProjectManager.Clear();

            // Tìm tất cả các file .unity trong thư mục
            var files = Directory.GetFiles(path, "*.unity", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                ScenesDes scenesDes = new ScenesDes();
                scenesDes.name = Path.GetFileNameWithoutExtension(file);
                path = file.Replace(Application.dataPath, "Assets").Replace("\\", "/");
                scenesDes.path = path.StartsWith("/") ? path.Substring(1) : path;
                ScenesInProjectManager.Add(scenesDes);
            }
        }

        void ScenesToBuildSettings(ScenesDes scenesDes, bool show = false)
        {

            List<EditorBuildSettingsScene> newScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            EditorBuildSettingsScene scene = newScenes.Find(x => x.path == scenesDes.path);
            int index = newScenes.FindIndex(x => x.path == scenesDes.path);
            if (scene == null)
            {
                if (GUILayout.Button("\u002B", GUILayout.Width(22)))
                {
                    newScenes.Add(new EditorBuildSettingsScene(scenesDes.path, scenesDes.isActive));
                }
                scenesDes.isActive = EditorGUILayout.Toggle("", scenesDes.isActive, GUILayout.Width(22));
            }
            else
            {
                if (show)
                {
                    GUI.enabled = index > 0;
                    if (GUILayout.Button("\u2191", GUILayout.Width(22)))
                    {
                        var temp = newScenes[index];
                        newScenes[index] = newScenes[index - 1];
                        newScenes[index - 1] = temp;
                    }

                    GUI.enabled = index < newScenes.Count - 1;
                    if (GUILayout.Button("\u2193", GUILayout.Width(22)))
                    {
                        var temp = newScenes[index];
                        newScenes[index] = newScenes[index + 1];
                        newScenes[index + 1] = temp;
                    }

                    GUI.enabled = true;
                }
                if (GUILayout.Button("\u2212", GUILayout.Width(22)))
                {
                    newScenes.Remove(scene);
                }
                scene.enabled = EditorGUILayout.Toggle("", scene.enabled, GUILayout.Width(22));
            }
            EditorBuildSettings.scenes = newScenes.ToArray();
        }

        private void OpenScene(string scenePath)
        {
            // Xác nhận lưu scene hiện tại trước khi mở scene khác
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                // Mở scene mới
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                Debug.Log($"Opened scene: {scenePath}");
            }
            else
            {
                Debug.LogWarning("Scene opening canceled by user.");
            }
        }
    }
    [System.Serializable]
    public class ScenesDes
    {
        public string name;
        public string path;
        public bool isActive;
    }
}
