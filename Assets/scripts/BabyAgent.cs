using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents.Sensor;
using MLAgents;
using DAIVID;

public class BabyAgent : Agent
{
    public Transform hips;
    public Transform chest;
    public Transform spine;
    public Transform head;
    public Transform thighL;
    public Transform shinL;
    public Transform footL;
    public Transform thighR;
    public Transform shinR;
    public Transform footR;
    public Transform upperArmL;
    public Transform lowerArmL;
    public Transform handL;
    public Transform upperArmR;
    public Transform lowerArmR;
    public Transform handR;
    public Transform eyeL;
    public Transform eyeR;

    public Transform thumR;
    public Transform indR;
    public Transform midR;
    public Transform rinR;
    public Transform lilR;

    public Transform thumiR;
    public Transform indiR;
    public Transform midiR;
    public Transform riniR;
    public Transform liliR;

    public Transform thumL;
    public Transform indL;
    public Transform midL;
    public Transform rinL;
    public Transform lilL;

    public Transform thumiL;
    public Transform indiL;
    public Transform midiL;
    public Transform riniL;
    public Transform liliL;

    JointDriveController m_JdController;
    VisionController m_VisionController;
    //TouchSensorController m_TouchController;
    TouchSensorControllerV2 m_TouchController;

    Rigidbody m_HipsRb;
    Rigidbody m_ChestRb;
    Rigidbody m_SpineRb;

    IFloatProperties m_ResetParams;
    public override void InitializeAgent()
    {
        m_JdController = GetComponent<JointDriveController>();
        m_JdController.SetupBodyPart(hips);
        m_JdController.SetupBodyPart(chest);
        m_JdController.SetupBodyPart(spine);
        m_JdController.SetupBodyPart(head);
        m_JdController.SetupBodyPart(thighL);
        m_JdController.SetupBodyPart(shinL);
        m_JdController.SetupBodyPart(footL);
        m_JdController.SetupBodyPart(thighR);
        m_JdController.SetupBodyPart(shinR);
        m_JdController.SetupBodyPart(footR);
        m_JdController.SetupBodyPart(upperArmL);
        m_JdController.SetupBodyPart(lowerArmL);
        m_JdController.SetupBodyPart(handL);
        m_JdController.SetupBodyPart(upperArmR);
        m_JdController.SetupBodyPart(lowerArmR);
        m_JdController.SetupBodyPart(handR);

        m_HipsRb = hips.GetComponent<Rigidbody>();
        m_ChestRb = chest.GetComponent<Rigidbody>();
        m_SpineRb = spine.GetComponent<Rigidbody>();

        //right fingers
        m_JdController.SetupBodyPart(thumR);
        m_JdController.SetupBodyPart(thumiR);
        m_JdController.SetupBodyPart(indR);
        m_JdController.SetupBodyPart(indiR);
        m_JdController.SetupBodyPart(midR);
        m_JdController.SetupBodyPart(midiR);
        m_JdController.SetupBodyPart(rinR);
        m_JdController.SetupBodyPart(riniR);
        m_JdController.SetupBodyPart(lilR);
        m_JdController.SetupBodyPart(liliR);

        //left fingers
        m_JdController.SetupBodyPart(thumL);
        m_JdController.SetupBodyPart(thumiL);
        m_JdController.SetupBodyPart(indL);
        m_JdController.SetupBodyPart(indiL);
        m_JdController.SetupBodyPart(midL);
        m_JdController.SetupBodyPart(midiL);
        m_JdController.SetupBodyPart(rinL);
        m_JdController.SetupBodyPart(riniL);
        m_JdController.SetupBodyPart(lilL);
        m_JdController.SetupBodyPart(liliL);

        m_VisionController = GetComponent<VisionController>();
        if (m_VisionController == null)
        {
            Debug.LogError("Agent doesn't have any vision controller attached.");
        }

        m_VisionController.SetupEye(eyeL);
        m_VisionController.SetupEye(eyeR);

        m_TouchController = GetComponent<TouchSensorControllerV2>();//GetComponent<TouchSensorController>();
        if (m_TouchController == null)
        {
            Debug.LogError("Agent doesn't have any Touch sensor controller attached.");
        }

        m_ResetParams = Academy.Instance.FloatProperties;

        SetResetParameters();

    }

    private void Start()
    {
        if (m_TouchController != null)
        {
            GetComponent<BehaviorParameters>().brainParameters.vectorObservationSize += m_TouchController.GetSensorCounts(m_JdController.bodyPartsDict.Keys);
        }
    }

    public void SetTorsoMass()
    {
        m_ChestRb.mass = m_ResetParams.GetPropertyWithDefault("chest_mass", 5);
        m_SpineRb.mass = m_ResetParams.GetPropertyWithDefault("spine_mass", 7);
        m_HipsRb.mass = m_ResetParams.GetPropertyWithDefault("hip_mass", 6);
    }

    public void SetResetParameters()
    {
        SetTorsoMass();
    }

