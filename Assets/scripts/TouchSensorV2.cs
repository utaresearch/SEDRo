using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface TouchListenerV2
{
    void onTouchTriggered(int touchSensorId);
    void onTouchExit(int touchSensorId);
}

public class TouchSensorV2 : MonoBehaviour
{
    [HideInInspector]
    public TouchListenerV2 touchListener;

    [Header("Touch sensor density:")]
    public int noOfSensorX = 1;
    public int noOfSensorY = 1;
    public int noOfSensorZ = 1;

    private List<float> touchSensorStatus;
    private int segmentCount;

    [SerializeField]
    private bool drawSegmentVisualizers;

    // Start is called before the first frame update
    void Start()
    {
        initializeSensorStatus();

        if (drawSegmentVisualizers)
        {
            drawSegmentation();
        }
    }

    void initializeSensorStatus()
    {

        segmentCount = noOfSensorX * noOfSensorY * noOfSensorZ;
        touchSensorStatus = Enumerable.Repeat(0f, segmentCount).ToList();
        //print(touchSensorStatus);
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
            //touchListener.onTouchTriggered(sensorId);
        }
        print(GetComponent<Renderer>().bounds.size);
    }

    private void drawSegmentation()
    {
        Bounds meshBounds = this.GetComponent<MeshFilter>().mesh.bounds;
        Vector3 meshBoundSize = meshBounds.size;
        for (int i = 0; i < noOfSensorZ - 1; i++)
        {
            GameObject s = CreateSeparator();
            s.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, s.transform.parent.transform.localRotation.z));
            s.transform.localPosition = new Vector3(0, 0, meshBounds.size.z / noOfSensorZ * (i + 1) - meshBounds.size.z / 2);
        }

        for (int i = 0; i < noOfSensorX - 1; i++)
        {
            GameObject s = CreateSeparator();
            s.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
            s.transform.localPosition = new Vector3(meshBounds.size.x / noOfSensorX * (i + 1) - meshBounds.size.x / 2, 0, 0);
        }

        for (int i = 0; i < noOfSensorY - 1; i++)
        {
            GameObject s = CreateSeparator();
            s.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
            s.transform.localScale = new Vector3(1f, 1f, .01f);
            s.transform.localPosition = new Vector3(0, meshBounds.size.y / noOfSensorY * (i + 1) - meshBounds.size.y / 2, 0);
        }

    }

    private GameObject CreateSeparator()
    {
        GameObject s = GameObject.CreatePrimitive(PrimitiveType.Cube);
        s.transform.SetParent(this.transform);
        s.GetComponent<Renderer>().material.color = Color.red;

        Bounds meshBounds = this.GetComponent<MeshFilter>().mesh.bounds;
        Vector3 meshBoundSize = meshBounds.size;
        print(meshBoundSize);

        s.transform.localScale = new Vector3(meshBoundSize.x, meshBoundSize.y, .01f);


        //Rigidbody rb = s.GetComponent<Rigidbody>();
        //rb.isKinematic = true;
        //rb.useGravity = false;
        Collider collider = s.GetComponent<Collider>();
        collider.enabled = false;
        collider.isTrigger = true;
        //DestroyImmediate(collider);
        return s;
    }

    /// <summary>
    /// Call this method to collect the sensor observations.
    /// This sensor class will gather sensor touch data untill this method is called.
    /// Once this method is called, sensor data will be cleared and restarted(Touch status is retained untill this method is called).
    /// </summary>
    /// <returns></returns>
    public List<float> CollectSensorStatus()
    {
        List<float> status = new List<float>();
        if (touchSensorStatus == null)
            return status;
        status.AddRange(touchSensorStatus);
        for (int i = 0; i < touchSensorStatus.Count; i++)
        {
            touchSensorStatus[i] = 0;
        }
        return status;
    }

    void udateSensorStatus(Collision collision)
    {
        //for(int i= 0;i<touchSensorStatus.Count;i++)
        //{
        //    touchSensorStatus[i] = 0;
        //}
        //print("Collision Count: "+collision.contacts.Count());
        Vector3 sensorPos;
        foreach (ContactPoint contact in collision.contacts)
        {
            //if (contact.otherCollider.name.Contains("PFB"))
            //    continue;
            sensorPos = findContactSegment(contact);
            int idx = (int)(sensorPos.x + noOfSensorX * sensorPos.y + noOfSensorX*noOfSensorY*sensorPos.z);
            //print("index: " + idx);
            touchSensorStatus[idx] = 1;

        }
        //Debug.Log(touchSensorStatus.ToString());
    }

    public int GetSensorCount()
    {
        return noOfSensorX * noOfSensorY * noOfSensorZ; ;
    }

    void OnCollisionStay(Collision collision)
    {
        if(touchSensorStatus != null)
            udateSensorStatus(collision);
    }

    private Vector3 findContactSegment(ContactPoint contact)
    {
        //print(contact.thisCollider.name + " hit " + contact.otherCollider.name);

        Bounds meshBounds = this.GetComponent<MeshFilter>().mesh.bounds;
        Vector3 meshBoundSize = meshBounds.size;
        Vector3 boundedScale = Vector3.Scale(contact.thisCollider.transform.localScale, meshBoundSize);

        Vector3 localContactPoint = contact.thisCollider.transform.InverseTransformPoint(contact.point);
        //localContactPoint = Vector3.Scale(localContactPoint, meshBoundSize);
        //print("CP: " + contact.point + ":local CP:" + localContactPoint + ":BS:" + boundedScale + ":Mesh Bound:" + meshBoundSize + "::" + meshBounds.center + "::" + meshBounds.max);

        Vector3 translatedLocalCP = localContactPoint; //+ meshBounds.size/2;//boundedScale / 2;
        //print(localContactPoint + ":---:" + translatedLocalCP);
        //Vector3 sensorPos = new Vector3(translatedLocalCP.x / (boundedScale.x),
        //                        translatedLocalCP.y / (boundedScale.y),
        //                        translatedLocalCP.z / (boundedScale.z));
        Vector3 sensorPos = new Vector3(translatedLocalCP.x / (meshBounds.size.x),
                                translatedLocalCP.y / (meshBounds.size.y),
                                translatedLocalCP.z / (meshBounds.size.z));
        sensorPos = sensorPos + new Vector3(.5f, .5f, .5f); //Translate the values to avoid negative.
        //print("Sensor pos: " + sensorPos);

        /// 0 indexed sensor/segment position
        /// Sometimes the collider detects collision on the edges, which causes incorrect calculation.
        /// To avoid that, index is capped between 0 and noOfSensor-1 (for 0 indexed)
        sensorPos = new Vector3(Math.Min(noOfSensorX-1, Math.Max(0, Mathf.CeilToInt(sensorPos.x * noOfSensorX)-1)),
                                Math.Min(noOfSensorY - 1, Math.Max(0, Mathf.CeilToInt(sensorPos.y * noOfSensorY)-1)),
                                Math.Min(noOfSensorZ - 1, Math.Max(0, Mathf.CeilToInt(sensorPos.z * noOfSensorZ)-1)));
        //print("Sensor index: " + sensorPos);

        //print((contact.thisCollider.transform.localPosition - contact.thisCollider.transform.InverseTransformPoint(contact.point)).normalized);

        //// Visualize the contact point
        Debug.DrawRay(contact.point, contact.normal, Color.blue);
        return sensorPos;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("TriggerExit:" + other.name);
        if (touchListener != null)
        {
            //touchListener.onTouchExit(sensorId);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Collision:Enter");
    }

    private void OnCollisionExit(Collision collision)
    {
        //Debug.Log("Collision:Exit");
    }
}
