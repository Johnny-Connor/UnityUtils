using System;
using UnityEngine;

/// <summary>
/// Utilizes <see cref="Mathf.Lerp"/> as intended, ensuring smooth value transitions over a specified 
/// duration without the exponential slow-down near the end seen in many implementations. Supports 
/// callback and cycle override control.
/// </summary>
public class Lerper
{
    // Variables.
    private Action _lerperCallback;
    private float _initialValue;
    private float _endValue;
    private float _lerpDuration;
    private float _timeElapsed = Mathf.Infinity;


    // Constructor.
    public Lerper(Action lerperCallback = null) =>
        _lerperCallback = lerperCallback ?? (() => { /* Default action, do nothing. */ })
    ;


    // Methods.
    public void SetCycle(
        float initialValue, float endValue, float lerpDuration, bool canOverrideOngoingCycle = false
    )
    {
        if (_timeElapsed < _lerpDuration && !canOverrideOngoingCycle)
        {
            Debug.LogWarning(
                $"Cannot override cycle because {nameof(canOverrideOngoingCycle)} was set to false. " +
                "Returning method."
            );
            return;
        }

        _initialValue = initialValue;
        _endValue = endValue;
        _lerpDuration = lerpDuration;

        _timeElapsed = 0;
    }

    public void Update(ref float lerpValue)
    {
        if (_timeElapsed < _lerpDuration)
        {
            lerpValue = Mathf.Lerp(_initialValue, _endValue, _timeElapsed / _lerpDuration);
            _timeElapsed += Time.deltaTime;

            if (_timeElapsed >= _lerpDuration)
            {
                lerpValue = _endValue;
                _lerperCallback();
            }
        }
    }
}
