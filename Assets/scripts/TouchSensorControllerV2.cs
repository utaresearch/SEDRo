using System;
using System.Collections;
using System.Collections.Generic;
using DAIVID;
using UnityEngine;

public class TouchSensorControllerV2 : MonoBehaviour
{

    internal List<float> CollectTouchUpdatesForBodyParts(Dictionary<Transform, BodyPart>.KeyCollection bodyParts)
    {
        List<float> sensorStatus = new List<float>();
        foreach (var bodyPart in bodyParts)
        {
            TouchSensorV2 sensor = bodyPart.GetComponent<TouchSensorV2>();
            if(sensor != null)
            {
                sensorStatus.AddRange(sensor.CollectSensorStatus());
            }
            
        }
        return sensorStatus;
    }

    internal int GetSensorCounts(Dictionary<Transform, BodyPart>.KeyCollection bodyParts)
    {
        int totalSensorCount = 0;
        foreach (var bodyPart in bodyParts)
        {
            TouchSensorV2 sensor = bodyPart.GetComponent<TouchSensorV2>();
            if (sensor != null)
            {
                totalSensorCount += sensor.GetSensorCount();
            }

        }
        return totalSensorCount;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
