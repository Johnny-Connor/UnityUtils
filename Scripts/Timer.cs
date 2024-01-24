using System;
using UnityEngine;

public class Timer
{
    // Variables.
    private readonly Action _timerCallback;
    private bool _callbackInvoked;
    
    
    // Properties.
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
        _callbackInvoked = false;
    }

    public void UpdateTimer()
    {
        if (TimeLeft > 0) TimeLeft -= Time.deltaTime;
        else if (!_callbackInvoked)
        {
            TimeLeft = 0;
            _timerCallback?.Invoke();
            _callbackInvoked = true;
        }
    }
}
