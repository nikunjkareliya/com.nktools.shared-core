/// <summary>
/// Scene Switcher Window - Part of NK Tools Shared Core Package
/// Provides quick scene switching from Build Settings
/// </summary>

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;

namespace Shared.Core.Editor
{
    public class SceneSwitcherWindow : EditorWindow
    {
        private Vector2 _scrollPos;
        private const float ButtonWidth = 200f;
        private const float ButtonHeight = 20f;
        private const float ButtonSpacing = 4f;

        [MenuItem("Tools/NK Tools/Scene Switcher")]
        public static void ShowWindow()
        {
            GetWindow<SceneSwitcherWindow>("Scene Switcher");
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label("Scenes in Build Settings", EditorStyles.boldLabel);
            GUILayout.Space(5);

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            var scenes = EditorBuildSettings.scenes;

            foreach (var sceneData in scenes.Select((s, i) => new { Scene = s, Index = i }))
            {
                string path = sceneData.Scene.path;
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);

                if (GUILayout.Button($"{sceneData.Index}. {sceneName}", GUILayout.Width(ButtonWidth), GUILayout.Height(ButtonHeight)))
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                    }
                }

                GUILayout.Space(ButtonSpacing);
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
