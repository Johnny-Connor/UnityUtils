using UnityEngine;

/// <summary>
/// Handles transitioning to the <see cref="SceneLoader._loadingSceneTargetScene"/>. Should be attached 
/// to a GameObject in a loading scene.
/// </summary>
public class SceneLoaderCallback : MonoBehaviour
{
    private void Awake() => SceneLoader.SceneLoaderCallback();
}
