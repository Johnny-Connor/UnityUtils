using System;
using UnityEngine;

public class Timer
{
    // Variables.
    private Action _timerCallback;
    public float TimeLeft { get; private set; }


    // Constructor.
    public Timer(Action timerCallback = null) => 
        _timerCallback = timerCallback ?? (() => { /* Default action, do nothing. */ })
    ;


    // Non-MonoBehaviour.
    public void StartTimer(float countdownTime, bool canOverrideCurrentTimer = false)
    {
        if (TimeLeft > 0 && !canOverrideCurrentTimer)
        {
            Debug.LogWarning("Cannot override timer because 'canOverrideCurrentTimer' is set to false.");
            return;
        }

        TimeLeft = countdownTime;
    }

    public void UpdateTimer()
    {
        TimeLeft -= Time.deltaTime;

        if (TimeLeft <= 0)
        {
            TimeLeft = 0;
            _timerCallback();
        }
    }
}
