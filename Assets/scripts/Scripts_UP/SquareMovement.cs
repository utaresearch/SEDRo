using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareMovement : MonoBehaviour
{
    // distance covered after each frame
    float distanceCovered;

    // diameter of revolution of the object
    float circleDiameter;

    // speed in deg/sec
    float speed;

    float z;

    public Transform block;


    void Start()
    {
        //initializing variables

        distanceCovered = 0;
        circleDiameter = (block.transform.position.x + 0.025F) - (block.transform.position.x - 0.025F);
        // converting speed to rad/sec as mathf functions accept only rad/sec.
        speed = 45F * Mathf.Deg2Rad;
        z = transform.localPosition.z;
    }


    void Update()
    {
        // multiplying s * t to get d
        distanceCovered += Time.deltaTime * speed;

        // stimulus movement control

        float x = Mathf.Cos(distanceCovered) * circleDiameter;
        float y = Mathf.Sin(distanceCovered) * circleDiameter;
        transform.position = new Vector3(x, y, z);
    }
}