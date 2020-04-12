using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fjGrab : MonoBehaviour
{
    const string V = "interactable";
    public bool isHolding = false;

    public float forceTbreak = 5;
    public float torqueTbreak = 5;

    ChekPalmSide temp;

    // Start is called before the first frame update
    private void Start()
    {
        temp = gameObject.GetComponentInChildren<ChekPalmSide>();
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Contacted!! but not yet facing.");
        if (collision.gameObject.tag == V && isHolding == false && temp.isFacing)
        {
            isHolding = true;
            FixedJoint joint = collision.gameObject.AddComponent<FixedJoint>();
            ContactPoint contact = collision.contacts[0];
           // Debug.Log("Contacted!!");
            
            joint.anchor = transform.InverseTransformPoint(contact.point);
            joint.connectedBody = this.gameObject.transform.GetComponent<Rigidbody>();

            joint.breakForce = forceTbreak;
            joint.breakTorque = torqueTbreak;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        isHolding = false;
    }

    private void FixedUpdate()
    {
        temp = gameObject.GetComponentInChildren<ChekPalmSide>();
    }

}
