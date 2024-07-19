using System.Collections;
using UnityEngine;

public class HingeSlapDownIntake : MonoBehaviour, IResettable
{
    [SerializeField] private HingeJoint intake;

    [SerializeField] private float intakeAngle;

    private float ampRotation;

    private bool atTarget;
    public bool isAmping;

    private Vector3 intakeStartingPos;
    private Quaternion intakeStartingRot;

    private int startingLayer;

    private DriveController controller;

    private void Start() 
    {
        intakeStartingPos = intake.gameObject.transform.localPosition;
        intakeStartingRot = intake.gameObject.transform.localRotation;

        startingLayer = intake.gameObject.layer;

        controller = GetComponent<DriveController>();

        ampRotation = intake.gameObject.transform.localEulerAngles.x + 20f;
    }

    private void Update()
    {
        if (!isAmping) 
        {
            if (controller.isIntaking) 
            {
                RotateIntake(intakeAngle);
            }
            else
            {
                StowIntake();
            }
        }
    }

    private void RotateIntake(float targetAngle)
    {
        if (!atTarget)
        {
            JointSpring intakeSpring = intake.spring;
            intakeSpring.targetPosition = intakeAngle;
            intake.spring = intakeSpring;
        }
    }

    public void StowIntake()
    {
        atTarget = false;
        JointSpring intakeSpring = intake.spring;
        intakeSpring.targetPosition = 0;
        intake.spring = intakeSpring;
    }

    public void MoveForAmp()
    {
        RotateToAmpRotation();
    }

    private void RotateToAmpRotation()
    {
        isAmping = true;
        JointSpring intakeSpring = intake.spring;
        intakeSpring.targetPosition = ampRotation;
        intake.spring = intakeSpring;
    }

    private IEnumerator WaitToEnable() 
    {
        yield return new WaitForSeconds(0.01f);
        intake.gameObject.layer = startingLayer;
    }

    public void Reset() 
    {   
        StopAllCoroutines();

        intake.gameObject.layer = 17;

        intake.gameObject.transform.localPosition = intakeStartingPos;
        intake.gameObject.transform.localRotation = intakeStartingRot;

        JointSpring intakeSpring = intake.spring;
        intakeSpring.targetPosition = 0;
        intake.spring = intakeSpring;

        atTarget = false;
        isAmping = false;
        
        StartCoroutine(WaitToEnable());
    }
}
