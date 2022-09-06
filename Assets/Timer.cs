using System;
using UnityEngine;

public class Timer : MonoBehaviour
{

    private Action _timerCallBack = null;
    private float _timeLeft = 0;

    private void Update() {
        Countdown();
    }

    private void Countdown(){
        if (_timeLeft > 0){
            _timeLeft -= Time.deltaTime;
            if (_timeLeft <= 0){
                _timerCallBack();
            }
        }
    }

    public void SetTimer(float timerDuration, Action callBackMethod){
        _timeLeft = timerDuration;
        _timerCallBack = callBackMethod;
    }

}