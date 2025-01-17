using UnityEngine;

/// <summary>
/// Allows control over the game's frame rate via the inspector.
/// </summary>
public class TargetFrameRateController : MonoBehaviour
{
    // Variables.
    [SerializeField] private int _targetFrameRate = -1;
    private int _previousTargetFrameRateValue;


    // MonoBehaviour Methods.
    private void Awake()
    {
        QualitySettings.vSyncCount = 0; // If vSyncCount != 0, targetFrameRate is ignored.
        Application.targetFrameRate = _targetFrameRate;
    }

    private void Update()
    {
        bool didTargetFrameRateUpdate = _targetFrameRate != _previousTargetFrameRateValue;

        if (didTargetFrameRateUpdate)
        {
            Application.targetFrameRate = _targetFrameRate;
            _previousTargetFrameRateValue = _targetFrameRate;
        }
    }
}
