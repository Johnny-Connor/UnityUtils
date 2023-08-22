using System;
using UnityEngine;

public class Timer
{
    // Variables.
    private Action _timerCallback;
    
    private float _timeLeft;
    public float TimeLeft { get => _timeLeft; }


    // Constructor.
    public Timer(Action timerCallback = null)
    {
        _timerCallback = timerCallback;
    }


    // Non-MonoBehaviour.
    public void StartTimer(float countdownTime, bool canOverrideCurrentTimer = false)
    {
        if (_timeLeft > 0 && !canOverrideCurrentTimer)
        {
            Debug.LogWarning("Cannot override timer because 'canOverrideCurrentTimer' is set to false.");
            return;
        }

        _timeLeft = countdownTime;
    }

    public void UpdateTimer()
    {
        if (_timeLeft > 0)
        {
            _timeLeft -= Time.deltaTime;

            if (_timeLeft <= 0)
            {
                _timeLeft = 0;
                _timerCallback();
            }
        }
    }
}
