using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickable : MonoBehaviour
{
    float tf = 0.091f;// Grabbing distance

    float dist, dist1, dist2, dist3, dist4, dist5, dst, dst1, dst2, dst3, dst4, dst5;

    public GameObject item;

    public GameObject palmR;
    public GameObject thumR;
    public GameObject indR;
    public GameObject midR;
    public GameObject rinR;
    public GameObject lilR;

    public GameObject palmL;
    public GameObject thumL;
    public GameObject indL;
    public GameObject midL;
    public GameObject rinL;
    public GameObject lilL;

    public bool isheldR = false;
    public bool isheldL = false;

    FixedJoint jointA = null;

    fjGrab Rholding;
    fjGrab Lholding;

    private void Start()
    {
        Rholding = palmR.transform.GetComponent<fjGrab>();
        Lholding = palmL.transform.GetComponent<fjGrab>();
    }


    void Update()
    {
        //distance from right hand 
        dist1 = Vector3.Distance(this.transform.position, thumR.transform.position);
        dist2 = Vector3.Distance(this.transform.position, indR.transform.position);
        dist3 = Vector3.Distance(this.transform.position, midR.transform.position);
        dist4 = Vector3.Distance(this.transform.position, rinR.transform.position);
        dist5 = Vector3.Distance(this.transform.position, lilR.transform.position);

        //distance from left hand
        dst1 = Vector3.Distance(this.transform.position, thumL.transform.position);
        dst2 = Vector3.Distance(this.transform.position, indL.transform.position);
        dst3 = Vector3.Distance(this.transform.position, midL.transform.position);
        dst4 = Vector3.Distance(this.transform.position, rinL.transform.position);
        dst5 = Vector3.Distance(this.transform.position, lilL.transform.position);


        if(dist1 <= tf && dist2 <= tf && dist3 <= tf && dist4 <= tf && dist5 <= tf)
        {
                isheldR = true;
        }
        
        
        if (dst1 <= tf && dst2 <= tf && dst3 <= tf && dst4 <= tf && dst5 <= tf)
        {
                isheldL = true;
        }

        if(jointA != null )
        {
            if(isheldL || isheldR)
            {
                jointA.breakForce = 10;
                jointA.breakTorque = 10;

            }
        }
        
    }

    private void OnJointBreak(float breakForce)
    {
        isheldL = false;
        isheldR = false;
        Rholding.isHolding = false;
        Lholding.isHolding = false;

    }


}
