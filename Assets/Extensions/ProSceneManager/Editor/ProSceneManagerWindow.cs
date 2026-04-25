using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProSceneManager
{
    public class ProSceneManagerWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private string searchQuery = "";
        private Dictionary<string, List<string>> categorizedScenes = new Dictionary<string, List<string>>();
        private Dictionary<string, bool> categoryFoldouts = new Dictionary<string, bool>();
        private Dictionary<string, string> sceneNotes = new Dictionary<string, string>();
        private Dictionary<string, bool> additiveLoadFlags = new Dictionary<string, bool>();

        [MenuItem("Tools/Pro Scene Manager")]
        public static void ShowWindow()
        {
            GetWindow<ProSceneManagerWindow>("Pro Scene Manager");
        }

        private void OnEnable()
        {
            LoadScenes();
        }

        private void LoadScenes()
        {
            categorizedScenes.Clear();
            string[] scenePaths = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories);

            foreach (var path in scenePaths)
            {
                string sceneName = Path.GetFileNameWithoutExtension(path);
                string parentFolder = Path.GetDirectoryName(path);
                string category = string.IsNullOrEmpty(parentFolder) ? "Uncategorized" : Path.GetFileName(parentFolder);

                if (!categorizedScenes.ContainsKey(category))
                {
                    categorizedScenes[category] = new List<string>();
                    categoryFoldouts[category] = true;
                }

                categorizedScenes[category].Add(path);
                if (!sceneNotes.ContainsKey(path)) sceneNotes[path] = "";
                if (!additiveLoadFlags.ContainsKey(path)) additiveLoadFlags[path] = false;
            }
        }

        private void OnGUI()
        {
            // 🔹 Stylish Header
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };

            EditorGUILayout.Space();
            GUIContent titleIcon = EditorGUIUtility.IconContent("d_BuildSettings.SelectedIcon");
            titleIcon.text = " Pro Scene Manager";
            EditorGUILayout.LabelField(titleIcon, headerStyle);
            EditorGUILayout.Space();

            // 🔹 Search Bar
            GUIContent searchIcon = EditorGUIUtility.IconContent("d_SearchWindow");
            searchIcon.text = " Search Scenes";
            searchQuery = EditorGUILayout.TextField(searchIcon, searchQuery).ToLower().Trim(); // Convert to lowercase for case-insensitive search
            EditorGUILayout.Space();

            // 🔹 Refresh Button
            GUIStyle refreshStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };

            GUIContent refreshIcon = EditorGUIUtility.IconContent("Refresh");
            refreshIcon.text = " Refresh Scenes";
            if (GUILayout.Button(refreshIcon, refreshStyle, GUILayout.Height(30)))
            {
                LoadScenes();
            }

            GUILayout.Space(10);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            foreach (var category in categorizedScenes.Keys.OrderBy(c => c))
            {
                // 🔎 Filter Out Categories Without Matching Scenes
                List<string> filteredScenes = categorizedScenes[category]
                    .Where(scenePath => string.IsNullOrEmpty(searchQuery) || Path.GetFileNameWithoutExtension(scenePath).ToLower().Contains(searchQuery))
                    .ToList();

                if (filteredScenes.Count == 0)
                    continue; // Skip category if no scenes match search query

                GUIContent folderIcon = EditorGUIUtility.IconContent("Folder Icon");
                folderIcon.text = $" {category}";

                categoryFoldouts[category] = EditorGUILayout.Foldout(categoryFoldouts[category], folderIcon, true, EditorStyles.foldoutHeader);

                if (categoryFoldouts[category])
                {
                    foreach (var scenePath in filteredScenes)
                    {
                        string sceneName = Path.GetFileNameWithoutExtension(scenePath);
                        Object sceneAsset = AssetDatabase.LoadAssetAtPath<Object>(scenePath);
                        bool isSceneLoaded = IsSceneCurrentlyLoaded(scenePath);

                        // 🔹 Scene Row Style
                        GUIStyle rowStyle = new GUIStyle(EditorStyles.helpBox)
                        {
                            padding = new RectOffset(10, 10, 5, 5),
                            normal = { background = MakeTex(1, 1, isSceneLoaded ? new Color(0.2f, 0.6f, 0.2f, 0.2f) : new Color(0, 0, 0, 0.1f)) }
                        };

                        GUILayout.BeginHorizontal(rowStyle);

                        // 🔹 Green Dot for Loaded Scenes
                        GUIStyle dotStyle = new GUIStyle
                        {
                            normal = { textColor = Color.green },
                            alignment = TextAnchor.MiddleCenter,
                            padding = new RectOffset(10, 0, 1, 0)
                        };

                        GUILayout.Label(isSceneLoaded ? "●" : "", dotStyle, GUILayout.Width(15));

                        // 🔹 Scene Name
                        GUIContent sceneIcon = EditorGUIUtility.IconContent("d_BuildSettings.SelectedIcon");
                        sceneIcon.text = $" {sceneName}";
                        GUILayout.Label(sceneIcon, GUILayout.Width(150));

                        // 🔹 Additive Load Checkbox
                        additiveLoadFlags[scenePath] = GUILayout.Toggle(additiveLoadFlags[scenePath], "Additive", GUILayout.Width(80));

                        // 🔹 Load Button
                        GUIStyle loadButtonStyle = new GUIStyle(GUI.skin.button);
                        if (GUILayout.Button("▶ Load", loadButtonStyle, GUILayout.ExpandWidth(true)))
                        {
                            OpenScene(scenePath, additiveLoadFlags[scenePath]);
                        }

                        // 🔹 Notes Button (Yellow if Contains Notes)
                        GUIStyle notesButtonStyle = new GUIStyle(GUI.skin.button);
                        if (!string.IsNullOrEmpty(sceneNotes[scenePath]))
                        {
                            notesButtonStyle.fontStyle = FontStyle.Bold;
                            notesButtonStyle.normal.textColor = Color.yellow;
                        }

                        GUIContent notesIcon = EditorGUIUtility.IconContent("d_UnityEditor.FindDependencies");
                        notesIcon.text = " Notes";

                        if (GUILayout.Button(notesIcon, notesButtonStyle, GUILayout.ExpandWidth(true)))
                        {
                            NotesWindow.OpenWindow(sceneName, scenePath, sceneNotes);
                        }

                        // 🔹 Select Scene Button
                        GUIContent selectIcon = EditorGUIUtility.IconContent("d_scenepicking_pickable-mixed");
                        selectIcon.text = " Select";

                        if (GUILayout.Button(selectIcon, GUILayout.Width(60)))
                        {
                            Selection.activeObject = sceneAsset;
                            EditorGUIUtility.PingObject(sceneAsset);
                        }

                        GUILayout.EndHorizontal();
                        GUILayout.Space(5);
                    }
                }
            }

            GUILayout.EndScrollView();
        }

        private void OpenScene(string scenePath, bool additive)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                if (additive)
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                else
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            }
        }

        private bool IsSceneCurrentlyLoaded(string scenePath)
        {
            scenePath = scenePath.Replace("\\", "/");
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                Scene scene = EditorSceneManager.GetSceneAt(i);
                if (scene.path.Replace("\\", "/") == scenePath)
                    return true;
            }
            return false;
        }

        // 🔹 Helper to Create Background Textures
        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}