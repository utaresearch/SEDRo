using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TouchListener
{
    void onTouchTriggered(int touchSensorId);
    void onTouchExit(int touchSensorId);
}

public class TouchSensor : MonoBehaviour
{
    [HideInInspector]
    public TouchListener touchListener;
    
    [Header("Id of the sensor")]
    public int sensorId;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        Collider collider = GetComponent<Collider>();
        collider.isTrigger = true;
        GetComponent<MeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Update:");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger:"+ other.name);
        if (touchListener != null)
        {
            touchListener.onTouchTriggered(sensorId);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("TriggerExit:" + other.name);
        if (touchListener != null)
        {
            touchListener.onTouchExit(sensorId);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision:");
    }
}
