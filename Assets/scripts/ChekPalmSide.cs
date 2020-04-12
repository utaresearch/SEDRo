using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChekPalmSide : MonoBehaviour
{
    const string V = "interactable";
    public bool isFacing = false;


    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == V && isFacing == false)
        {
            //Debug.Log("Contacted the child");
            isFacing = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        //Debug.Log("Exited the child");
        isFacing = false;
    }
}
