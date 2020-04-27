using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObserver : MonoBehaviour
{

    public GameObject R;
    public GameObject L;

    [HideInInspector]
    public float[] obserVect = new float [2];
    
 

    // Update is called once per frame
    void Update()
    {
     /*   if (R.transform.GetComponent<fjGrab>().isHolding)
        { }
        else { obserVect[0] = 0f; }
        if (L.transform.GetComponent<fjGrab>().isHolding)
        { }
        else { obserVect[1] = 0f; }
        */
    }

    public float[] CollectGrabstatus()
    {
        obserVect[0] = 1f;
        obserVect[1] = 1f;
        return obserVect;
    }
}
