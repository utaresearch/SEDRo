using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MotherIkController))]
public class Mother : MonoBehaviour
{

    private CharacterController momController;
    private Animator modelAnimator;

    private float rotationSpeed = 1f;

    private NavMeshAgent agent;

    [SerializeField]
    private Transform baby;
    [SerializeField]
    private Transform babyMouth;

    private bool isMoving = false;

    private MotherIkController ikController;

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

        ikController = GetComponent<MotherIkController>();
        if (!ikController) {
            Debug.LogError("Mother IK Controller not attached.");
        }
    }

    public bool isFeedingInProgress = false;
    // Update is called once per frame
    void Update()
    {
        
        if(Utility.Vector2DDistance(baby.position, transform.position) <= agent.stoppingDistance)
        {
            Debug.Log(Utility.Vector2DDistance(baby.position, transform.position) + " +updating+ " + agent.stoppingDistance);
            agent.isStopped = true;
            agent.SetDestination(transform.position);
        } else if (isMoving)
        {
            agent.SetDestination(baby.position);
        }
    }

    public void MoveToBaby(System.Action callback)
    {
        //Debug.Log(Utility.Vector2DDistance(baby.position, transform.position) + " ++ " + agent.stoppingDistance);
        if (Utility.Vector2DDistance(baby.position, transform.position) > agent.stoppingDistance)
        {
            agent.isStopped = false;
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
        
        if (!isMoving && Utility.Vector2DDistance(baby.position, transform.position) <= agent.stoppingDistance)
        {
            Debug.Log("Greeting");
            modelAnimator.SetBool("greet", true);
            //modelAnimator.GetCurrentAnimatorStateInfo
        }
    }

    private System.Action callback;

    public void Feed(System.Action callback)
    {
        isFeedingInProgress = true;
        Debug.Log("Start feeding");
        if (!isMoving && Utility.Vector2DDistance(baby.position, transform.position) > agent.stoppingDistance)
        {
            this.callback = callback;
            MoveToBaby(MovingComplete);
            Debug.Log("Going to baby");
        }
        else
        {
            //isFeeding = true;
            ikController.EnableIK(true, baby, babyMouth);

            StartCoroutine(OnCompleteFeedingAnimation(callback));
        }
    }

    private void MovingComplete()
    {
        //isFeeding = true;
        Debug.Log("MovingComplete");
        //modelAnimator.SetBool("greet", true);
        ikController.EnableIK(true, baby, babyMouth);
        
        StartCoroutine(OnCompleteFeedingAnimation(this.callback));
    }

    IEnumerator OnCompleteFeedingAnimation(System.Action callback)
    {
        //yield return new WaitUntil(() => modelAnimator.GetCurrentAnimatorStateInfo(0).IsName("greet") && modelAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
        yield return new WaitForSeconds(3.0f);

        //if (Utility.Vector2DDistance(baby.position, transform.position) <= agent.stoppingDistance)
        {
            //modelAnimator.SetBool("greet", false);
            callback();
            ikController.EnableIK(false, baby, babyMouth);
            isFeedingInProgress = false;
        }
    }


    IEnumerator OnCompleteGreetAnimation(System.Action callback)
    {
        yield return new WaitUntil(() => modelAnimator.GetCurrentAnimatorStateInfo(0).IsName("greet") && modelAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);

        if (Utility.Vector2DDistance(baby.position, transform.position) <= agent.stoppingDistance)
        {
            modelAnimator.SetBool("greet", false);
            callback();
        }
    }

    IEnumerator OnCompleteMoveAnimation(System.Action callback)
    {
        yield return new WaitUntil(() => Utility.Vector2DDistance(baby.position, transform.position) <= agent.stoppingDistance);

        if (Utility.Vector2DDistance(baby.position, transform.position) <= agent.stoppingDistance)
        {
            isMoving = false;
            modelAnimator.SetBool("move", false);
            callback();
        }
    }
}
