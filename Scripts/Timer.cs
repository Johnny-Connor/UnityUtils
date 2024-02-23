using System;
using UnityEngine;

public class Timer
{
    // Variables.
    private readonly Action _timerCallback;
    private bool _timerCallbackInvoked = true;
    
    
    // Properties.
    public float TimeLeft { get; private set; }


    // Constructor.
    public Timer(Action timerCallback = null) =>
        _timerCallback = timerCallback ?? (() => { /* Default action, do nothing. */ })
    ;


    // Non-MonoBehaviour.
    public void SetTimeLeft(float newTimeLeft, bool canOverrideOngoingTimeLeft = false)
    {
        if (TimeLeft > 0 && !canOverrideOngoingTimeLeft)
        {
            Debug.LogWarning(
                $"Cannot override time because {nameof(canOverrideOngoingTimeLeft)} was set to false. " +
                "Returning method."
            );
            return;
        }

        TimeLeft = newTimeLeft;
        _timerCallbackInvoked = false;
    }

    public void Update()
    {
        if (TimeLeft > 0) TimeLeft -= Time.deltaTime;
        else if (!_timerCallbackInvoked)
        {
            TimeLeft = 0;
            _timerCallback?.Invoke();
            _timerCallbackInvoked = true;
        }
    }
}
