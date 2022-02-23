using System;
using UnityEngine;

public class Timer :MonoBehaviour
{
    private float time_left = 0f;
    private float start_time = 0f;
    private bool one_shot = false;
    private bool timer_isRunning = false;
    //bool paused = false;

    public float Time_Left
    {
        get
        {
            return time_left;
        }
    }

    public bool One_Shot
    {
        get
        {
            return one_shot;
        }
        set
        {
            one_shot = value;
        }
    }

    public bool Timer_isRunning
    {
        get
        {
            return timer_isRunning;
        }
    }

    public void StartTimer(float time,bool oneShot)
    {
        start_time = time;
        time_left = start_time;
        one_shot = oneShot;
        timer_isRunning = true;
    }

    private void Update()
    {
        if (timer_isRunning)
        {
            CalculateTime();
        }
    }

    private void CalculateTime()
    {
        if (time_left > 0)
        {
            time_left -= Time.deltaTime;
        }
        else
        {
            if (one_shot == false)
            {
                StartTimer(start_time, true);
                Debug.Log("<color=#a52a2aff>Timer restart</color>");
            }
            else
            {
                time_left = 0;
                timer_isRunning = false;
                Debug.Log("<color=#dc143cff>Timer finished</color>");
            }
        }
    }

    public void Stop()
    {
        time_left = 0;
        timer_isRunning = false;
        Debug.Log("<color=#8b0000ff>Timer stopped</color>");
    }
}
