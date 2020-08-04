using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mother : MonoBehaviour
{

    private CharacterController momController;
    private Animator modelAnimator;

    private float rotationSpeed = 1f;

    private NavMeshAgent agent;

    [SerializeField]
    private Transform baby;

    // Start is called before the first frame update
    void Start()
    {
        momController = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        if (!momController)
        {
            Debug.LogError("Mother doesn't have any Character Controller attached");
        }
        modelAnimator = GetComponent<Animator>();
        if (!modelAnimator)
        {
            Debug.LogWarning("Mother doesn't have any Animator attached");
        }

        //Invo
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 direction = baby.position - transform.position;
        //direction = direction.normalized * modelAnimator.runtimeAnimatorController.animationClips[0].averageSpeed.magnitude;
        //momController.Move(direction);

        if(Vector3.Distance(baby.position, transform.position) >= .9)
        {
            agent.SetDestination(baby.position);
            Debug.Log("Moving");
            modelAnimator.SetBool("move", true);
            //modelAnimator.
            //Vector3 _direction = (baby.position - transform.position).normalized;

            ////create the rotation we need to be in to look at the target
            //Quaternion _lookRotation = Quaternion.LookRotation(_direction);

            ////rotate us over time according to speed until we are in the required rotation
            //transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            modelAnimator.SetBool("move", false);
        }

        
    }
}
