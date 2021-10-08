using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookableObject: MonoBehaviour
{
    string objectName;

    void Start()
    {
        this.objectName = gameObject.name;
        Debug.LogError(this.objectName+" is lookable");
    }
    

}
