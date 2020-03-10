using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchSensorController : MonoBehaviour, TouchListener
{
    [Header("Body's touch sensors")]
    [Space(10)]
    public List<Transform> sensors;

    private List<float> touchSensorStatus;

    public void onTouchTriggered(int touchSensorId)
    {
        Debug.Log("Touch detected: " + touchSensorId+"::"+ touchSensorStatus.Count);
        updateSensorStatus(touchSensorId, 1);
    }

    private void updateSensorStatus(int sensorId, float status)
    {
        if (touchSensorStatus.Count - 1 < sensorId)
        {
            touchSensorStatus.Add(status);
        }
        else
        {
            touchSensorStatus[sensorId] = status;
        }
    }

    public List<float> collectTouchUpdates()
    {
        return touchSensorStatus;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(sensors!= null)
        {
            int sensorId = 0;
            foreach(Transform sensorObj in sensors)
            {
                TouchSensor sensor = sensorObj.gameObject.AddComponent<TouchSensor>();
                sensor.sensorId = sensorId++;
                sensor.touchListener = this;
            }
            touchSensorStatus = new List<float>();


        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onTouchExit(int touchSensorId)
    {
        Debug.Log("Touch exit: " + touchSensorId);
        updateSensorStatus(touchSensorId, 0);
    }
}
