using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MotherController:MonoBehaviour
{
    private IEnumerator feedRoutine;

    static Lazy<MotherController> s_Lazy = new Lazy<MotherController>(() => new MotherController());


    public static bool IsInitialized
    {
        get { return s_Lazy.IsValueCreated; }
    }

    public static MotherController Instance { get { return s_Lazy.Value; } }

    private MotherController()
    {
        StartCoroutine(FeedBaby(1));
    }

    IEnumerator FeedBaby(float time)
    {
        yield return null;
    }
}
