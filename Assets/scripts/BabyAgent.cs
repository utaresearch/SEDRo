﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Unity.ML
using Unity.MLAgents.Sensors;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using DAIVID;

[RequireComponent(typeof(AudioSource))]
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

    [Header("Mother")]
    public Mother mother;

    JointDriveController m_JdController;
    private EnvironmentCommunicationChannel m_envCommChannel;
    VisionController m_VisionController;
    //TouchSensorController m_TouchController;
    TouchSensorControllerV2 m_TouchController;

    Rigidbody m_HipsRb;
    Rigidbody m_ChestRb;
    Rigidbody m_SpineRb;

    EnvironmentParameters m_ResetParams;



    private float stomachFoodLevel = 1;

    private AudioSource babyVoice;

    /// <summary>
    /// this is the amount of food that will be reduced every second.
    /// Calculated in the following way.
    /// 1 day = 86400 seconds
    /// if feeding n times a day, then every 86400/n seconds, stomach should go empty, unless baby is fed again.
    /// So, toreduce stomachFoodLevel=1, reduce 1/(86400/n) in every step.
    /// initially n is 5 times a day.
    /// </summary>
    private float stomachFoodReductionRate = 1f / (10);
    private const float MIN_FOOD_THRESHOLD = 0.1f;

    private Renderer headRenderer;
    private Color originalHeadColor;
    private Color targetHeadColor;

    public override void Initialize()
    {

        SetupBodyParts();

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

        headRenderer = head.GetComponent<Renderer>();

        originalHeadColor = headRenderer.material.color;
        targetHeadColor = originalHeadColor;

        SetResetParameters();

    }

    private void SetupBodyParts()
    {
        m_JdController = GetComponent<JointDriveController>();

        m_envCommChannel = EnvironmentCommunicationChannel.Instance;
        Dictionary<string, float> bodyConfig = m_envCommChannel.GetAgentBodyConfig();
        m_envCommChannel.SendExtraPython("Body config received");

        if (bodyConfig != null)
        {
            m_JdController.SetJointStrengthScale(EnvironmentController.Instance.GetCurrentDay() / 365f); //Scaling strength upto 1 year.
            m_JdController.SetupBodyPart(hips, new Vector3(0, 0, 0));
            Vector3 bodyTorque = new Vector3(0, 0, 0)
            {
                x = bodyConfig.ContainsKey(CommMessageKeys.chestX) ? bodyConfig[CommMessageKeys.chestX] : 0,
                y = bodyConfig.ContainsKey(CommMessageKeys.chestX) ? bodyConfig[CommMessageKeys.chestY] : 0,
                z = bodyConfig.ContainsKey(CommMessageKeys.chestX) ? bodyConfig[CommMessageKeys.chestZ] : 0
            };
            m_JdController.SetupBodyPart(chest, bodyTorque);

            bodyTorque.x = bodyConfig.ContainsKey(CommMessageKeys.spineX) ? bodyConfig[CommMessageKeys.spineX] : 0;
            bodyTorque.y = bodyConfig.ContainsKey(CommMessageKeys.spineY) ? bodyConfig[CommMessageKeys.spineY] : 0;
            bodyTorque.z = bodyConfig.ContainsKey(CommMessageKeys.spineZ) ? bodyConfig[CommMessageKeys.spineZ] : 0;
            m_JdController.SetupBodyPart(spine, bodyTorque);

            bodyTorque.x = bodyConfig.ContainsKey(CommMessageKeys.headX) ? bodyConfig[CommMessageKeys.headX] : 0;
            bodyTorque.y = bodyConfig.ContainsKey(CommMessageKeys.headY) ? bodyConfig[CommMessageKeys.headY] : 0;
            bodyTorque.z = bodyConfig.ContainsKey(CommMessageKeys.headZ) ? bodyConfig[CommMessageKeys.headZ] : 0;
            m_JdController.SetupBodyPart(head, bodyTorque);

            Vector3 thighTorque = new Vector3(0, 0, 0)
            {
                x = bodyConfig.ContainsKey(CommMessageKeys.thighX) ? bodyConfig[CommMessageKeys.thighX] : 0,
                y = bodyConfig.ContainsKey(CommMessageKeys.thighY) ? bodyConfig[CommMessageKeys.thighY] : 0
            };

            Vector3 shinTorque = new Vector3(0, 0, 0)
            {
                x = bodyConfig.ContainsKey(CommMessageKeys.shinX) ? bodyConfig[CommMessageKeys.shinX] : 0
            };

            Vector3 footTorque = new Vector3(0, 0, 0)
            {
                x = bodyConfig.ContainsKey(CommMessageKeys.footX) ? bodyConfig[CommMessageKeys.footX] : 0,
                y = bodyConfig.ContainsKey(CommMessageKeys.footY) ? bodyConfig[CommMessageKeys.footY] : 0,
                z = bodyConfig.ContainsKey(CommMessageKeys.footZ) ? bodyConfig[CommMessageKeys.footZ] : 0
            };

            m_JdController.SetupBodyPart(thighL, thighTorque);
            m_JdController.SetupBodyPart(shinL, shinTorque);
            m_JdController.SetupBodyPart(footL, footTorque);
            m_JdController.SetupBodyPart(thighR, thighTorque);
            m_JdController.SetupBodyPart(shinR, shinTorque);
            m_JdController.SetupBodyPart(footR, footTorque);

            Vector3 upperArmTorque = new Vector3(0, 0, 0)
            {
                x = bodyConfig.ContainsKey(CommMessageKeys.upperArmX) ? bodyConfig[CommMessageKeys.upperArmX] : 0,
                y = bodyConfig.ContainsKey(CommMessageKeys.upperArmY) ? bodyConfig[CommMessageKeys.upperArmY] : 0
            };

            Vector3 lowerArmTorque = new Vector3(0, 0, 0)
            {
                x = bodyConfig.ContainsKey(CommMessageKeys.lowerArmX) ? bodyConfig[CommMessageKeys.lowerArmX] : 0
            };

            Vector3 handTorque = new Vector3(0, 0, 0)
            {
                x = bodyConfig.ContainsKey(CommMessageKeys.handX) ? bodyConfig[CommMessageKeys.handX] : 0,
                y = bodyConfig.ContainsKey(CommMessageKeys.handY) ? bodyConfig[CommMessageKeys.handY] : 0
            };


            m_JdController.SetupBodyPart(upperArmL, upperArmTorque);
            m_JdController.SetupBodyPart(lowerArmL, lowerArmTorque);
            m_JdController.SetupBodyPart(handL, handTorque);
            m_JdController.SetupBodyPart(upperArmR, upperArmTorque);
            m_JdController.SetupBodyPart(lowerArmR, lowerArmTorque);
            m_JdController.SetupBodyPart(handR, handTorque);

            Vector3 finger1Torque = new Vector3(0, 0, 0)
            {
                x = bodyConfig.ContainsKey(CommMessageKeys.fingerUpperX) ? bodyConfig[CommMessageKeys.fingerUpperX] : 0,
                z = bodyConfig.ContainsKey(CommMessageKeys.fingerUpperZ) ? bodyConfig[CommMessageKeys.fingerUpperZ] : 0
            };

            Vector3 finger2Torque = new Vector3(0, 0, 0)
            {
                x = bodyConfig.ContainsKey(CommMessageKeys.fingerLowerX) ? bodyConfig[CommMessageKeys.fingerLowerX] : 0
            };

            //right fingers
            m_JdController.SetupBodyPart(thum1R, finger1Torque);
            m_JdController.SetupBodyPart(thum2R, finger2Torque);
            m_JdController.SetupBodyPart(ind1R, finger1Torque);
            m_JdController.SetupBodyPart(ind2R, finger2Torque);
            m_JdController.SetupBodyPart(mid1R, finger1Torque);
            m_JdController.SetupBodyPart(mid2R, finger2Torque);
            m_JdController.SetupBodyPart(rin1R, finger1Torque);
            m_JdController.SetupBodyPart(rin2R, finger2Torque);
            m_JdController.SetupBodyPart(lil1R, finger1Torque);
            m_JdController.SetupBodyPart(lil2R, finger2Torque);

            //left fingers
            m_JdController.SetupBodyPart(thum1L, finger1Torque);
            m_JdController.SetupBodyPart(thum2L, finger2Torque);
            m_JdController.SetupBodyPart(ind1L, finger1Torque);
            m_JdController.SetupBodyPart(ind2L, finger2Torque);
            m_JdController.SetupBodyPart(mid1L, finger1Torque);
            m_JdController.SetupBodyPart(mid2L, finger2Torque);
            m_JdController.SetupBodyPart(rin1L, finger1Torque);
            m_JdController.SetupBodyPart(rin2L, finger2Torque);
            m_JdController.SetupBodyPart(lil1L, finger1Torque);
            m_JdController.SetupBodyPart(lil2L, finger2Torque);
        }
        else
        {
            SetupDefaultBodyTourques();
        }        

        m_HipsRb = hips.GetComponent<Rigidbody>();
        m_ChestRb = chest.GetComponent<Rigidbody>();
        m_SpineRb = spine.GetComponent<Rigidbody>();
    }

    private void SetupDefaultBodyTourques()
    {
        m_JdController.SetJointStrengthScale(EnvironmentController.Instance.GetCurrentDay() / 365f); //Scaling strength upto 1 year.
        m_JdController.SetupBodyPart(hips, new Vector3(0, 0, 0));
        m_JdController.SetupBodyPart(chest, new Vector3(12, 12, 12));
        m_JdController.SetupBodyPart(spine, new Vector3(12, 12, 12));
        m_JdController.SetupBodyPart(head, new Vector3(5, 5, 5));

        m_JdController.SetupBodyPart(thighL, new Vector3(12, 12, 0));
        m_JdController.SetupBodyPart(shinL, new Vector3(12, 0, 0));
        m_JdController.SetupBodyPart(footL, new Vector3(6, 6, 6));
        m_JdController.SetupBodyPart(thighR, new Vector3(12, 12, 0));
        m_JdController.SetupBodyPart(shinR, new Vector3(12, 0, 0));
        m_JdController.SetupBodyPart(footR, new Vector3(6, 6, 6));

        m_JdController.SetupBodyPart(upperArmL, new Vector3(2, 1, 0));
        m_JdController.SetupBodyPart(lowerArmL, new Vector3(1, 0, 0));
        m_JdController.SetupBodyPart(handL, new Vector3(1, 1, 0));
        m_JdController.SetupBodyPart(upperArmR, new Vector3(2, 1, 0));
        m_JdController.SetupBodyPart(lowerArmR, new Vector3(1, 0, 0));
        m_JdController.SetupBodyPart(handR, new Vector3(1, 1, 0));

        /*
         * Total 30 DoF for fingers
         */
        Vector3 finger1Torque = new Vector3(.25f, 0, .25f);
        Vector3 finger2Torque = new Vector3(.25f, 0, 0);
        //right fingers
        m_JdController.SetupBodyPart(thum1R, finger1Torque);
        m_JdController.SetupBodyPart(thum2R, finger2Torque);
        m_JdController.SetupBodyPart(ind1R, finger1Torque);
        m_JdController.SetupBodyPart(ind2R, finger2Torque);
        m_JdController.SetupBodyPart(mid1R, finger1Torque);
        m_JdController.SetupBodyPart(mid2R, finger2Torque);
        m_JdController.SetupBodyPart(rin1R, finger1Torque);
        m_JdController.SetupBodyPart(rin2R, finger2Torque);
        m_JdController.SetupBodyPart(lil1R, finger1Torque);
        m_JdController.SetupBodyPart(lil2R, finger2Torque);

        //left fingers
        m_JdController.SetupBodyPart(thum1L, finger1Torque);
        m_JdController.SetupBodyPart(thum2L, finger2Torque);
        m_JdController.SetupBodyPart(ind1L, finger1Torque);
        m_JdController.SetupBodyPart(ind2L, finger2Torque);
        m_JdController.SetupBodyPart(mid1L, finger1Torque);
        m_JdController.SetupBodyPart(mid2L, finger2Torque);
        m_JdController.SetupBodyPart(rin1L, finger1Torque);
        m_JdController.SetupBodyPart(rin2L, finger2Torque);
        m_JdController.SetupBodyPart(lil1L, finger1Torque);
        m_JdController.SetupBodyPart(lil2L, finger2Torque);
    }

    private void Start()
    {
        if (m_TouchController != null)
        {
            GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize += m_TouchController.GetSensorCounts(m_JdController.bodyPartsDict.Keys);
        }

        babyVoice = GetComponent<AudioSource>();
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

        ////Touch sensor status
        if (m_TouchController != null)
        {
            sensor.AddObservation(m_TouchController.CollectTouchUpdatesForBodyParts(m_JdController.bodyPartsDict.Keys));
        }
        //sensor.AddObservation(stomachFoodLevel);
        //if (mother)
        //{
        //    float[] voice = mother.getVoiceVector();
        //    sensor.AddObservation(voice);

        //}
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
            sensor.AddObservation(bp.currentNormalizedTorque);
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



    public override void OnActionReceived(float[] vectorAction)
    {
        //if(stomachFoodLevel < MIN_FOOD_THRESHOLD)
        //{
        //    return;
        //}

        //float fps = 1.0f/Time.deltaTime;
        //Debug.Log("FPS: " + fps);

        //Debug.Break();
        var bpDict = m_JdController.bodyPartsDict;
        var i = -1;


        bpDict[spine].SetJointTorque(vectorAction[++i], vectorAction[++i], vectorAction[++i]);
        bpDict[chest].SetJointTorque(vectorAction[++i], vectorAction[++i], vectorAction[++i]);
        bpDict[head].SetJointTorque(vectorAction[++i], vectorAction[++i], vectorAction[++i]);

        bpDict[thighL].SetJointTorque(vectorAction[++i], vectorAction[++i], 0);
        bpDict[thighR].SetJointTorque(vectorAction[++i], vectorAction[++i], 0);
        bpDict[shinL].SetJointTorque(vectorAction[++i], 0, 0);
        bpDict[shinR].SetJointTorque(vectorAction[++i], 0, 0);
        bpDict[footR].SetJointTorque(vectorAction[++i], vectorAction[++i], vectorAction[++i]);
        bpDict[footL].SetJointTorque(vectorAction[++i], vectorAction[++i], vectorAction[++i]);


        bpDict[upperArmL].SetJointTorque(vectorAction[++i], vectorAction[++i], 0);
        bpDict[upperArmR].SetJointTorque(vectorAction[++i], vectorAction[++i], 0);
        bpDict[lowerArmL].SetJointTorque(vectorAction[++i], 0, 0);
        bpDict[lowerArmR].SetJointTorque(vectorAction[++i], 0, 0);

        ////Left hand
        bpDict[handL].SetJointTorque(vectorAction[++i], vectorAction[++i], 0);
        bpDict[thum1L].SetJointTorque(vectorAction[++i], 0, vectorAction[++i]);
        bpDict[thum2L].SetJointTorque(vectorAction[++i], 0, 0);
        bpDict[ind1L].SetJointTorque(vectorAction[++i], 0, vectorAction[++i]);
        bpDict[ind2L].SetJointTorque(vectorAction[++i], 0, 0);
        bpDict[mid1L].SetJointTorque(vectorAction[++i], 0, vectorAction[++i]);
        bpDict[mid2L].SetJointTorque(vectorAction[++i], 0, 0);
        bpDict[rin1L].SetJointTorque(vectorAction[++i], 0, vectorAction[++i]);
        bpDict[rin2L].SetJointTorque(vectorAction[++i], 0, 0);
        bpDict[lil1L].SetJointTorque(vectorAction[++i], 0, vectorAction[++i]);
        bpDict[lil2L].SetJointTorque(vectorAction[++i], 0, 0);

        //////Right hand
        bpDict[handR].SetJointTorque(vectorAction[++i], vectorAction[++i], 0);
        bpDict[thum1R].SetJointTorque(vectorAction[++i], 0, vectorAction[++i]);
        bpDict[thum2R].SetJointTorque(vectorAction[++i], 0, 0);
        bpDict[ind1R].SetJointTorque(vectorAction[++i], 0, vectorAction[++i]);
        bpDict[ind2R].SetJointTorque(vectorAction[++i], 0, 0);
        bpDict[mid1R].SetJointTorque(vectorAction[++i], 0, vectorAction[++i]);
        bpDict[mid2R].SetJointTorque(vectorAction[++i], 0, 0);
        bpDict[rin1R].SetJointTorque(vectorAction[++i], 0, vectorAction[++i]);
        bpDict[rin2R].SetJointTorque(vectorAction[++i], 0, 0);
        bpDict[lil1R].SetJointTorque(vectorAction[++i], 0, vectorAction[++i]);
        bpDict[lil2R].SetJointTorque(vectorAction[++i], 0, 0);


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

        m_VisionController.SetEyeRotation(eyeL, eyeR, vectorAction[++i], vectorAction[++i], Mathf.Abs(vectorAction[++i]));

        //Cry vector at 77th index
        //Cry(vectorAction[++i] > 0);

    }

    private Color dangerRedColor = new Color(250/255f, 38/255f, 36/255f);
    float startTime = 0;
    float colorTransitionSpeed = 3;
    private void Update()
    {
        stomachFoodLevel = Mathf.Max(0, stomachFoodLevel - stomachFoodReductionRate * Time.deltaTime);
        //Debug.Log("Reduce rate: " + stomachFoodReductionRate + ", stomach level: " + stomachFoodLevel);

        //Cry(stomachFoodLevel < MIN_FOOD_THRESHOLD);
    }

    void Cry(bool enable)
    {
        if (enable)
        {
            if (!babyVoice.isPlaying)
            {
                babyVoice.Play();
            }
            //Debug.Log("Target color: " + targetHeadColor+" Ping Pong val: "+ Mathf.Abs(Mathf.Sin((Time.time - startTime) * colorTransitionSpeed)));

            if (headRenderer)
            {
                headRenderer.material.color = Color.Lerp(originalHeadColor, dangerRedColor, Mathf.Abs(Mathf.Sin((Time.time - startTime) * colorTransitionSpeed)));
            }
        }
        else
        {
            StartCoroutine(StopCryingDelayed());
        }

    }

    IEnumerator StopCryingDelayed()
    {
        yield return new WaitForSeconds(5);// Until(() => Utility.Vector2DDistance(baby.position, transform.position) <= agent.stoppingDistance);

        startTime = Time.time;
        headRenderer.material.color = originalHeadColor;
        babyVoice.Stop();
    }

    public void feed()
    {
        stomachFoodLevel = 1;
        if (babyVoice.isPlaying)
        {
            babyVoice.Stop();
        }
    }
    bool flag = true;
    public override void Heuristic(float[] actionsOut)
    {
        //Debug.Log("Called hurestic");

        for (int i = 0; i < 27; i++)
        {
            actionsOut[i] = Random.Range(-1.0f, 1f);
        }
        //Hands
        for (int i = 27; i < 61; i++)
        {
            actionsOut[i] = Random.Range(-1.0f, 1.0f); //1;
        }
        //Eyes
        for (int i = 61; i < 63; i++)
        {
            actionsOut[i] = Random.Range(-1.0f, 1.0f);
        }

        actionsOut[63] = Random.Range(0f, 1.0f);  // Focal distance

        //actionsOut[64] = Random.Range(0, 2);  // Cry vector
        //if (flag)
        //{
        //    actionsOut[64] = 1;
        //    flag = false;
        //}
        //else
        //{
        //    actionsOut[64] = 0;
        //}
        //Debug.Log("Heuristic");

        //action[39] = -1;
        //Time.timeScale = 0.3f;
    }
}
