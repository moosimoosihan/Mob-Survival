using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class PlayModeSceneSwitch
{
    // private static string lastScene;
    // private const string introScene = "Assets/00_Scene/Intro.unity"; // Update this to match your Intro scene path

    // static PlayModeSceneSwitch()
    // {
    //     EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    //     EditorSceneManager.sceneOpened += OnSceneOpened;
    // }

    // private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
    // {
    //     if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
    //     {
    //         lastScene = scene.path;
    //     }
    // }

    // private static void OnPlayModeStateChanged(PlayModeStateChange state)
    // {
    //     if (state == PlayModeStateChange.ExitingEditMode)
    //     {
    //         if (lastScene != introScene)
    //         {
    //             EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
    //             EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(introScene);
    //         }
    //     }
    // }
}