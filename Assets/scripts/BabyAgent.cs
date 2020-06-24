using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents.Sensors;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using DAIVID;

public class BabyAgent : Agent
{
    [Header("Body parts")]
    public Transform hips;
    public Transform chest;
    public Transform spine;
    [Header("Head")]
    public Transform head;
    public Transform eyeL;
    public Transform eyeR;

    //Legs
    //Left
    [Header("Leg:Left parts")]
    public Transform thighL;
    public Transform shinL;
    public Transform footL;
    //Right
    [Header("Leg:Right parts")]
    public Transform thighR;
    public Transform shinR;
    public Transform footR;

    //Hands
    //Left
    [Header("Hand:Left parts")]
    public Transform upperArmL;
    public Transform lowerArmL;
    public Transform handL;
    public Transform thum1L;
    public Transform thum2L;
    public Transform ind1L;
    public Transform ind2L;
    public Transform mid1L;
    public Transform mid2L;
    public Transform rin1L;
    public Transform rin2L;
    public Transform lil1L;
    public Transform lil2L;
    //Right
    [Header("Hand:Right parts")]
    public Transform upperArmR;
    public Transform lowerArmR;
    public Transform handR;
    public Transform thum1R;
    public Transform thum2R;
    public Transform ind1R;
    public Transform ind2R;
    public Transform mid1R;
    public Transform mid2R;
    public Transform rin1R;
    public Transform rin2R;
    public Transform lil1R;
    public Transform lil2R;

    JointDriveController m_JdController;
    VisionController m_VisionController;
    //TouchSensorController m_TouchController;
    TouchSensorControllerV2 m_TouchController;

    Rigidbody m_HipsRb;
    Rigidbody m_ChestRb;
    Rigidbody m_SpineRb;

    EnvironmentParameters m_ResetParams;
    public override void Initialize()
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
        m_JdController.SetupBodyPart(thum1R);
        m_JdController.SetupBodyPart(thum2R);
        m_JdController.SetupBodyPart(ind1R);
        m_JdController.SetupBodyPart(ind2R);
        m_JdController.SetupBodyPart(mid1R);
        m_JdController.SetupBodyPart(mid2R);
        m_JdController.SetupBodyPart(rin1R);
        m_JdController.SetupBodyPart(rin2R);
        m_JdController.SetupBodyPart(lil1R);
        m_JdController.SetupBodyPart(lil2R);

        //left fingers
        m_JdController.SetupBodyPart(thum1L);
        m_JdController.SetupBodyPart(thum2L);
        m_JdController.SetupBodyPart(ind1L);
        m_JdController.SetupBodyPart(ind2L);
        m_JdController.SetupBodyPart(mid1L);
        m_JdController.SetupBodyPart(mid2L);
        m_JdController.SetupBodyPart(rin1L);
        m_JdController.SetupBodyPart(rin2L);
        m_JdController.SetupBodyPart(lil1L);
        m_JdController.SetupBodyPart(lil2L);

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

        m_ResetParams = Academy.Instance.EnvironmentParameters;