    /// <summary>
    /// Loop over body parts to add them to observation.
    /// </summary>
    public override void CollectObservations()
    {

        //babyHeadCam.
        m_JdController.GetCurrentJointForces();

        //sensor.AddObservation(m_DirToTarget.normalized);
        AddVectorObs(m_JdController.bodyPartsDict[hips].rb.position);
        AddVectorObs(hips.forward);
        AddVectorObs(hips.up);

        foreach (var bodyPart in m_JdController.bodyPartsDict.Values)
        {
            CollectObservationBodyPart(bodyPart);
        }

        //grab object status
        //AddVectorObs(m_GrabObservation.CollectGrabstatus());

        //Touch sensor status
        if (m_TouchController != null)
        {
            AddVectorObs(m_TouchController.CollectTouchUpdatesForBodyParts(m_JdController.bodyPartsDict.Keys));
        }
    }

    /// <summary>
    /// Add relevant information on each body part to observations.
    /// </summary>
    public void CollectObservationBodyPart(BodyPart bp)
    {
        var rb = bp.rb;
        AddVectorObs(rb.velocity);
        AddVectorObs(rb.angularVelocity);
        var localPosRelToHips = hips.InverseTransformPoint(rb.position);
        AddVectorObs(localPosRelToHips);


        if (bp.rb.transform != hips && bp.rb.transform != head)
        {
            AddVectorObs(bp.currentXNormalizedRot);
            AddVectorObs(bp.currentYNormalizedRot);
            AddVectorObs(bp.currentZNormalizedRot);
            AddVectorObs(bp.currentStrength / m_JdController.maxJointForceLimit);
        }
    }

    /// <summary>
    /// Loop over body parts and reset them to initial conditions.
    /// </summary>
    public override void AgentReset()
    {
        //if (m_DirToTarget != Vector3.zero)
        //{
        //    transform.rotation = Quaternion.LookRotation(m_DirToTarget);
        //}

        foreach (var bodyPart in m_JdController.bodyPartsDict.Values)
        {
            bodyPart.Reset(bodyPart);
        }
        SetResetParameters();
    }

    public override void AgentAction(float[] vectorAction)
    {
        //Debug.Break();
        var bpDict = m_JdController.bodyPartsDict;
        var i = -1;

        bpDict[chest].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], vectorAction[++i]);
        bpDict[spine].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], vectorAction[++i]);

        bpDict[thighL].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
        bpDict[thighR].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
        bpDict[shinL].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[shinR].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[footR].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], vectorAction[++i]);
        bpDict[footL].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], vectorAction[++i]);


        bpDict[upperArmL].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
        bpDict[upperArmR].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
        bpDict[lowerArmL].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[lowerArmR].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[head].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);

        //update joint strength settings
        bpDict[chest].SetJointStrength(vectorAction[++i]);
        bpDict[spine].SetJointStrength(vectorAction[++i]);
        bpDict[head].SetJointStrength(vectorAction[++i]);
        bpDict[thighL].SetJointStrength(vectorAction[++i]);
        bpDict[shinL].SetJointStrength(vectorAction[++i]);
        bpDict[footL].SetJointStrength(vectorAction[++i]);
        bpDict[thighR].SetJointStrength(vectorAction[++i]);
        bpDict[shinR].SetJointStrength(vectorAction[++i]);
        bpDict[footR].SetJointStrength(vectorAction[++i]);
        bpDict[upperArmL].SetJointStrength(vectorAction[++i]);
        bpDict[lowerArmL].SetJointStrength(vectorAction[++i]);
        bpDict[upperArmR].SetJointStrength(vectorAction[++i]);
        bpDict[lowerArmR].SetJointStrength(vectorAction[++i]);

        m_VisionController.SetEyeRotation(eyeL, eyeR, vectorAction[++i], vectorAction[++i], vectorAction[++i]);

        //Right hand
        bpDict[handR].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
        bpDict[thumR].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[thumiR].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[indR].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[indiR].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[midR].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[midiR].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[rinR].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[riniR].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[lilR].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[liliR].SetJointTargetRotation(vectorAction[++i], 0, 0);

        //Left hand
        bpDict[handL].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
        bpDict[thumL].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[thumiL].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[indL].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[indiL].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[midL].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[midiL].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[rinL].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[riniL].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[lilL].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[liliL].SetJointTargetRotation(vectorAction[++i], 0, 0);


    }

    public override float[] Heuristic()
    {
        var action = new float[69];
        //Debug.Log("Called hurestic");

        for (int i = 0; i < 39; i++)
        {
            action[i] = 0; Random.Range(-1.0f, 1.0f);
        }
        for (int i = 39; i < 41; i++)
        {
            action[i] =  Random.Range(-1.0f, 1.0f);
        }

        action[41] = Random.Range(0f, 1.0f);  // Focal distance

        
        for (int i = 42; i < 69; i++)
        {
            action[i] = Random.Range(-1.0f, 1.0f); //1;
        }

        //Debug.Log("Heuristic");

        //action[39] = -1;

        return action;
    }
}

