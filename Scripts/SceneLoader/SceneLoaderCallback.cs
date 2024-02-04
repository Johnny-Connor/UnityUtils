using UnityEngine;
using UnityEngine.SceneManagement;

// Attach this script to a GameObject in the LoadingScene.
public class SceneLoaderCallback : MonoBehaviour
{
    private void Awake() => SceneLoader.SceneLoaderCallback();
}
