using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/* Default time is dependent on difficulty and special conditions. */

public class Timer : MonoBehaviour
{
    public float time;           //time in seconds. Will use this value to get the minutes, seconds and milliseconds
    public float initTime;      //the initial start time. Used to get time elapsed.
    public bool timerRunning;

    //UI
    public TextMeshProUGUI timerUI;

    // Start is called before the first frame update
    void Start()
    {
        //timerRunning = false;
       // time = 120;             //default time is 2 minutes.
    }

    // Update is called once per frame
    void Update()
    {
        //timer is displayed as a string so that extra zeroes can be added when necessary to display correct time
        if (timerRunning)
        {
            time -= Time.deltaTime;

            if (time < 0)
            {
                time = 0;
                timerRunning = false;
            }
          
        }

        //update timer display
        if (time <= 10)
            timerUI.color = Color.red;
        else
            timerUI.color = Color.white;

        timerUI.text = DisplayTimer();
    }

    public void SetTimer(float seconds)
    {
        //timer will not exceed five minutes
        time = (seconds > 300) ? 300 : seconds;
        initTime = time;
    }

    public string DisplayTimer()
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        float milliseconds = time % 1 * 99;    //I only want the first two digits displayed so I'm not using 1000. Format wouldn't remove the extra digits.

        string timeText = string.Format("{0:0}:{1:00}:{2:00}", minutes, seconds, milliseconds);

        return timeText;
    }

    public string DisplayElapsedTime()
    {
        float elapsedTime = initTime - time;
        float minutes = Mathf.FloorToInt(elapsedTime / 60);
        float seconds = Mathf.FloorToInt(elapsedTime % 60);

        string timeText = string.Format("{0:0}:{1:00}", minutes, seconds);

        return timeText;
    }

    public bool TimeUp()
    {
        return time <= 0;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public void StartTimer()
    {
        timerRunning = true;
    }

}
