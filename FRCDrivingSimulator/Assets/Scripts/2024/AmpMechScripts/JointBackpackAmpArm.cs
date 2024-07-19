using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class JointBackpackAmpArm : MonoBehaviour, IResettable
{
    [SerializeField] private RobotSettings robot;
    [SerializeField] private ConfigurableJoint ampStage;
    [SerializeField] private float ampDistance;

    private int startingLayer;

    private Vector3 ampStartingPos;
    private Quaternion ampStartingRot;

    private bool isMoving = false;
    private bool isStowed = true;
    private bool isAtPosition = false;
    private bool moveIntake = true;

    private bool canTrap = true;

    [SerializeField] private RobotNoteManager collisions;

    [SerializeField] private AudioSource source;
    [SerializeField] private AudioResource stall;

    [SerializeField] private GameObject hiddenRing;

    private DriveController robotController;
    private HingeSlapDownIntake intake;

    private void Start()
    {
        hiddenRing.SetActive(false);
        robotController = GetComponent<DriveController>();

        if (robot == RobotSettings.CitrusCircuits) 
        {
            intake = GetComponent<HingeSlapDownIntake>();
        }

        ampStartingPos = ampStage.gameObject.transform.localPosition;
        ampStartingRot = ampStage.gameObject.transform.localRotation;

        startingLayer = ampStage.gameObject.layer;
    }

    private void Update()
    {
        if (collisions.isAmping && !isMoving && !isAtPosition)
        {
            hiddenRing.SetActive(true);
            StartCoroutine(AmpArm());
        }
        else if (!collisions.isAmping && !isMoving && !isStowed)
        {
            StowAmpArm();
        }

        if (hiddenRing.activeSelf && !collisions.hasRingInRobot) 
        {
            hiddenRing.SetActive(false);
        }


        if (isMoving) 
        {
            robotController.canIntake = false;
            if (robot == RobotSettings.CitrusCircuits) 
            {
                if (moveIntake) { intake.MoveForAmp(); moveIntake = false; }
            }
        }
        else 
        {
            if (robot == RobotSettings.CitrusCircuits) 
            {
                if (!moveIntake) { intake.StowIntake(); moveIntake = true; intake.isAmping = false; }
            }
            robotController.canIntake = true;
        }
    }

    private IEnumerator AmpArm()
    {
        isMoving = true;
        isStowed = false;

        hiddenRing.SetActive(true);
        ampStage.targetPosition = new Vector3(0f, ampDistance, 0f);

        source.resource = stall;
        source.Play();
        
        yield return new WaitForSeconds(0.98f);

        isAtPosition = true;
        isMoving = false;
    }


    private void StowAmpArm()
    {
        isAtPosition = false;
        isMoving = true;

        ampStage.targetPosition = new Vector3(0, 0, 0);

        isMoving = false;
        isStowed = true;
    }

    private void ExtendArm()
    {
        isMoving = true;
        isStowed = false;
        hiddenRing.SetActive(true);
        ampStage.targetPosition = new Vector3(0f, ampDistance, 0f);

        source.resource = stall;
        source.Play();
    }

    public void TrapAmpArm() 
    {
        if (canTrap)
        {
            canTrap = false;
            ExtendArm();
        }
    }

    private IEnumerator WaitToEnable() 
    {
        yield return new WaitForSeconds(0.01f);
        ampStage.gameObject.layer = startingLayer;
    }

    public void Reset() 
    {
        StopAllCoroutines();

        ampStage.gameObject.layer = 17;

        ampStage.gameObject.transform.localPosition = ampStartingPos;
        ampStage.gameObject.transform.localRotation = ampStartingRot;
        
        ampStage.targetPosition = new Vector3(0, 0, 0);

        isMoving = false;
        isStowed = true;
        isAtPosition = false;
        moveIntake = true;
        canTrap = true;
        hiddenRing.SetActive(false);

        StartCoroutine(WaitToEnable());
    }
}
