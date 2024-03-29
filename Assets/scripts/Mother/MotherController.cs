﻿using System.Collections;
using System.Timers;
using UnityEngine;
using System;
using DAIVID;

[RequireComponent(typeof(Mother))]
public class MotherController:MonoBehaviour
{
    private IEnumerator feedRoutine;

    private Timer motherScheduler;

    private Mother mother;

    static Lazy<MotherController> s_Lazy = new Lazy<MotherController>(() => new MotherController());

    private const long GREET_DURATION_SECONDS = 5;   // time in Seconds.

    private const long GREET_DURATION_MILLIS = GREET_DURATION_SECONDS * 1000;   // time in milliseconds.

    [SerializeField]
    private BabyAgent baby;

    public static bool IsInitialized
    {
        get { return s_Lazy.IsValueCreated; }
    }

    public static MotherController Instance { get { return s_Lazy.Value; } }

    private MotherController()
    {
    }

    void Start() {

        mother = GetComponent<Mother>();
        if (!mother)
        {
            Debug.LogWarning("Mother script not attached to the mother object.");
            return;
        }
        lastInterval = Time.time;
    }

    private float lastInterval;
    private void FixedUpdate()
    {

        float currentTime = EnvironmentController.Instance.GetCurrentHour();

        if (currentTime == 8 || currentTime == 12 || currentTime == 21)
            //if (currentTime % 3 ==0)
        {
            Debug.Log("currentTime: " + currentTime);

            if (!mother.isFeedingInProgress)
            {
                //mother.BringToy(ToyShowingCompleted);
                mother.Feed(FeedingCompleted);
            }
        }
        if (currentTime == 16)
        //if (currentTime % 3 ==0)
        {
            Debug.Log("currentTime: " + currentTime);

            if (!mother.isMotherBusy)
            {
                mother.BringToy(ToyShowingCompleted);
                //mother.Feed(FeedingCompleted);
            }
        }
        float timeNow = Time.time;
        if (timeNow > lastInterval + GREET_DURATION_SECONDS)
        {
            //mother.Greet();
            lastInterval = timeNow;
        }
    }
    private void ToyShowingCompleted()
    {
        Debug.Log("Toy display AnimCompleted");
    }
    private void FeedingCompleted()
    {
        if (baby)
        {
            baby.feed();
        }
        Debug.Log("Feeding AnimCompleted");
    }

    private void TimedGreetings(System.Object source, ElapsedEventArgs eventArg)
    {
        Debug.Log(eventArg.SignalTime);
        mother.Greet();
    }
}
