using System;
using UnityEngine;

public class Timer
{
    // Variables.
    private Action _timerCallback;

    private bool _isTimerRunning;
    public bool IsTimerRunning { get => _isTimerRunning; }

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
        if (_isTimerRunning && !canOverrideCurrentTimer)
        {
            Debug.LogWarning("Cannot override timer because 'canOverrideCurrentTimer' is set to false.");
            return;
        }

        _timeLeft = countdownTime;
        _isTimerRunning = true;
    }

    public void UpdateTimer()
    {
        if (_isTimerRunning)
        {
            _timeLeft -= Time.deltaTime;

            if (_timeLeft <= 0)
            {
                _isTimerRunning = false;
                _timeLeft = 0;
                _timerCallback();
            }
        }
    }
}
