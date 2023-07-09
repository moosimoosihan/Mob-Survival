using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace olimsko
{
    [InitializeOnLoad]
    public class EngineConfigurationSetting : ConfigurationSettingsProvider<EngineConfiguration>
    {
        private static string lastScene;
        // private const string introScene = "Assets/00_Scene/Intro.unity"; // Update this to match your Intro scene path

        static EngineConfigurationSetting()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorSceneManager.sceneOpened += OnSceneOpened;
        }

        private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                lastScene = scene.path;
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                if (ConfigurationProvider.LoadOrDefault<EngineConfiguration>().UseForceStartScene)
                {
                    if (lastScene != ConfigurationProvider.LoadOrDefault<EngineConfiguration>().StartScenePath)
                    {
                        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                        EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(ConfigurationProvider.LoadOrDefault<EngineConfiguration>().StartScenePath);
                    }
                }
                else
                {
                    EditorSceneManager.playModeStartScene = null;
                }
            }
        }
    }

}

