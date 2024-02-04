using UnityEngine;
using UnityEngine.SceneManagement;

// A scene loader based on CodeMonkey's Loader.cs from his Kitchen Chaos Game Course.
public static class SceneLoader
{
    // Variables.

    // Elements must match scene names.
    public enum Scene
    {
        Scene1,
        Scene2,
        LoadingScene
    }

    // The scene the loading scene will load.
    private static Scene _loadingSceneTargetScene;


    // Non-MonoBehaviour.
    public static void Load(Scene newTargetScene)
    {
        _loadingSceneTargetScene = newTargetScene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void SceneLoaderCallback()
    {
        if (SceneManager.GetActiveScene().name == Scene.LoadingScene.ToString())
        {
            SceneManager.LoadScene(_loadingSceneTargetScene.ToString());
        }
        else
        {
            Debug.LogError($"The method {nameof(SceneLoaderCallback)} should only be called from the " +
            $"{nameof(Scene.LoadingScene)} scene.");
        }
    }
}
