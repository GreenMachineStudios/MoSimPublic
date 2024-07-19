using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class JointRobotAlignToSpeaker : MonoBehaviour, IResettable
{
    [SerializeField] private Alliance alliance;
    [SerializeField] private RobotSettings robot;

    public Transform target;
    public HingeJoint shooterPivot;
    public Transform shooterDirection;
    public Rigidbody robotRigidbody;

    public float robotRotationSpeed = 50f;

    public float angleOffset;

    public float armAmpPos;

    public float maxAimDistance = 40f;

    public bool stowedShooter = false;

    private float alignWholeRobot;

    private Quaternion targetRotation;

    private bool canDoAlign = true;
    private bool amp = false;
    private bool isAmping = false;
    public bool isShooting = false;

    public RobotNoteManager ringCollisions;

    [SerializeField] private float passingAngle;

    private DriveController drive;

    private void Start()
    {
        ringCollisions = gameObject.GetComponent<RobotNoteManager>();
        drive = gameObject.GetComponent<DriveController>();
    }

    private void Update()
    {
        if (GameManager.canRobotMove)
        {
            float distanceToTarget = Vector3.Distance(shooterPivot.transform.position, target.position);

            if (isAmping)
            {
                ringCollisions.canShoot = false;
            }
            else if (stowedShooter && distanceToTarget > maxAimDistance) 
            {
                ringCollisions.canShoot = false;
            }
            else
            {
                ringCollisions.canShoot = true;
            }

             //Handle shooting speeds
            if (distanceToTarget <= maxAimDistance) 
            {
                ringCollisions.speed = ringCollisions.shootingSpeed;
            }
            else 
            {
                ringCollisions.noteDrag = 1.2f;
                ringCollisions.speed = ringCollisions.passingSpeed;
            }

            if (!isAmping && !isShooting)
            {
                if (amp && ringCollisions.hasRingInRobot && !isAmping && !ringCollisions.isShooting)
                {
                    StartCoroutine(AmplifyRotation());
                }
                else 
                {
                    StowShooter();
                }

                if (alignWholeRobot > 0 && canDoAlign && distanceToTarget <= maxAimDistance)
                {
                    canDoAlign = false;
                    RotateRobotToTarget();
                }
                else if (alignWholeRobot == 0)
                {
                    canDoAlign = true;
                }
            }
        }
    }

    IEnumerator AmplifyRotation()
    {
        isAmping = true;
        JointSpring shooterSpring = shooterPivot.spring;
        shooterSpring.targetPosition = armAmpPos;
        shooterPivot.spring = shooterSpring;
        yield return new WaitForSeconds(1f);
        shooterSpring.targetPosition = 0;
        shooterPivot.spring = shooterSpring;
        amp = false;
        isAmping = false;
    }

    private void RotateRobotToTarget()
    {
        if (drive.isGrounded)
        {
            isShooting = true;

            RotateShooter();

            Vector3 directionToTarget = target.position - transform.position;
            directionToTarget.y = 0f;

            targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

            StartCoroutine(RotateTowardsTarget(targetRotation));
        }
    }

    IEnumerator RotateTowardsTarget(Quaternion targetRotation)
    {
        if (alliance == Alliance.Blue) { DriveController.canBlueRotate = false; }
        else { DriveController.canRedRotate = false; }

        while (Quaternion.Angle(robotRigidbody.rotation, targetRotation) > 0.1f)
        {
            robotRigidbody.rotation = Quaternion.RotateTowards(robotRigidbody.rotation, targetRotation, robotRotationSpeed * Time.deltaTime);
            yield return null;
        }

        if (alliance == Alliance.Blue) { DriveController.canBlueRotate = true; }
        else { DriveController.canRedRotate = true; }

        yield return new WaitForSeconds(0.8f);
        isShooting = false;
    }

    private void RotateShooter()
    {
        Vector3 targetDir = target.position - shooterDirection.transform.position;
        float shooterAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        JointSpring shooterSpring = shooterPivot.spring;
        if (alliance == Alliance.Blue) { shooterAngle = -shooterAngle; }
        shooterSpring.targetPosition = shooterAngle - angleOffset;
        shooterPivot.spring = shooterSpring;
    }

    private void StowShooter()
    {
        JointSpring shooterSpring = shooterPivot.spring;
        shooterSpring.targetPosition = 0;
        shooterPivot.spring = shooterSpring;
    }

    public void OnShoot(InputAction.CallbackContext ctx) 
    {
        alignWholeRobot = ctx.ReadValue<float>();
    }

    public void OnAmp(InputAction.CallbackContext ctx)
    {
        amp = ctx.action.triggered;
    }

    public void Reset() 
    {
        StopAllCoroutines();
        DriveController.canBlueRotate = true;
        DriveController.canRedRotate = true;
        canDoAlign = true;    
        isAmping = false;
        isShooting = false;
    }
}