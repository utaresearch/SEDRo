using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DAIVID;

public class BabyDataCollector : MonoBehaviour
{
    [SerializeField]
    private Camera headCam;

    Ray RayOrigin;
    RaycastHit HitInfo;

    private Dictionary<string, int> lookedObjectDistribution = null;

    private EnvironmentCommunicationChannel envCommunicationChannel;

    float timeSinceLastDataSent = 0;

    // Start is called before the first frame update
    void Start()
    {
        envCommunicationChannel = EnvironmentCommunicationChannel.Instance;
        lookedObjectDistribution = new Dictionary<string, int>();
        LookableObject[] allLookableObjects = FindObjectsOfType<LookableObject>();
        foreach(LookableObject obj in allLookableObjects)
        {
            //Debug.LogError(obj.name);
            lookedObjectDistribution.Add(obj.name, 0);
        }
        timeSinceLastDataSent = Time.time;
    }

    

    // Update is called once per frame
    void Update()
    {
        Transform cameraTransform = headCam.transform;
        RayOrigin = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(RayOrigin, out HitInfo, 100.0f))
        {
            Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 100.0f, Color.yellow);
            if (HitInfo.transform.gameObject.GetComponent<LookableObject>())
                //if (HitInfo.transform.name != "PFB_Building_Base")
            {
                string name = HitInfo.transform.name;
                //Debug.LogError(name + " Freq: " + lookedObjectDistribution[name]);

                lookedObjectDistribution[name] = lookedObjectDistribution[name] + 1;

                Debug.LogError(name + " Freq: " + lookedObjectDistribution[name]);
            }
            
        }
        float currentTime = Time.time;
        if((currentTime - timeSinceLastDataSent) > 5)
        {
            timeSinceLastDataSent = currentTime;
            envCommunicationChannel.SendInfoOutside(lookedObjectDistribution);
        }
        
    }
}
