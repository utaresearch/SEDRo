using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using MLAgents;

namespace DAIVID
{
    /// <summary>
    /// Used to store relevant information for acting and learning for eyes of agent.
    /// </summary>
    [System.Serializable]
    public class Eye
    {
        [HideInInspector] public Quaternion startingRot;

        [HideInInspector] public VisionController thisVisionController;

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
        /// Reset eye to initial configuration.
        /// </summary>
        public void Reset()
        {
            transform.rotation = startingRot;

        }

        /// <summary>
        /// Change the rotation of eyeball object
        /// </summary>
        public void SetEyeTargetRotation(float x, float y, float z)
        {
            x = (x + 1f) * 0.5f;
            y = (y + 1f) * 0.5f;
            z = (z + 1f) * 0.5f;

            var xRot = Mathf.Lerp(-maxAscendingRotationAngle, maxDescendingRotationAngle, x);
            var yRot = Mathf.Lerp(-maxLeftRotationAngle, maxRightRotationAngle, y);
            //var focalLength = Mathf.Lerp(-joint.angularZLimit.limit, joint.angularZLimit.limit, z);

            transform.localRotation = Quaternion.Euler(xRot,yRot,0);
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
