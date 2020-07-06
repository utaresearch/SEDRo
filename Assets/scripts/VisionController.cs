using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

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


        [Header("Head and Eye cams")]
        [Space(10)]
        public Camera headPeripheralVisionCam;
        public Camera headCentralVisionCam;
        public Camera leftPeripheralVisionCam;
        public Camera leftCentralVisionCam;
        public Camera rightPeripheralVisionCam;
        public Camera rightCentralVisionCam;

        [HideInInspector] public const int NO_OF_VISION_CAM = 6;

        private Camera[] cameras = new Camera[NO_OF_VISION_CAM];

        //This limit of change is in degree
        private float maxAllowableChangePerStep = 5;
        private Vector3 currentEularGazeRotation;

        [HideInInspector] public Dictionary<Transform, Eye> eyeDict = new Dictionary<Transform, Eye>();

        [HideInInspector] public List<Eye> eyeList = new List<Eye>();

        private Dictionary<Camera, DepthOfField> depthFilterDict = new Dictionary<Camera, DepthOfField>();


        [HideInInspector] public const float MaxEyeAperture = 2.4f;
        [HideInInspector] public const float MinEyeAperture = 9.5f;

        

        private void Awake()
        {
            //maxVisionDistance = 2f;
            FindDepthFilters();
        }

        private void FindDepthFilters()
        {
                cameras[0] = headPeripheralVisionCam;
            cameras[1] = headCentralVisionCam;
            cameras[2] = leftPeripheralVisionCam;
            cameras[3] = leftCentralVisionCam;
            cameras[4] = rightPeripheralVisionCam;
            cameras[5] = rightCentralVisionCam;

            foreach(Camera cam in cameras)
            {
                PostProcessVolume postVol = cam.gameObject.GetComponent<PostProcessVolume>();
                DepthOfField dof = null;
                if (postVol)
                {
                    if (postVol.sharedProfile.TryGetSettings<DepthOfField>(out dof))
                    {
                        Debug.Log("Depth filter found for camera" + cam);
                    }
                }
                depthFilterDict.Add(cam, dof);
            }
            
        }

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

            rightLaser.localRotation = Quaternion.Euler(eyeGazeLaser.localRotation.eulerAngles.x, eyeGazeLaser.localRotation.eulerAngles.y - angleRight, eyeGazeLaser.localRotation.eulerAngles.z);
            rightEye.localRotation = rightLaser.localRotation * Quaternion.Euler(-90, 0, 0);    //Laser angle are rotated by 90 degree. So converting towards the forward of eye

            targetDir = gazeLaserEndPosition - leftEye.localPosition;

            float angleLeft = Vector3.Angle(Vector3.forward, targetDir.normalized);

            leftLaser.localRotation = Quaternion.Euler(eyeGazeLaser.localRotation.eulerAngles.x, eyeGazeLaser.localRotation.eulerAngles.y + angleLeft, eyeGazeLaser.localRotation.eulerAngles.z);
            leftEye.localRotation = leftLaser.localRotation * Quaternion.Euler(-90, 0, 0);    //Laser angle are rotated by 90 degree. So converting towards the forward of eye
            //Debug.Break();
        }

        internal void SetGazeRotation(float x, float y, float visionFocusDistance)
        {
            x = (x + 1f) * 0.5f;
            y = (y + 1f) * 0.5f;
            //z = (z + 1f) * 0.5f;

            visionFocusDistance = Mathf.Abs(visionFocusDistance);
            //visionFocusDistance = 1;

            Vector3 prev = currentEularGazeRotation;

            var xRot = Mathf.Lerp(-maxAllowableChangePerStep, maxAllowableChangePerStep, x);
            var yRot = Mathf.Lerp(-maxAllowableChangePerStep, maxAllowableChangePerStep, y);

            xRot = Mathf.Clamp(prev.x + xRot, -maxAscendingRotationAngle, maxDescendingRotationAngle);
            yRot = Mathf.Clamp(prev.y + yRot, -maxLeftRotationAngle, maxRightRotationAngle);

            //var xRot = Mathf.Lerp(-maxAscendingRotationAngle, maxDescendingRotationAngle, x);
            //var yRot = Mathf.Lerp(-maxLeftRotationAngle, maxRightRotationAngle, y);
            //var focalLength = Mathf.Lerp(-joint.angularZLimit.limit, joint.angularZLimit.limit, z);
            if(visionFocusDistance * maxVisionDistance < 0)
            {
                Debug.LogError("ScaleMode negative" + visionFocusDistance + "::" + maxVisionDistance);
            }

            eyeGazeLaser.localRotation = Quaternion.Euler(xRot + 90, yRot, 0);
            eyeGazeLaser.localScale = new Vector3(eyeGazeLaser.localScale.x, visionFocusDistance * maxVisionDistance, eyeGazeLaser.localScale.z);
            headPeripheralVisionCam.transform.localRotation = Quaternion.Euler(xRot, yRot, 0);
            currentEularGazeRotation = new Vector3(xRot, yRot, 0);

            ApplyDepthOfFieldEffects(visionFocusDistance);
        }

        private void ApplyDepthOfFieldEffects(float visionFocusDistance)
        {
            Vector3 gazeLaserEndPosition = new Vector3(eyeGazeLaser.localPosition.x, eyeGazeLaser.localPosition.y, eyeGazeLaser.localPosition.z + eyeGazeLaser.lossyScale.y * 2);
            Vector3 gazeLaserStartPosition = new Vector3(eyeGazeLaser.localPosition.x, eyeGazeLaser.localPosition.y, eyeGazeLaser.localPosition.z);


            float distance = Vector3.Distance(gazeLaserEndPosition, gazeLaserStartPosition);//(gazeWorldCoord - periCentralCamWorldCoord).magnitude;
            //Debug.Log("Dist-Val:" + distance.ToString("F4"));

            foreach (Camera cam in cameras)
            {
                DepthOfField dof = depthFilterDict[cam];
                if (dof)
                {
                    dof.focusDistance.value = Mathf.Abs(distance);

                    // Limiting the aperture in the range of around 20 meter. So, after 20 meters aperture will be f/9.5. 
                    dof.aperture.value = MaxEyeAperture + Math.Min((visionFocusDistance * maxVisionDistance)/ 20, 1) * (MinEyeAperture - MaxEyeAperture);
                }
            }
        }
    }
}
