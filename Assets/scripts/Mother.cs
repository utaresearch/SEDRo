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

    private bool isMoving = false;

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

    private bool isFeeding = false;
    // Update is called once per frame
    void Update()
    {

    
    }

    public void MoveToBaby(System.Action callback)
    {
        Debug.Log(Vector3.Distance(baby.position, transform.position) + " ++ " + agent.stoppingDistance);
        if (Vector3.Distance(baby.position, transform.position) > agent.stoppingDistance)
        {
            agent.SetDestination(baby.position);
            modelAnimator.SetBool("move", true);
            isMoving = true;
            StartCoroutine(OnCompleteMoveAnimation(callback));
        }
        else
        {
            modelAnimator.SetBool("move", false);
            isMoving = false;
        }
    }

    public void Greet()
    {
        
        if (!isMoving && Vector3.Distance(baby.position, transform.position) <= agent.stoppingDistance)
        {
            Debug.Log("Greeting");
            modelAnimator.SetBool("greet", true);
            //modelAnimator.GetCurrentAnimatorStateInfo
        }
    }

    private System.Action callback;

    public void Feed(System.Action callback)
    {
        if (!isMoving && Vector3.Distance(baby.position, transform.position) > agent.stoppingDistance)
        {
            this.callback = callback;
            MoveToBaby(MovingComplete);
            Debug.Log("Going to baby");
        }
        else
        {
            //isFeeding = true;
            modelAnimator.SetBool("greet", true);
            StartCoroutine(OnCompleteGreetAnimation(this.callback));
        }
    }

    private void MovingComplete()
    {
        //isFeeding = true;
        Debug.Log("MovingComplete");
        modelAnimator.SetBool("greet", true);
        StartCoroutine(OnCompleteGreetAnimation(this.callback));
    }

    IEnumerator OnCompleteGreetAnimation(System.Action callback)
    {
        yield return new WaitUntil(() => modelAnimator.GetCurrentAnimatorStateInfo(0).IsName("greet") && modelAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);

        if (Vector3.Distance(baby.position, transform.position) <= agent.stoppingDistance)
        {
            modelAnimator.SetBool("greet", false);
            callback();
        }
    }

    IEnumerator OnCompleteMoveAnimation(System.Action callback)
    {
        yield return new WaitUntil(() => Vector3.Distance(baby.position, transform.position) <= agent.stoppingDistance);

        if (Vector3.Distance(baby.position, transform.position) <= agent.stoppingDistance)
        {
            isMoving = false;
            modelAnimator.SetBool("move", false);
            callback();
        }
    }
}
