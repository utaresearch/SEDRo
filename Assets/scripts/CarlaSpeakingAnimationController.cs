using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarlaSpeakingAnimationController : MonoBehaviour
{
    public Transform jaw;

    [Range(0, 10)]
    public float maxAngle = 10;
    [Range(0, 8)]
    public float speed = 5;
    // Start is called before the first frame update
    Vector3 curRot;
    void Start()
    {
        curRot = jaw.localRotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.time * speed;
        float deltaRot = Mathf.Abs(Mathf.Sin(time)+ Mathf.Sin(2 * time) + Mathf.Sin(time/2.5f))/2;
        deltaRot = maxAngle * deltaRot + Random.Range(-1, 1);
        //Debug.Log("Sin Rotation: " + deltaRot);
        jaw.localRotation = Quaternion.Euler(curRot.x, curRot.y, curRot.z+ deltaRot);
    }
}
