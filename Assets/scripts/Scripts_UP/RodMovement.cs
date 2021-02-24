using UnityEngine;
using System.Collections;

public class RodMovement : MonoBehaviour
{
    // start and end markers for the path.
    public Transform startMarker;
    public Transform endMarker;

    // movement speed in units per second.
    public float speed = 0.025F;

    // time at start of movement
    private float startTime;

    // distance between the markers.
    private float pathLength;

    // temporary variables to interchange positions of markers to set reverse motion
    Vector3 tempPos1, tempPos2;
    

    void Start()
    {
        // to be used as offset when you want to reset timer
        startTime = Time.time;


        // to calculate path length between markers
        pathLength = Vector3.Distance(startMarker.position, endMarker.position);
    }

   

    // To and fro movement between the markers
    void Update()
    {

        // distance moved equals elapsed time elapsed * speed. StartTime = 0 till it is reset
        float distCovered = (Time.time - startTime) * speed;

        // fraction of path covered in the time elapsed 
        float fractionOfPath = distCovered / pathLength;

        // set our position as a fraction of the distance between the markers. movement from start to end once
        transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fractionOfPath);

        // once gameobject reaches the end marker
        if (transform.position == endMarker.position)
        {
            // setting the variables for return from end to start
            startTime = Time.time;
            tempPos1 = startMarker.position;
            startMarker.position = endMarker.position;
            endMarker.position = tempPos1;
        }

        // once the gameobject reaches the start again
        if (transform.position == tempPos1)
        {
            //setting variables for journey from start to end again
            startTime = Time.time;
            tempPos2 = startMarker.position;
            startMarker.position = tempPos1;
            endMarker.position = tempPos2;
        }


    }

}