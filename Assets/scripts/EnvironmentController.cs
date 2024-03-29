﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Unity.MLAgents;

public class EnvironmentController
{
    static Lazy<EnvironmentController> s_Lazy = new Lazy<EnvironmentController>(() => new EnvironmentController());

    public static bool IsInitialized
    {
        get { return s_Lazy.IsValueCreated; }
    }

    public static EnvironmentController Instance { get { return s_Lazy.Value; } }

    private EnvironmentController()
    {
        //StartCoroutine(FeedBaby(1));
    }
    public static class GameScene
    {
        public static string Main = "MainScene";
        public static string PaperRod = "Paper_Rod_Experiment";
        public static string Fetus = "Fetus";
    }

    private string currentGameScene = GameScene.Main;

    public void ChangeGameScene(int scene)
    {
        //Debug.Log(SceneManager.GetActiveScene().name);
        if (scene == 3.0 && !SceneManager.GetActiveScene().name.ToLower().Equals(GameScene.Fetus.ToLower()))
        {
            //Academy.Instance.Dispose();
            SceneManager.LoadScene(GameScene.Fetus, LoadSceneMode.Single);
        }
        else if (scene == 2.0 && !SceneManager.GetActiveScene().name.ToLower().Equals(GameScene.PaperRod.ToLower()))
        {
            //Academy.Instance.Dispose();
            SceneManager.LoadScene(GameScene.PaperRod, LoadSceneMode.Single);
        }
        else if (!SceneManager.GetActiveScene().name.Equals(GameScene.Main))
        {
            //Academy.Instance.Dispose();
            SceneManager.LoadScene(GameScene.Main, LoadSceneMode.Single);
        }
    }

    public float gameStartDay;
    /// <summary>
    /// Time should be in 24 hour format and minutes should be fraction after the hour i.e.time = hour + minutes/60 </param>
    /// </summary>
    public float gameStartTime;

    private float gameCurrentDay;
    /// <summary>
    /// Time should be in 24 hour format and minutes should be fraction after the hour i.e.time = hour + minutes/60 </param>
    /// </summary>
    private float gameCurrentTime;

    private readonly float timeMultiplier = 24.0f / (60.0f * 2); // 24.0f / 86400;

    /// <summary>
    /// Time will be in 24 hour format and minutes will be fraction after the hour i.e.time = hour + minutes/60 </param>
    /// </summary>
    /// <returns>
    /// Current time.
    /// </returns>
    public float GetCurrentTime()
    {
        float curTime = (gameStartTime + (Time.time * timeMultiplier)) % 24;
        return curTime;

    }

    /// <summary>
    /// Time will be in 24 hour format and minutes will be fraction after the hour i.e.time = hour + minutes/60 </param>
    /// </summary>
    /// <returns>
    /// Current time.
    /// </returns>
    public int GetCurrentHour()
    {
        //return (gameStartTime + (Time.time * 24 / 86400)) % 24;
        float curTime = this.GetCurrentTime();
        return (int)Mathf.Floor(curTime);

    }

    /// <summary>
    /// Time will be in 24 hour format and minutes will be fraction after the hour i.e.time = hour + minutes/60 </param>
    /// </summary>
    /// <returns>
    /// Current time.
    /// </returns>
    public int GetCurrentMinutes()
    {
        float curTime = this.GetCurrentTime();
        return (int)((curTime * 60) % 60);

    }

    /// <summary>
    /// Time will be in 24 hour format and minutes will be fraction after the hour i.e.time = hour + minutes/60 </param>
    /// </summary>
    /// <returns>
    /// Current time.
    /// </returns>
    public int GetCurrentSeconds()
    {
        float curTime = this.GetCurrentTime();
        return (int)((curTime * 3600) % 60);

    }

    /// <summary>
    /// Returns current day of the game : starting day + day passed in game.
    /// </summary>
    /// <returns></returns>
    public float GetCurrentDay()
    {
        float currentTime = (gameStartTime + (Time.time * timeMultiplier));
        //Debug.Log("Cur time: " + currentTime + " Day2: " + currentTime / 24);
        return gameStartDay + currentTime / 24;

    }
}
