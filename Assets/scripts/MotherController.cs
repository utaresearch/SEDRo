using System.Collections;
using System.Timers;
using UnityEngine;
using System;

public class MotherController:MonoBehaviour
{
    private IEnumerator feedRoutine;

    private Timer motherScheduler;

    private Mother mother;

    static Lazy<MotherController> s_Lazy = new Lazy<MotherController>(() => new MotherController());

    private const long GREET_DURATION_SECONDS = 5;   // time in Seconds.

    private const long GREET_DURATION_MILLIS = GREET_DURATION_SECONDS * 1000;   // time in milliseconds.



    public static bool IsInitialized
    {
        get { return s_Lazy.IsValueCreated; }
    }

    public static MotherController Instance { get { return s_Lazy.Value; } }

    private MotherController()
    {
        //StartCoroutine(FeedBaby(1));
    }

    void Start() {

        mother = GetComponent<Mother>();
        if (!mother)
        {
            Debug.LogWarning("Mother script not attached to the mother object.");
            return;
        }
        lastInterval = Time.time;
        //PrepareTimer();
    }
    private float lastInterval;
    private void FixedUpdate()
    {

        float currentTime = EnvironmentController.Instance.GetCurrentHour();
        
        if (currentTime == 8 || currentTime == 12 || currentTime == 18)
        {
            Debug.Log("currentTime: " + currentTime);
            mother.Feed(FeedingCompleted);
        }

        float timeNow = Time.time;
        if (timeNow > lastInterval + GREET_DURATION_SECONDS)
        {
            //mother.Greet();
            lastInterval = timeNow;
        }
    }

    private void FeedingCompleted()
    {
        Debug.Log("FeedingCompleted");
    }

    IEnumerator FeedBaby(float time)
    {
        yield return null;
    }

    private void PrepareTimer()
    {
        motherScheduler = new Timer(GREET_DURATION_MILLIS);
        motherScheduler.AutoReset = true;
        motherScheduler.Enabled = true;
        motherScheduler.Elapsed += TimedGreetings;
    }

    private void TimedGreetings(System.Object source, ElapsedEventArgs eventArg)
    {
        Debug.Log(eventArg.SignalTime);
        mother.Greet();
    }

    void OnDestroy()
    {
        //motherScheduler.Stop();
        //motherScheduler.Dispose();
    }
}
