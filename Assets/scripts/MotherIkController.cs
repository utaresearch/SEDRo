using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class MotherIkController : MonoBehaviour
{
    public bool ikActive = false;
    public Transform rightHandObj = null;
    public Transform leftHandObj = null;
    public Transform rightFootObj = null;
    public Transform leftFootObj = null;

    public Transform rightHandTarget = null;
    public Transform leftHandTarget  = null;
    public Transform rightFootTarget = null;
    public Transform leftFootTarget = null;

    public Transform rightHandHint = null;
    public Transform leftHandHint = null;
    public Transform rightFootHint = null;
    public Transform leftFootHint = null;

    public Transform lookTarget = null;

    public Transform hipTarget = null;

    public Transform hipRotTarget = null;

    public Transform spineRotTarget = null;

    public Transform rightWristRotTarget = null;

    public Transform hipObj = null;

    public Transform spineObj = null;

    protected Animator animator;

    [Range(-1, 1)]
    public float hipOffset;

    [Header("Rig objects")]
    [SerializeField]
    private Rig headRig;
    [SerializeField]
    private Rig armRig;
    [SerializeField]
    private Rig hipRig;
    [SerializeField]
    private Rig feetRig;
    [SerializeField]
    private Rig hipRotRig;


    Vector3 hipOriginalPos;
    void Start()
    {
        animator = GetComponent<Animator>();
        hipOriginalPos = hipTarget.position;
    }

    private void Update()
    {
        if (ikActive)
        {
            Vector3 relativePos = babyMouthPos.position - hipObj.position;
            relativePos = new Vector3(relativePos.x, 0, relativePos.z);

            // the second argument, upwards, defaults to Vector3.up
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            //hipTarget.rotation = rotation;

            Vector3 eulerRot = rotation.eulerAngles;

            eulerRot.z -= 90;
            eulerRot.y += 90;
            //hipObj.rotation = Quaternion.Euler(eulerRot);
        }
    }

    private Transform lookObj;

    private Transform babyMouthPos;
    private float startTime = 0;
    public void EnableIK(bool enabled, Transform baby, Transform babyMouth)
    {
        ikActive = enabled;
        if (ikActive)
        {
            //headRig.weight = 1;
            //armRig.weight = 1;
            //feetRig.weight = 1;
            //hipRig.weight = 1;

            startTime = Time.time;

            leftHandTarget.position = leftHandObj.position;
            rightHandTarget.position = rightHandObj.position;
            //leftFootTarget.position = leftFootObj.position;
            //rightFootTarget.position = rightFootObj.position;

            leftHandTarget.rotation = leftHandObj.rotation;
            rightHandTarget.rotation = rightHandObj.rotation;
            //leftFootTarget.rotation = leftFootObj.rotation;
            //rightFootTarget.rotation = rightFootObj.rotation;

            lookObj = baby;
            babyMouthPos = babyMouth;
            lookTarget.position = babyMouthPos.position;
            rightHandTarget.position = babyMouthPos.position;
            heightDiff = rightHandTarget.position.y - rightHandObj.position.y;
        }
        else
        {
            startTime = Time.time;
            //headRig.weight = 0;
            //armRig.weight = 0;
            //feetRig.weight = 0;
            //hipRig.weight = 0;
        }
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (animator)
        {

            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {
                
                //animator.bodyPosition += Vector3.down * hipOffset;
                //lfootTarget.position -= Vector3.down * hipOffset;
                // Set the look target position, if one has been assigned
                //if (lookObj != null)
                //{
                //    animator.SetLookAtWeight(1);
                //    animator.SetLookAtPosition(lookObj.position);
                //}

                //// Set the right hand target position and rotation, if one has been assigned
                //if (rightHandObj != null)
                //{
                //animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                //    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                //    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                //    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);

                //    animator.SetIKPosition(AvatarIKGoal.LeftFoot, rightHandObj.position);
                //    animator.SetIKRotation(AvatarIKGoal.LeftFoot, rightHandObj.rotation);

                //    animator.SetIKPosition(AvatarIKGoal.RightFoot, rightHandObj.position);
                //    animator.SetIKRotation(AvatarIKGoal.RightFoot, rightHandObj.rotation);
                //}

            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                //animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                //animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                //animator.SetLookAtWeight(0);
            }
        }
    }

    private float speed = 2f;
    private float totalAnimTime = 1.2f;
    private Vector3 hipTargetOldPos = Vector3.zero;
    private float heightDiff = 0;
    private void LateUpdate()
    {
        float t = (Time.time - startTime) / totalAnimTime * speed;
        if (ikActive)
        {
            

            if (Mathf.Abs(heightDiff) > 0.05f)
            {
                float hipHeight = Mathf.Clamp(hipOriginalPos.y + Mathf.Lerp(0, heightDiff, t) , 0.5f, 1.2f);
                //Debug.Log("Hip Height: " + hipHeight+" Original: "+ hipOriginalPos.y);
                //Debug.Log("rightHandTarget.position.y: " + rightHandTarget.position.y);
                //Debug.Log("rightHandObj.position.y: " + rightHandObj.position.y);
                //Debug.Log("Diff: " + (rightHandTarget.position.y - rightHandObj.position.y));

                //Debug.Log("Dir: " + relativePos);
                //if(Mathf.Abs(hipTargetOldPos.y - hipHeight) >0.15)
                {
                    hipTarget.position = new Vector3(hipTarget.position.x, hipHeight, hipTarget.position.z);
                    hipTargetOldPos = hipTarget.position;
                }
            }
        }

        if (ikActive)
        {
            Vector3 relativePos = babyMouthPos.position - hipObj.position;
            Vector3 relativePosAlongY = new Vector3(relativePos.x, 0, relativePos.z);

            // the second argument, upwards, defaults to Vector3.up
            Quaternion rotation = Quaternion.LookRotation(relativePosAlongY, Vector3.up);
            //hipTarget.rotation = rotation;

            Vector3 eulerRot = rotation.eulerAngles;

            //eulerRot.z -= 90;
            //eulerRot.y += 90;
            //hipRotTarget.position = hipTarget.position;
            hipRotTarget.rotation = Quaternion.Euler(eulerRot);

            relativePos = babyMouthPos.position - spineObj.position;

            //Vector3 relativePosAlongX = new Vector3(relativePos.x, relativePos.y, 0);
            Vector3 relativePosAlongX = new Vector3(0, relativePos.y, relativePos.z);
            relativePosAlongY = new Vector3(relativePos.x, 0, relativePos.y);

            // the second argument, upwards, defaults to Vector3.up
            Quaternion rotationX = Quaternion.LookRotation(relativePosAlongX, Vector3.up);
            Quaternion rotationY = Quaternion.LookRotation(relativePosAlongY, Vector3.up);
            //hipTarget.rotation = rotation;

            eulerRot.x = rotationX.eulerAngles.x;
            eulerRot.y = rotationY.eulerAngles.y;

            spineRotTarget.position = spineObj.position;
            spineRotTarget.rotation = Quaternion.Euler(eulerRot);

            rightWristRotTarget.position = babyMouthPos.position;
        }


        //Debug.Log(ikActive);
        
        if(ikActive && headRig.weight != 1)
        {
            float weight = Mathf.Lerp(0, 1, t);
            headRig.weight = weight;
            armRig.weight = weight;
            feetRig.weight = weight;
            hipRig.weight = weight;
            hipRotRig.weight = weight;
        } else if (!ikActive && headRig.weight != 0)
        {
            float weight = Mathf.Lerp(1, 0, t);
            headRig.weight = weight;
            armRig.weight = weight;
            feetRig.weight = weight;
            hipRig.weight = weight;
            hipRotRig.weight = weight;
        }
        
        //Debug.Log("Late update");
        //lfootTarget.position -= Vector3.down * hipOffset;
        //animator.bodyPosition += Vector3.down * hipOffset;
    }
}
