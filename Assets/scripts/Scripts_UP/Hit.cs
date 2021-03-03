
//** Hit.cs **//

using UnityEngine;
using System.Collections.Generic;
using Unity.MLAgents;
using CHANNEL;
using Unity.MLAgents.SideChannels;
using System.Text;
using System;

namespace HIT
{
    public class Hit : MonoBehaviour
    {
        public Vector3? collisionPoint2 = null;
        public List<float> hitPoints = new List<float>();
        private float contactTime = 0;
        public float x = 0F;
        public float y = 0F;
        public float z = 0F;

        public FloatSideChannel floatSideChannel;

        public void Awake()
        {
            floatSideChannel = new FloatSideChannel();
            SideChannelManager.RegisterSideChannel(floatSideChannel);
            
        }

        public void OnTriggerEnter(Collider other)
        {
            contactTime = 0;
        }

        public void OnTriggerStay(Collider other)
        {
            Debug.Log(other.tag);
            if (other.tag == "looking" && Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
            {
                contactTime += Time.deltaTime;
                Debug.Log("contactTime" + contactTime);


                //Debug.Log("tick start" + t1);88
                GameObject cube = GameObject.Find("Cube");


                // finding point of collision
                collisionPoint2 = cube.transform.InverseTransformPoint(hit.point);

                // extracting x,y,z coordinates of point of collision
                x = collisionPoint2.Value.x;
                y = collisionPoint2.Value.y;
                z = collisionPoint2.Value.z;
               

                // adding the coordinates to a list
                hitPoints.Add(x);
                //hitPoints.Add(y);
                hitPoints.Add(z);
                hitPoints.Add(contactTime);
                

                // sending the list to a method in FloatChannel to transmit to Python
                floatSideChannel.SendDebugStatementToPython(hitPoints);

                // clearing the list to load coordinates of next hit point
                hitPoints.Clear();
            }
        }

        public void Update() { }
    }
}

