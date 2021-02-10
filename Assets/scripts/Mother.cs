using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DAIVID
{
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

        [SerializeField]
        private Transform toyTarget;
        [SerializeField]
        private Transform feederObj;

        [SerializeField]
        private Transform rightHand;

        private Transform destination;

        private bool isMoving = false;

        private MotherIkController ikController;

        private float[] motherVoiceTextVec = new float[26];

        public Transform cribStandingTarget;


        public enum ActionType
        {
            None,
            Feeding,
            BringingToy,
            ShowToy
        }

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
            if (!ikController)
            {
                Debug.LogError("Mother IK Controller not attached.");
            }
        }

        public bool isFeedingInProgress = false;
        public bool isMotherBusy = false;
        public bool isMotherSpeaking = false;
        private char motherSpeakingCharacter = (char)0;
        // Update is called once per frame
        void Update()
        {

            if (isMotherSpeaking && motherSpeakingCharacter>0)
            {
                Array.Clear(motherVoiceTextVec, 0, motherVoiceTextVec.Length);
                motherVoiceTextVec[motherSpeakingCharacter - 'A'] = 1;
                Debug.Log("Mother speaking voice: " + motherSpeakingCharacter);
            }

            if (destination && Utility.Vector2DDistance(destination.position, transform.position) <= agent.stoppingDistance)
            {
                //Debug.Log(Utility.Vector2DDistance(baby.position, transform.position) + " +updating+ " + agent.stoppingDistance);
                agent.isStopped = true;
                agent.SetDestination(transform.position);
            }
            else if (isMoving)
            {
                agent.SetDestination(destination.position);
            }
        }

        public float[] getVoiceVector()
        {

            float[] copy = new float[26];
            Array.Copy(motherVoiceTextVec, copy, copy.Length);
            Array.Clear(motherVoiceTextVec, 0, motherVoiceTextVec.Length);
            return copy;
        }

        public void MoveToTarget(System.Action<Transform, ActionType> callback, Transform target, ActionType actionType)
        {
            destination = target;
            //Debug.Log(Utility.Vector2DDistance(baby.position, transform.position) + " ++ " + agent.stoppingDistance);
            if (Utility.Vector2DDistance(target.position, transform.position) > agent.stoppingDistance)
            {
                agent.isStopped = false;
                agent.SetDestination(destination.position);
                if (actionType == ActionType.Feeding)
                {
                    //agent.stoppingDistance = 0.6f;
                }
                else if (actionType == ActionType.BringingToy)
                {
                    //agent.stoppingDistance = .5f;
                }
                modelAnimator.SetBool("move", true);
                isMoving = true;
                StartCoroutine(OnCompleteMoveAnimation(callback, target, actionType));
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
            feederObj.gameObject.SetActive(true);
            if (!isMoving && Utility.Vector2DDistance(baby.position, transform.position) > agent.stoppingDistance)
            {
                this.callback = callback;
                MoveToTarget(MovingComplete, baby, ActionType.Feeding);
                Debug.Log("Going to baby");
            }
            else
            {
                //isFeeding = true;
                ikController.EnableIK(true, babyMouth, ActionType.Feeding);

                StartCoroutine(OnCompleteFeedingAnimation(callback, ActionType.Feeding));
            }
        }

        public void BringToy(System.Action callback)
        {
            if (isFeedingInProgress)
            {
                Debug.LogWarning("Mother is busy in feeding");
                return;
            }
            if (isMotherBusy)
            {
                Debug.LogWarning("Mother is busy");
                return;
            }
            isMotherBusy = true;
            feederObj.gameObject.SetActive(false);
            if (!isMoving && Utility.Vector2DDistance(toyTarget.position, transform.position) > agent.stoppingDistance)
            {
                this.callback = callback;
                MoveToTarget(MovingComplete, toyTarget, ActionType.BringingToy);
                Debug.Log("Going to baby");
            }
            else
            {
                //isFeeding = true;
                ikController.EnableIK(true, toyTarget, ActionType.BringingToy);

                StartCoroutine(OnCompletePickingAnimation(callback, ActionType.BringingToy));
            }
        }

        private void MovingComplete(Transform target, ActionType actionType)
        {
            //isFeeding = true;
            Debug.Log("MovingComplete: " + target.ToString());
            //modelAnimator.SetBool("greet", true);
            agent.isStopped = true;
            agent.SetDestination(transform.position);

            if (actionType == ActionType.Feeding)
            {
                ikController.EnableIK(true, baby, babyMouth, actionType);
                StartCoroutine(OnCompleteFeedingAnimation(this.callback, actionType));


                GetComponent<CarlaSpeakingAnimationController>().enabled = true;
                voiceCharIndex = 0;
                motherSpeakingCharacter = (char)0;
                isMotherSpeaking = true;
                this.actionType = ActionType.Feeding;
                StartCoroutine(DescribeObject("MILK", 2, OnSpeakingComplete));
            }
            else if (actionType == ActionType.BringingToy)
            {
                toyTarget.GetComponent<Rigidbody>().isKinematic = true;
                ikController.EnableIK(true, toyTarget, actionType);
                StartCoroutine(OnCompletePickingAnimation(this.callback, actionType));
            } else if(actionType == ActionType.ShowToy)
            {
                Transform showTarget = Transform.Instantiate(babyMouth, babyMouth.position + new Vector3(.2f, .4f, 0), babyMouth.rotation);
                ikController.EnableIK(true, showTarget, actionType);
                //DescribeObject(toyTarget, 1);
                GetComponent<CarlaSpeakingAnimationController>().enabled = true;
                voiceCharIndex = 0;
                isMotherSpeaking = true;
                motherSpeakingCharacter = (char)0;
                this.actionType = ActionType.ShowToy;
                StartCoroutine(DescribeObject(toyTarget.name, 2, OnSpeakingComplete));
            }

        }

        ActionType actionType = ActionType.None;

        private void OnSpeakingComplete()
        {
            if(this.actionType==ActionType.ShowToy)
                ikController.EnableIK(false, toyTarget, ActionType.None);
            isMotherSpeaking = false;
            GetComponent<CarlaSpeakingAnimationController>().enabled = false;
            //toyTarget.parent = null;
            //toyTarget.GetComponent<Rigidbody>().isKinematic = false;
            Debug.Log("Speaking complete");
        }

        private int voiceCharIndex = 0;
        IEnumerator DescribeObject(string word, float duration, System.Action callback)
        {
            
            yield return new WaitForSeconds(duration/ word.Length);
            if (voiceCharIndex < word.Length)
            {
                StartCoroutine(DescribeObject(word, duration, callback));
                Debug.Log("Word: " + word + " idx: " + voiceCharIndex.ToString());

                motherSpeakingCharacter = word[voiceCharIndex++];
                Debug.Log("Voice char: " + motherSpeakingCharacter);
            }
            else
            {
                
                callback();
                yield return new WaitForSeconds(1);
                //ikController.EnableIK(false, toyTarget, ActionType.None);
                toyTarget.parent = null;
                toyTarget.GetComponent<Rigidbody>().isKinematic = false;
                toyTarget.GetComponent<MeshCollider>().enabled = true;
                //return;
            }



            
            
        }

        IEnumerator OnCompleteFeedingAnimation(System.Action callback, ActionType actionType)
        {
            //yield return new WaitUntil(() => modelAnimator.GetCurrentAnimatorStateInfo(0).IsName("greet") && modelAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
            yield return new WaitForSeconds(3.0f);

            //if (Utility.Vector2DDistance(baby.position, transform.position) <= agent.stoppingDistance)
            {
                //modelAnimator.SetBool("greet", false);
                callback();
                ikController.EnableIK(false, baby, babyMouth, actionType);
                isFeedingInProgress = false;
            }
        }

        Vector3 toyPosition = new Vector3(0.301f, 0.019f, 0.155f);
        Vector3 toyRotation = new Vector3(-19.425f, -39.449f, 81.834f);
        IEnumerator OnCompletePickingAnimation(System.Action callback, ActionType actionType)
        {
            //yield return new WaitUntil(() => modelAnimator.GetCurrentAnimatorStateInfo(0).IsName("greet") && modelAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
            yield return new WaitForSeconds(1.0f);

            //if (Utility.Vector2DDistance(baby.position, transform.position) <= agent.stoppingDistance)
            {
                //modelAnimator.SetBool("greet", false);
                //callback();
                toyTarget.parent = rightHand;
                toyTarget.GetComponent<Rigidbody>().isKinematic = true;
                toyTarget.GetComponent<MeshCollider>().enabled = false;
                toyTarget.localPosition = toyPosition;
                toyTarget.localRotation = Quaternion.Euler(toyRotation);
                ikController.EnableIK(false, null, actionType);
                MoveToTarget(MovingComplete, cribStandingTarget, ActionType.ShowToy);
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

        IEnumerator OnCompleteMoveAnimation(System.Action<Transform, ActionType> callback, Transform target, ActionType actionType)
        {
            yield return new WaitUntil(() => Utility.Vector2DDistance(target.position, transform.position) <= agent.stoppingDistance);

            if (Utility.Vector2DDistance(target.position, transform.position) <= agent.stoppingDistance)
            {
                isMoving = false;
                modelAnimator.SetBool("move", false);
                callback(target, actionType);
            }
        }
    }
}