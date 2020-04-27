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
        internal void SetEyeTargetRotation(float x, float y, float z)
        {
            x = (x + 1f) * 0.5f;
            y = (y + 1f) * 0.5f;
            z = (z + 1f) * 0.5f;

            var xRot = Mathf.Lerp(-maxAscendingRotationAngle, maxDescendingRotationAngle, x);
            var yRot = Mathf.Lerp(-maxLeftRotationAngle, maxRightRotationAngle, y);
            //var focalLength = Mathf.Lerp(-joint.angularZLimit.limit, joint.angularZLimit.limit, z);

            transform.localRotation = Quaternion.Euler(xRot, yRot, 0);
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
        public float maxVisionDistance;

        public float focalLength;
        float m_FacingDot;

        public Transform eyeGazeLaser;

        //Left and Right lasers are used for debug/test purpose now.
        public Transform rightLaser;
        public Transform leftLaser;

        //This limit of change is in degree
        private float maxAllowableChangePerStep = 5;
        private Vector3 currentEularGazeRotation;

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

        public void SetEyeRotation(Transform leftEye, Transform rightEye, float x, float y, float z)
        {
            //eyeDict[leftEye].SetEyeTargetRotation(x, y, z);
            //eyeDict[rightEye].SetEyeTargetRotation(x, y, z);
            SetGazeRotation(x, y, z);


            Vector3 gazeLaserEndPosition = new Vector3(eyeGazeLaser.localPosition.x, eyeGazeLaser.localPosition.y, eyeGazeLaser.localPosition.z + eyeGazeLaser.localScale.y * 2);


            Vector3 targetDir = gazeLaserEndPosition - rightEye.localPosition;

            float angleRight = Vector3.Angle(Vector3.forward, targetDir.normalized);

            rightLaser.localRotation = Quaternion.Euler(eyeGazeLaser.localRotation.eulerAngles.x, eyeGazeLaser.localRotation.eulerAngles.y -angleRight, eyeGazeLaser.localRotation.eulerAngles.z);
            rightEye.localRotation = rightLaser.localRotation * Quaternion.Euler(-90, 0, 0);    //Laser angle are rotated by 90 degree. So converting towards the forward of eye

            targetDir = gazeLaserEndPosition - leftEye.localPosition;

            float angleLeft = Vector3.Angle(Vector3.forward, targetDir.normalized);

            leftLaser.localRotation = Quaternion.Euler(eyeGazeLaser.localRotation.eulerAngles.x, eyeGazeLaser.localRotation.eulerAngles.y + angleLeft, eyeGazeLaser.localRotation.eulerAngles.z);
            leftEye.localRotation = leftLaser.localRotation * Quaternion.Euler(-90, 0, 0);    //Laser angle are rotated by 90 degree. So converting towards the forward of eye
            //Debug.Break();
        }

        internal void SetGazeRotation(float x, float y, float visionDistance)
        {
            x = (x + 1f) * 0.5f;
            y = (y + 1f) * 0.5f;
            //z = (z + 1f) * 0.5f;

            Vector3 prev = currentEularGazeRotation;

            var xRot = Mathf.Lerp(-maxAllowableChangePerStep, maxAllowableChangePerStep, x);
            var yRot = Mathf.Lerp(-maxAllowableChangePerStep, maxAllowableChangePerStep, y);

            xRot = Mathf.Clamp(prev.x + xRot, maxAscendingRotationAngle, maxDescendingRotationAngle);
            yRot = Mathf.Clamp(prev.y + yRot, -maxLeftRotationAngle, maxRightRotationAngle);

            //var xRot = Mathf.Lerp(-maxAscendingRotationAngle, maxDescendingRotationAngle, x);
            //var yRot = Mathf.Lerp(-maxLeftRotationAngle, maxRightRotationAngle, y);
            //var focalLength = Mathf.Lerp(-joint.angularZLimit.limit, joint.angularZLimit.limit, z);
            if(visionDistance * maxVisionDistance < 0)
            {
                Debug.LogError("ScaleMode negative" + visionDistance + "::" + maxVisionDistance);
            }

            eyeGazeLaser.localRotation = Quaternion.Euler(xRot + 90, yRot, 0);
            eyeGazeLaser.localScale = new Vector3(eyeGazeLaser.localScale.x, visionDistance * maxVisionDistance, eyeGazeLaser.localScale.z);
            currentEularGazeRotation = new Vector3(xRot, yRot, 0);
        }


    }
}
