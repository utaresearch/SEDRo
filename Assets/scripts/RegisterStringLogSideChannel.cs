using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.SideChannels;
using DAIVID;

public class RegisterStringLogSideChannel : MonoBehaviour
{

    EnvironmentCommunicationChannel stringChannel;
    public void Awake()
    {
        // We create the Side Channel
        stringChannel = EnvironmentCommunicationChannel.Instance;

        // When a Debug.Log message is created, we send it to the stringChannel
        Application.logMessageReceived += stringChannel.SendDebugStatementToPython;

        // The channel must be registered with the SideChannelManager class
        SideChannelManager.RegisterSideChannel(stringChannel);
    }

    public void OnDestroy()
    {
        // De-register the Debug.Log callback
        Application.logMessageReceived -= stringChannel.SendDebugStatementToPython;
        if (Academy.IsInitialized)
        {
            SideChannelManager.UnregisterSideChannel(stringChannel);
        }
    }
}