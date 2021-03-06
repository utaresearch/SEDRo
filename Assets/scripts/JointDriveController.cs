﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DAIVID
{
    /// <summary>
    /// Used to store relevant information for acting and learning for each body part in agent.
    /// </summary>
    [System.Serializable]
    public class BodyPart
    {
        [Header("Body Part Info")] [Space(10)] public ConfigurableJoint joint;
        public Rigidbody rb;
        [HideInInspector] public Vector3 startingPos;
        [HideInInspector] public Quaternion startingRot;

        [FormerlySerializedAs("thisJDController")]
        [HideInInspector] public JointDriveController thisJdController;

        [Header("Current Joint Settings")]
        [Space(10)]
        public Vector3 currentEularJointRotation;

        [HideInInspector] public float currentStrength;
        public float currentXNormalizedRot;
        public float currentYNormalizedRot;
        public float currentZNormalizedRot;

        //This limit of change is in degree
        private float maxAllowableChangePerStep = 5;

        [Header("Other Debug Info")]
        [Space(10)]
        public Vector3 currentJointForce;

        public float currentJointForceSqrMag;
        public Vector3 currentJointTorque;
        public float currentJointTorqueSqrMag;
        public AnimationCurve jointForceCurve = new AnimationCurve();
        public AnimationCurve jointTorqueCurve = new AnimationCurve();

        public Vector3 maxTorque;
        public Vector3 currentNormalizedTorque = new Vector3(0, 0, 0);

        /// <summary>
        /// Reset body part to initial configuration.
        /// </summary>
        public void Reset(BodyPart bp)
        {
            bp.rb.transform.position = bp.startingPos;
            bp.rb.transform.rotation = bp.startingRot;
            bp.rb.velocity = Vector3.zero;
            bp.rb.angularVelocity = Vector3.zero;

        }

        /// <summary>
        /// Apply torque according to defined goal `x, y, z` angle and force `strength`.
        /// </summary>
        /// 

        public void SetJointTargetRotation(float x, float y, float z)
        {
            x = (x + 1f) * 0.5f;
            y = (y + 1f) * 0.5f;
            z = (z + 1f) * 0.5f;

            Vector3 prev = currentEularJointRotation;

            var xRot = Mathf.Lerp(-maxAllowableChangePerStep, maxAllowableChangePerStep, x);
            var yRot = Mathf.Lerp(-maxAllowableChangePerStep, maxAllowableChangePerStep, y);
            var zRot = Mathf.Lerp(-maxAllowableChangePerStep, maxAllowableChangePerStep, z);

            xRot = Mathf.Clamp(prev.x + xRot, joint.lowAngularXLimit.limit, joint.highAngularXLimit.limit);
            yRot = Mathf.Clamp(prev.y + yRot, -joint.angularYLimit.limit, joint.angularYLimit.limit);
            zRot = Mathf.Clamp(prev.z + zRot, -joint.angularZLimit.limit, joint.angularZLimit.limit);

            currentXNormalizedRot =
                Mathf.InverseLerp(joint.lowAngularXLimit.limit, joint.highAngularXLimit.limit, xRot);
            currentYNormalizedRot = Mathf.InverseLerp(-joint.angularYLimit.limit, joint.angularYLimit.limit, yRot);
            currentZNormalizedRot = Mathf.InverseLerp(-joint.angularZLimit.limit, joint.angularZLimit.limit, zRot);

            joint.targetRotation = Quaternion.Euler(xRot, yRot, zRot);
            currentEularJointRotation = new Vector3(xRot, yRot, zRot);
        }

        // <summary>
        /// Apply torque according to defined goal `x, y, z` angle.
        /// </summary>
        /// 

        public void SetJointTorque(float x, float y, float z)
        {
            currentNormalizedTorque.x = maxTorque.x == 0 ? 0 : x / maxTorque.x;
            currentNormalizedTorque.y = maxTorque.y == 0 ? 0 : y / maxTorque.y;
            currentNormalizedTorque.z = maxTorque.z == 0 ? 0 : z / maxTorque.z;
            rb.AddRelativeTorque(x * maxTorque.x * thisJdController.jointTorqueScale, y * maxTorque.y * thisJdController.jointTorqueScale, z * maxTorque.z * thisJdController.jointTorqueScale);

        }

        public void SetJointStrength(float strength)
        {
            var rawVal = (strength + 1f) * 0.5f * thisJdController.maxJointForceLimit;
            var jd = new JointDrive
            {
                positionSpring = thisJdController.maxJointSpring,
                positionDamper = thisJdController.jointDampen,
                maximumForce = rawVal
            };
            joint.slerpDrive = jd;
            currentStrength = jd.maximumForce;
        }

    }

    public class JointDriveController : MonoBehaviour
    {
        [Header("Joint Drive Settings")]
        [Space(10)]
        public float maxJointSpring;

        public float jointDampen;
        public float maxJointForceLimit;

        //Body joints strength controlling params
        [Range(.3f, 7)]
        public float jointTorqueScale = .3f;
        private const float MIN_Joint_Torque_Scale = .3f;
        private float MAX_Joint_Torque_Scale = 7f;

        float m_FacingDot;

        [HideInInspector] public Dictionary<Transform, BodyPart> bodyPartsDict = new Dictionary<Transform, BodyPart>();

        [HideInInspector] public List<BodyPart> bodyPartsList = new List<BodyPart>();

        public void SetJointStrengthScale(float scale)
        {
            jointTorqueScale = Mathf.Min(MIN_Joint_Torque_Scale + Mathf.Clamp(scale, 0, 1) * (MAX_Joint_Torque_Scale - MIN_Joint_Torque_Scale), MAX_Joint_Torque_Scale);
        }

        /// <summary>
        /// Create BodyPart object and add it to dictionary.
        /// </summary>
        public void SetupBodyPart(Transform t, Vector3 maxTorque)
        {
            var bp = new BodyPart
            {
                rb = t.GetComponent<Rigidbody>(),
                joint = t.GetComponent<ConfigurableJoint>(),
                startingPos = t.position,
                startingRot = t.rotation,
                maxTorque = maxTorque
            };
            bp.rb.maxAngularVelocity = 100;
            bp.thisJdController = this;
            bodyPartsDict.Add(t, bp);
            bodyPartsList.Add(bp);
        }

        public void GetCurrentJointForces()
        {
            foreach (var bodyPart in bodyPartsDict.Values)
            {
                if (bodyPart.joint)
                {
                    bodyPart.currentJointForce = bodyPart.joint.currentForce;
                    bodyPart.currentJointForceSqrMag = bodyPart.joint.currentForce.magnitude;
                    bodyPart.currentJointTorque = bodyPart.joint.currentTorque;
                    bodyPart.currentJointTorqueSqrMag = bodyPart.joint.currentTorque.magnitude;
                    if (Application.isEditor)
                    {
                        if (bodyPart.jointForceCurve.length > 1000)
                        {
                            bodyPart.jointForceCurve = new AnimationCurve();
                        }

                        if (bodyPart.jointTorqueCurve.length > 1000)
                        {
                            bodyPart.jointTorqueCurve = new AnimationCurve();
                        }

                        bodyPart.jointForceCurve.AddKey(Time.time, bodyPart.currentJointForceSqrMag);
                        bodyPart.jointTorqueCurve.AddKey(Time.time, bodyPart.currentJointTorqueSqrMag);
                    }
                }
            }

        }
    }
}
