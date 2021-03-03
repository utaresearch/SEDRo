
//** FloatChannel.cs **//


using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.SideChannels;
using System.Text;
using System;
using System.Collections.Generic;
using CHANNEL;


namespace CHANNEL
{

    public class FloatSideChannel : SideChannel
    {
        public FloatSideChannel()
        {
            ChannelId = new Guid("a2abf121-a0d4-4247-8515-6737ae911ecc");
        }

        protected override void OnMessageReceived(IncomingMessage msg)
        {
            var receivedString = msg.ReadString();
            Debug.Log("From Python : " + receivedString);
        }


        public void SendDebugStatementToPython(List<float> hitPoints)
        {
            using (var msgOut = new OutgoingMessage())
            {
                msgOut.WriteFloatList((hitPoints));
                QueueMessageToSend(msgOut);
            }

        }

    }
}
