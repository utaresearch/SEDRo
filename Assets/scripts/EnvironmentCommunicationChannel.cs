using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.SideChannels;
using System.Text;
using System;
using System.Collections.Generic;
using TinyJson;

namespace DAIVID
{
    static public class CommMessageKeys
    {
        public static string envConfig = "env_config";
        public static string bodyConfig = "body_config";
        public static string sceneToLoad = "scene_to_load";
        public static string gameDay = "game_day";
        public static string gameTimeOfDay = "game_time_of_day";

        // body config keys
        public static string hipsX = "hips_x";
        public static string hipsY = "hips_y";
        public static string hipsZ = "hips_z";
        public static string chestX = "chest_x";
        public static string chestY = "chest_y";
        public static string chestZ = "chest_z";
        public static string spineX = "spine_x";
        public static string spineY = "spine_y";
        public static string spineZ = "spine_z";
        public static string headX = "head_x";
        public static string headY = "head_y";
        public static string headZ = "head_z";
        public static string thighX = "thigh_x";
        public static string thighY = "thigh_y";
        public static string thighZ = "thigh_z";
        public static string shinX = "shin_x";
        public static string shinY = "shin_y";
        public static string shinZ = "shin_z";
        public static string footX = "foot_x";
        public static string footY = "foot_z";
        public static string footZ = "foot_y";
        //public static string thighLX = "thighL_x";
        //public static string thighLY = "thighL_y";
        //public static string thighLZ = "thighL_z";
        //public static string shinLX = "shinL_x";
        //public static string shinLY = "shinL_y";
        //public static string shinLZ = "shinL_z";
        //public static string footLX = "footL_x";
        //public static string footLY = "footL_z";
        //public static string footLZ = "footL_y";
        //public static string thighRX = "thighR_x";
        //public static string thighRY = "thighR_y";
        //public static string thighRZ = "thighR_z";
        //public static string shinRX = "shinR_x";
        //public static string shinRY = "shinR_y";
        //public static string shinRZ = "shinR_z";
        //public static string footRX = "footR_x";
        //public static string footRY = "footR_y";
        //public static string footRZ = "footR_z";
        public static string upperArmX = "upperArm_x";
        public static string upperArmY = "upperArm_y";
        public static string upperArmZ = "upperArm_z";
        public static string lowerArmX = "lowerArm_x";
        public static string lowerArmY = "lowerArm_y";
        public static string lowerArmZ = "lowerArm_z";
        public static string handX = "hand_x";
        public static string handY = "hand_y";
        public static string handZ = "hand_z";
        //public static string upperArmLX = "upperArmL_x";
        //public static string upperArmLY = "upperArmL_y";
        //public static string upperArmLZ = "upperArmL_z";
        //public static string lowerArmLX = "lowerArmL_x";
        //public static string lowerArmLY = "lowerArmL_y";
        //public static string lowerArmLZ = "lowerArmL_z";
        //public static string handLX = "handL_x";
        //public static string handLY = "handL_y";
        //public static string handLZ = "handL_z";
        //public static string upperArmRX = "upperArmR_x";
        //public static string upperArmRY = "upperArmR_y";
        //public static string upperArmRZ = "upperArmR_z";
        //public static string lowerArmRX = "lowerArmR_x";
        //public static string lowerArmRY = "lowerArmR_y";
        //public static string lowerArmRZ = "lowerArmR_z";
        //public static string handRX = "handR_x";
        //public static string handRY = "handR_y";
        //public static string handRZ = "handR_z";
        public static string fingerUpperX = "finger_upper_x";
        public static string fingerUpperY = "finger_upper_y";
        public static string fingerUpperZ = "finger_upper_z";
        public static string fingerLowerX = "finger_lower_x";
        public static string fingerLowerY = "finger_lower_y";
        public static string fingerLowerZ = "finger_lower_z";
    }

    public class EnvironmentCommunicationChannel : SideChannel
    {
        static Lazy<EnvironmentCommunicationChannel> s_Lazy = new Lazy<EnvironmentCommunicationChannel>(() => new EnvironmentCommunicationChannel());

        public static bool IsInitialized
        {
            get { return s_Lazy.IsValueCreated; }
        }

        public static EnvironmentCommunicationChannel Instance { get { return s_Lazy.Value; } }

        private EnvironmentCommunicationChannel()
        {
            ChannelId = new Guid("621f0a70-4f87-11ea-a6bf-784f4387d1f7");
        }

        private Dictionary<string, float> envDict = null;
        private Dictionary<string, float> bodyDict = null;

        public Dictionary<string, float> GetEnvConfig()
        {
            return envDict;
        }

        public Dictionary<string, float> GetAgentBodyConfig()
        {
            return bodyDict;
        }

        protected override void OnMessageReceived(IncomingMessage msg)
        {
            var receivedString = msg.ReadString();
            Debug.Log("From Python : " + receivedString);

            Dictionary<string, string> msgDict = receivedString.FromJson<Dictionary<string, string>>();

            if (msgDict.ContainsKey(CommMessageKeys.envConfig))
            {
                envDict = ("{" + msgDict[CommMessageKeys.envConfig] + "}").FromJson<Dictionary<string, float>>();
            }
            SetEnvironmentParams();
            if (msgDict.ContainsKey(CommMessageKeys.bodyConfig))
            {
                bodyDict = ("{" + msgDict[CommMessageKeys.bodyConfig] + "}").FromJson<Dictionary<string, float>>();
            }

        }

        private void SetEnvironmentParams()
        {
            if (envDict == null)
                return;
            float scene = envDict.ContainsKey(CommMessageKeys.sceneToLoad)?envDict[CommMessageKeys.sceneToLoad] : 0;
            float day = envDict.ContainsKey(CommMessageKeys.gameDay) ? envDict[CommMessageKeys.gameDay] : 0;
            float time = envDict.ContainsKey(CommMessageKeys.gameTimeOfDay) ? envDict[CommMessageKeys.gameTimeOfDay] : 1200;
            Debug.Log("Time of day set from python: " + time);

            time = (float)(Math.Floor(time / 100f) + time % 100 / 60);

            EnvironmentController.Instance.gameStartDay = day;
            EnvironmentController.Instance.gameStartTime = time;
            //EnvironmentController.Instance.ChangeGameScene((int)scene);
            Debug.Log("Time of day: " + time);
        }

        public void SendExtraPython(string msg)
        {
            
                using (var msgOut = new OutgoingMessage())
                {
                    msgOut.WriteString(msg);
                    QueueMessageToSend(msgOut);
                }
        }

        public void SendDebugStatementToPython(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error)
            {
                var stringToSend = type.ToString() + ": " + logString + "\n" + stackTrace;
                using (var msgOut = new OutgoingMessage())
                {
                    msgOut.WriteString(stringToSend);
                    QueueMessageToSend(msgOut);
                }
            }
        }
    }
}