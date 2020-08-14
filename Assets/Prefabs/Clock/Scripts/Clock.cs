using UnityEngine;
using System.Collections;
using System;

public class Clock : MonoBehaviour
{

    //-- set start time 00:00
    public int hour = 0;
    public int minutes = 0;
    public int seconds = 0;
    public int days = 0;
    public bool realTime = false;
    public bool selfTimeUpdate = false;

    public GameObject pointerSeconds;
    public GameObject pointerMinutes;
    public GameObject pointerHours;
    public GameObject digitalText;

    //-- time speed factor
    public float clockSpeed = 1.0f;     // 1.0f = realtime, < 1.0f = slower, > 1.0f = faster

    //-- internal vars
    float msecs = 0;

    void Start()
    {
        //-- set real time
        if (realTime)
        {
            hour = System.DateTime.Now.Hour;
            minutes = System.DateTime.Now.Minute;
            seconds = System.DateTime.Now.Second;
            days = System.DateTime.Now.Day;
        }
        else
        {
            GetEnvironmentTime();
        }
        UpdateText();
    }

    private void GetEnvironmentTime()
    {
        hour = EnvironmentController.Instance.GetCurrentHour();
        minutes = EnvironmentController.Instance.GetCurrentMinutes();
        seconds = EnvironmentController.Instance.GetCurrentSeconds();
        days = (int)EnvironmentController.Instance.GetCurrentDay();
    }

    private void UpdateText()
    {
        if(digitalText == null)
        {
            Debug.LogWarning("Text for clock not attached");
            return;
        }
        string text = hour.ToString("D2") + " : " + minutes.ToString("D2") + " : " + seconds.ToString("D2") + "\n"+"Day : "+ days.ToString("D2");
        digitalText.GetComponent<TextMesh>().text = text;
    }

    void Update()
    {
        //-- calculate time
        if (selfTimeUpdate)
        {
            msecs += Time.deltaTime * clockSpeed;
            if (msecs >= 1.0f)
            {
                msecs -= 1.0f;
                seconds++;
                if (seconds >= 60)
                {
                    seconds = 0;
                    minutes++;
                    if (minutes > 60)
                    {
                        minutes = 0;
                        hour++;
                        if (hour >= 24)
                            hour = 0;
                    }
                }
            }
        }
        else
        {
            GetEnvironmentTime();
        }
        UpdateText();



        //-- calculate pointer angles
        float rotationSeconds = (360.0f / 60.0f) * seconds;
        float rotationMinutes = (360.0f / 60.0f) * minutes;
        float rotationHours = ((360.0f / 12.0f) * hour) + ((360.0f / (60.0f * 12.0f)) * minutes);

        //-- draw pointers
        pointerSeconds.transform.localEulerAngles = new Vector3(0.0f, 0.0f, rotationSeconds);
        pointerMinutes.transform.localEulerAngles = new Vector3(0.0f, 0.0f, rotationMinutes);
        pointerHours.transform.localEulerAngles = new Vector3(0.0f, 0.0f, rotationHours);

    }
}
