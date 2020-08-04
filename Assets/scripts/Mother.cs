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
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(baby.position, transform.position) >= agent.stoppingDistance)
        {
            agent.SetDestination(baby.position);
            modelAnimator.SetBool("move", true);
        }
        else
        {
            modelAnimator.SetBool("move", false);
        }

        
    }
}
