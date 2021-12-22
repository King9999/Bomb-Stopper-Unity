using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/* Default time is dependent on difficulty and special conditions. */

public class Timer : MonoBehaviour
{
    public float time;           //time in seconds. Will use this value to get the minutes, seconds and milliseconds
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

        timerUI.text = DisplayTimer();
    }

    public void SetTimer(float seconds)
    {
        //timer will not exceed five minutes
        time = (seconds > 300) ? 300 : seconds;
    }

    public string DisplayTimer()
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        float milliseconds = time % 1 * 99;    //I only want the first two digits displayed so I'm not using 1000. Format wouldn't remove the extra digits.

        string timeText = string.Format("{0:0}:{1:00}:{2:00}", minutes, seconds, milliseconds);

        return timeText;
    }

    public bool TimeUp()
    {
        return time <= 0;
    }

}
