//** RegisterFloatChannel.cs **//

using UnityEngine;
using Unity.MLAgents;
using CHANNEL;
using Unity.MLAgents.SideChannels;
using System.Text;
using System;
//using ANGLE;
using HIT;


public class RegisterFloatChannel : MonoBehaviour
{
    FloatSideChannel floatSideChannel;
    GameObject laser;


    public void Awake()
    {
        floatSideChannel = new FloatSideChannel();

        laser = GameObject.Find("Eyesight Main").gameObject;
        SideChannelManager.RegisterSideChannel(floatSideChannel);
    }
}
