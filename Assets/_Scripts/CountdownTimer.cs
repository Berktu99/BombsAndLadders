using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownTimer 
{
    private float time;
    public float timer;

    public CountdownTimer(float _time)
    {
        time = _time;
        timer = _time;
    }

    public bool Tick(float t)
    {
        timer -= t;

        if (timer <= 0)
        {
            timer = time;
            return true;
        }

        return false;
    }
}
