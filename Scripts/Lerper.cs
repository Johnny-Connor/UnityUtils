using System;
using UnityEngine;

/*
A class that uses Mathf.Lerp the way it was intended to be used, guaranteeing a value transition over a
given time without slowing down exponentialy as it approaches the end, as often seen in many
implementations.
*/
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
    public void SetLerper(
        float initialValue, float endValue, float lerpDuration, bool canOverrideOnGoingLerpCycle = false
    )
    {
        if (_timeElapsed < _lerpDuration && !canOverrideOnGoingLerpCycle)
        {
            Debug.LogWarning("Cannot override Lerp cycle because 'canOverrideLerpCycle' was set to " + 
            "false. Returning method.");
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
