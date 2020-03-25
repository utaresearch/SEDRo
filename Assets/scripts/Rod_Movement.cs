using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rod_Movement : MonoBehaviour
{
    private Vector3 end1;
    private Vector3 end2; 
    public float speed = 1.0f;
    public Transform Paper;

    private void Start()
    {
        end1 = new Vector3(.177f, Paper.transform.localPosition.y + 0.013f,
         Paper.transform.localPosition.z);
        end2 = new Vector3(-.176f, Paper.transform.localPosition.y + 0.013f,
         Paper.transform.localPosition.z);
    }

    // Update is called once per frame
    void Update()
    {

        transform.localPosition = Vector3.Lerp(end1, end2,
            (Mathf.Sin(speed * Time.time) + 1.0f) / 2.0f);
    }
}
//Vector3 end1 = new Vector3(-1.039f, 0.997f, -0.179f);
//Vector3 end2 = new Vector3(-1.307f, 0.997f, -7);