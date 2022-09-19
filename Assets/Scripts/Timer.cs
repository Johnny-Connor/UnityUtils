using System;
using UnityEngine;

public class Timer : MonoBehaviour
{

    private Action _timerCallback;
    private float _timeLeft;

    private void Update()
    {

        void Countdown()
        {
            if (_timeLeft > 0)
            {
                _timeLeft -= Time.deltaTime;
                if (_timeLeft <= 0)
                {
                    _timerCallback();
                }
            }
        }

        Countdown();

    }

    public float GetTimeLeft()
    {
        return _timeLeft;
    }

    /*
    Sets a new value for the timer and receives a callback method to use when the timer
    reaches 0 in the Countdown() method.
    */
    public void SetTimer(float timerDuration, Action callbackMethod)
    {
        _timeLeft = timerDuration;
        _timerCallback = callbackMethod;
    }

}