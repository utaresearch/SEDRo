using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.MLAgents;
using TinyJson;
public class ExternalCommunicator : MonoBehaviour
{
    static Lazy<ExternalCommunicator> s_Lazy = new Lazy<ExternalCommunicator>(() => new ExternalCommunicator());

    private EnvironmentParameters environmentParams;

    public static bool IsInitialized
    {
        get { return s_Lazy.IsValueCreated; }
    }

    public static ExternalCommunicator Instance { get { return s_Lazy.Value; } }

    private ExternalCommunicator()
    {
        //StartCoroutine(FeedBaby(1));
    }

    static class MessageParamas
    {
        public static string gameDay = "game_day";
        public static string gameTimeOfDay = "game_time_of_day";
        public static string sceneToLoad = "scene_to_load";
        public static string agentConfig = "agent_config";
    }

    private void Awake()
    {
        environmentParams = Academy.Instance.EnvironmentParameters;
        setEnvironmentParams();
    }

    private void setEnvironmentParams()
    {
        float scene = environmentParams.GetWithDefault(MessageParamas.sceneToLoad, 0);
        float day = environmentParams.GetWithDefault(MessageParamas.gameDay, 0);
        float time = environmentParams.GetWithDefault(MessageParamas.gameTimeOfDay, 000);
        Debug.Log("Time of day set from python: " + time);

        time = (float)(Math.Floor(time / 100f)+ time % 100 / 60);

        EnvironmentController.Instance.gameStartDay = day;
        EnvironmentController.Instance.gameStartTime = time;
        Debug.Log("Time of day: " + time);
    }
}