        SetResetParameters();

    }

    private void Start()
    {
        if (m_TouchController != null)
        {
            GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize += m_TouchController.GetSensorCounts(m_JdController.bodyPartsDict.Keys);
        }
    }

    public void SetTorsoMass()
    {
        m_ChestRb.mass = m_ResetParams.GetWithDefault("chest_mass", 5);
        m_SpineRb.mass = m_ResetParams.GetWithDefault("spine_mass", 7);
        m_HipsRb.mass = m_ResetParams.GetWithDefault("hip_mass", 6);
    }

    public void SetResetParameters()
    {
        SetTorsoMass();
    }

    /// <summary>
    /// Loop over body parts to add them to observation.
    /// </summary>
    public override void CollectObservations(VectorSensor sensor)
    {
        //babyHeadCam.
        m_JdController.GetCurrentJointForces();

        //sensor.AddObservation(m_DirToTarget.normalized);
        sensor.AddObservation(m_JdController.bodyPartsDict[hips].rb.position);
        sensor.AddObservation(hips.forward);
        sensor.AddObservation(hips.up);

        foreach (var bodyPart in m_JdController.bodyPartsDict.Values)
        {
            CollectObservationBodyPart(sensor, bodyPart);
        }

        //grab object status
        //AddVectorObs(m_GrabObservation.CollectGrabstatus());

        //Touch sensor status
        if (m_TouchController != null)
        {
            sensor.AddObservation(m_TouchController.CollectTouchUpdatesForBodyParts(m_JdController.bodyPartsDict.Keys));
        }
    }

    /// <summary>
    /// Add relevant information on each body part to observations.
    /// </summary>
    public void CollectObservationBodyPart(VectorSensor sensor, BodyPart bp)
    {
        var rb = bp.rb;
        sensor.AddObservation(rb.velocity);
        sensor.AddObservation(rb.angularVelocity);
        var localPosRelToHips = hips.InverseTransformPoint(rb.position);
        sensor.AddObservation(localPosRelToHips);


        if (bp.rb.transform != hips && bp.rb.transform != head)
        {
            sensor.AddObservation(bp.currentXNormalizedRot);
            sensor.AddObservation(bp.currentYNormalizedRot);
            sensor.AddObservation(bp.currentZNormalizedRot);
            sensor.AddObservation(bp.currentStrength / m_JdController.maxJointForceLimit);
        }
    }

    /// <summary>
    /// Loop over body parts and reset them to initial conditions.
    /// </summary>
    public override void OnEpisodeBegin()
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

    private float TORQUE_SCALE = .15f;

    public override void OnActionReceived(float[] vectorAction)
    {
        //Debug.Break();
        var bpDict = m_JdController.bodyPartsDict;
        var i = -1;

        spine.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i] * 50 * TORQUE_SCALE, vectorAction[++i] * 50 * TORQUE_SCALE, vectorAction[++i] * 50 * TORQUE_SCALE);

        thighL.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i] * 50 * TORQUE_SCALE, vectorAction[++i] * 50 * TORQUE_SCALE, 0);
        thighR.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i] * 50 * TORQUE_SCALE, vectorAction[++i] * 50 * TORQUE_SCALE, 0);
        shinL.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i] * 50 * TORQUE_SCALE, 0, 0);
        shinR.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i] * 50 * TORQUE_SCALE, 0, 0);
        footR.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i] * 25 * TORQUE_SCALE, vectorAction[++i] * 25 * TORQUE_SCALE, vectorAction[++i] * 25 * TORQUE_SCALE);
        footL.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i] * 25 * TORQUE_SCALE, vectorAction[++i] * 25 * TORQUE_SCALE, vectorAction[++i] * 25 * TORQUE_SCALE);


        upperArmL.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i] * 10 * TORQUE_SCALE, vectorAction[++i] * 5 * TORQUE_SCALE, 0);
        upperArmR.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i] * 10 * TORQUE_SCALE, vectorAction[++i] * 5 * TORQUE_SCALE, 0);
        lowerArmL.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i] * 5 * TORQUE_SCALE, 0, 0);
        lowerArmR.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i] * 5 * TORQUE_SCALE, 0, 0);
        head.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i] * 20 * TORQUE_SCALE, vectorAction[++i] * 20 * TORQUE_SCALE, 0);

        ////Left hand
        handL.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i] * 5 * TORQUE_SCALE, vectorAction[++i] * 5 * TORQUE_SCALE, 0);
        //thum1L.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i], 0, 0);
        //thum2L.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i], 0, 0);
        //ind1L.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i], 0, 0);
        //ind2L.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i], 0, 0);
        //mid1L.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i], 0, 0);
        //mid2L.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i], 0, 0);
        //rin1L.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i], 0, 0);
        //rin2L.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i], 0, 0);
        //lil1L.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i], 0, 0);
        //lil2L.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i], 0, 0);

        //////Right hand
        handR.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i] * 5 * TORQUE_SCALE, vectorAction[++i] * 5 * TORQUE_SCALE, 0);
        //thum1R.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i], 0, 0);
        //thum2R.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i], 0, 0);
        //ind1R.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i], 0, 0);
        //ind2R.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i], 0, 0);
        //mid1R.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i], 0, 0);
        //mid2R.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i], 0, 0);
        //rin1R.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i], 0, 0);
        //rin2R.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i], 0, 0);
        //lil1R.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i], 0, 0);
        //lil2R.GetComponent<Rigidbody>().AddRelativeTorque(vectorAction[++i], 0, 0);


        //bpDict[chest].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], vectorAction[++i]);
        //bpDict[spine].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], vectorAction[++i]);

        //bpDict[thighL].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
        //bpDict[thighR].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
        //bpDict[shinL].SetJointTargetRotation(vectorAction[++i], 0, 0);
        //bpDict[shinR].SetJointTargetRotation(vectorAction[++i], 0, 0);
        //bpDict[footR].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], vectorAction[++i]);
        //bpDict[footL].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], vectorAction[++i]);


        //bpDict[upperArmL].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
        //bpDict[upperArmR].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
        //bpDict[lowerArmL].SetJointTargetRotation(vectorAction[++i], 0, 0);
        //bpDict[lowerArmR].SetJointTargetRotation(vectorAction[++i], 0, 0);
        //bpDict[head].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);

        ////update joint strength settings
        //bpDict[chest].SetJointStrength(vectorAction[++i]);
        //bpDict[spine].SetJointStrength(vectorAction[++i]);
        //bpDict[head].SetJointStrength(vectorAction[++i]);
        //bpDict[thighL].SetJointStrength(vectorAction[++i]);
        //bpDict[shinL].SetJointStrength(vectorAction[++i]);
        //bpDict[footL].SetJointStrength(vectorAction[++i]);
        //bpDict[thighR].SetJointStrength(vectorAction[++i]);
        //bpDict[shinR].SetJointStrength(vectorAction[++i]);
        //bpDict[footR].SetJointStrength(vectorAction[++i]);
        //bpDict[upperArmL].SetJointStrength(vectorAction[++i]);
        //bpDict[lowerArmL].SetJointStrength(vectorAction[++i]);
        //bpDict[upperArmR].SetJointStrength(vectorAction[++i]);
        //bpDict[lowerArmR].SetJointStrength(vectorAction[++i]);

        ////Left hand
        //bpDict[handL].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
        //bpDict[thum1L].SetJointTargetRotation(vectorAction[++i], 0, 0);
        //bpDict[thum2L].SetJointTargetRotation(vectorAction[++i], 0, 0);
        //bpDict[ind1L].SetJointTargetRotation(vectorAction[++i], 0, 0);
        //bpDict[ind2L].SetJointTargetRotation(vectorAction[++i], 0, 0);
        //bpDict[mid1L].SetJointTargetRotation(vectorAction[++i], 0, 0);
        //bpDict[mid2L].SetJointTargetRotation(vectorAction[++i], 0, 0);
        //bpDict[rin1L].SetJointTargetRotation(vectorAction[++i], 0, 0);
        //bpDict[rin2L].SetJointTargetRotation(vectorAction[++i], 0, 0);
        //bpDict[lil1L].SetJointTargetRotation(vectorAction[++i], 0, 0);
        //bpDict[lil2L].SetJointTargetRotation(vectorAction[++i], 0, 0);

        ////Right hand
        //bpDict[handR].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
        //bpDict[thum1R].SetJointTargetRotation(vectorAction[++i], 0, 0);
        //bpDict[thum2R].SetJointTargetRotation(vectorAction[++i], 0, 0);
        //bpDict[ind1R].SetJointTargetRotation(vectorAction[++i], 0, 0);
        //bpDict[ind2R].SetJointTargetRotation(vectorAction[++i], 0, 0);
        //bpDict[mid1R].SetJointTargetRotation(vectorAction[++i], 0, 0);
        //bpDict[mid2R].SetJointTargetRotation(vectorAction[++i], 0, 0);
        //bpDict[rin1R].SetJointTargetRotation(vectorAction[++i], 0, 0);
        //bpDict[rin2R].SetJointTargetRotation(vectorAction[++i], 0, 0);
        //bpDict[lil1R].SetJointTargetRotation(vectorAction[++i], 0, 0);
        //bpDict[lil2R].SetJointTargetRotation(vectorAction[++i], 0, 0);

        m_VisionController.SetEyeRotation(eyeL, eyeR, vectorAction[++i], vectorAction[++i], vectorAction[++i]);

    }

    public override void Heuristic(float[] actionsOut)
    {
        //Debug.Log("Called hurestic");

        for (int i = 0; i < 39; i++)
        {
            actionsOut[i] = Random.Range(-1.0f, 1f);
        }
        //Hands
        for (int i = 39; i < 63; i++)
        {
            actionsOut[i] = Random.Range(-1.0f, 1.0f); //1;
        }
        //Eyes
        for (int i = 63; i < 65; i++)
        {
            actionsOut[i] = Random.Range(-1.0f, 1.0f);
        }

        actionsOut[65] = Random.Range(0f, 1.0f);  // Focal distance
        //Debug.Log("Heuristic");

        //action[39] = -1;
        //Time.timeScale = 0.3f;
    }
}

