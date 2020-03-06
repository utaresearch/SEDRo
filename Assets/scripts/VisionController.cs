using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using MLAgents;

namespace DAIVID
{
    /// <summary>
    /// Used to store relevant information for acting and learning for each body part in agent.
    /// </summary>
    [System.Serializable]
    public class Eye
    {
        [HideInInspector] public Quaternion startingRot;

        //[FormerlySerializedAs("thisVisionController")]
        [HideInInspector] public VisionController thisVisionController;

        [Header("Current Joint Settings")]
        [Space(10)]
        public Vector3 currentEularJointRotation;

        public float currentXNormalizedRot;
        public float currentYNormalizedRot;
        public float currentZNormalizedRot;

        [Header("Eye Settings")]
        [Space(10)]
        public float maxLeftRotationAngle;
        public float maxRightRotationAngle;
        public float maxAscendingRotationAngle;
        public float maxDescendingRotationAngle;


        [Header("Other Debug Info")]
        public AnimationCurve jointForceCurve = new AnimationCurve();
        public AnimationCurve jointTorqueCurve = new AnimationCurve();

        public Transform transform;
        /// <summary>
        /// Reset body part to initial configuration.
        /// </summary>
        public void Reset()
        {
            transform.rotation = startingRot;

        }

        /// <summary>
        /// Apply torque according to defined goal `x, y, z` angle and force `strength`.
        /// </summary>
        public void SetEyeTargetRotation(float x, float y, float z)
        {
            x = (x + 1f) * 0.5f;
            y = (y + 1f) * 0.5f;
            z = (z + 1f) * 0.5f;

            var xRot = Mathf.Lerp(-maxAscendingRotationAngle, maxDescendingRotationAngle, x);
            var yRot = Mathf.Lerp(-maxLeftRotationAngle, maxRightRotationAngle, y);
            //var focalLength = Mathf.Lerp(-joint.angularZLimit.limit, joint.angularZLimit.limit, z);

            //currentXNormalizedRot =
            //    Mathf.InverseLerp(joint.lowAngularXLimit.limit, joint.highAngularXLimit.limit, xRot);
            //currentYNormalizedRot = Mathf.InverseLerp(-joint.angularYLimit.limit, joint.angularYLimit.limit, yRot);
            //currentZNormalizedRot = Mathf.InverseLerp(-joint.angularZLimit.limit, joint.angularZLimit.limit, zRot);
            //transform.localRotation = transform.rotation;
            transform.localRotation = Quaternion.Euler(xRot,yRot,0);
            //currentEularJointRotation = new Vector3(xRot, transform.rotation, zRot);
        }
        public void SetJointStrength(float strength)
        {
            //var rawVal = (strength + 1f) * 0.5f * thisJdController.maxJointForceLimit;
            //var jd = new JointDrive
            //{
            //    positionSpring = thisJdController.maxJointSpring,
            //    positionDamper = thisJdController.jointDampen,
            //    maximumForce = rawVal
            //};
            //joint.slerpDrive = jd;
            //currentStrength = jd.maximumForce;
        }

    }

    public class VisionController : MonoBehaviour
    {
        [Header("Eye Settings")]
        [Space(10)]
        public float maxLeftRotationAngle;
        public float maxRightRotationAngle;
        public float maxAscendingRotationAngle;
        public float maxDescendingRotationAngle;

        public float focalLength;
        float m_FacingDot;

        [HideInInspector] public Dictionary<Transform, Eye> eyeDict = new Dictionary<Transform, Eye>();

        [HideInInspector] public List<Eye> eyeList = new List<Eye>();

        /// <summary>
        /// Create Eye object and add it to dictionary.
        /// </summary>
        public void SetupEye(Transform t)
        {
            var bp = new Eye
            {
                maxLeftRotationAngle = this.maxLeftRotationAngle,
                maxRightRotationAngle = this.maxRightRotationAngle,
                maxAscendingRotationAngle = this.maxAscendingRotationAngle,
                maxDescendingRotationAngle = this.maxDescendingRotationAngle,
                //joint = t.GetComponent<Eye>()
                startingRot = t.rotation,
                transform = t
            };
            bp.thisVisionController = this;
            eyeDict.Add(t, bp);
            eyeList.Add(bp);
        }

        
    }
}
