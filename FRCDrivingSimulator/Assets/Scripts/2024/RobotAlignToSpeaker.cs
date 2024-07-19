using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class RobotAlignToSpeaker : MonoBehaviour, IResettable
{
    [SerializeField] private Alliance alliance;
    [SerializeField] private RobotSettings robot;

    public Transform target;
    public GameObject shooterPivot;
    public Rigidbody robotRigidbody;

    public float shooterSpeed = 600f;
    public float rotationSpeed = 50f;
    public float ampRotationSpeed = 100f;
    public float ampDuration = 0f;

    public float downwardOffset;

    public float maxAimDistance = 40f;

    private Quaternion pivotLocalStartingRot;

    public bool stowedShooter = false;

    private float alignWholeRobot;

    [SerializeField] private bool useLimits;
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;

    [SerializeField] private float hardMinAngle;
    [SerializeField] private float hardMaxAngle;

    private Quaternion targetRotation;

    private bool canDoAlign = true;
    private bool amp = false;
    private float pass = 0f;
    private bool isAmping = false;
    public bool isShooting = false;
    private bool isPassing = false;
    public bool isStowing = false;
    private bool isTrapping = false;

    [SerializeField] private bool robotThatRotatesUpForAmping;
    [SerializeField] private bool robotHasUniquePassAngle;
    [SerializeField] private bool dontAllowShootingWhenStowed = true;
    [SerializeField] private bool is1678;

    public RobotNoteManager ringCollisions;

    [SerializeField] private float passingAngle;

    private DriveController drive;

    public bool within4414SpeakerDistance;

    private Coroutine stowingCoroutine = null;

    private void Start()
    {
        ringCollisions = gameObject.GetComponent<RobotNoteManager>();
        drive = gameObject.GetComponent<DriveController>();
        pivotLocalStartingRot = shooterPivot.transform.localRotation;
    }

    private void Update()
    {
        if (GameManager.canRobotMove)
        {
            if (!isTrapping) 
            {
                float distanceToTarget = Vector3.Distance(shooterPivot.transform.position, target.position);

                if (isStowing || isAmping)
                {
                    ringCollisions.canShoot = false;
                }
                else if (stowedShooter && distanceToTarget > maxAimDistance && dontAllowShootingWhenStowed) 
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

                    if (30 <= distanceToTarget && distanceToTarget <= maxAimDistance && robot == RobotSettings.HighTide) 
                    {
                        within4414SpeakerDistance = true;
                        ringCollisions.noteDrag = (maxAimDistance - distanceToTarget) * 0.02f;
                        ringCollisions.speed = distanceToTarget * 3.5f;
                    }
                    else if (distanceToTarget < 18 && robot == RobotSettings.HighTide) 
                    {
                        within4414SpeakerDistance = false;
                        ringCollisions.noteDrag = 4f;
                    }
                    else if (robot == RobotSettings.HighTide)
                    {
                        within4414SpeakerDistance = true;
                        ringCollisions.noteDrag = 1.2f;
                    }
                }
                else 
                {
                    within4414SpeakerDistance = false;
                    ringCollisions.noteDrag = 1.2f;
                    ringCollisions.speed = ringCollisions.passingSpeed;
                }

                if (!isAmping && !isPassing)
                {
                    if (!amp && !(pass > 0f))
                    {
                        if (distanceToTarget <= maxAimDistance && !stowedShooter)
                        {
                            //To account for weird missing when 1678 is close to subwoofer
                            if (distanceToTarget <= 26f && is1678) { downwardOffset = 17.5f; }
                            else if (is1678) { downwardOffset = 16.6f; }

                            RotateShooter();
                            stowedShooter = false;
                        }
                        else if (distanceToTarget > maxAimDistance && robotHasUniquePassAngle && !(pass > 0f) && !stowedShooter)
                        {
                            StowShooter();
                        }
                    }
                    else if (distanceToTarget > maxAimDistance && robotHasUniquePassAngle && pass > 0f)
                    {
                        stowedShooter = false;
                        if (isStowing && stowingCoroutine != null)
                        {
                            StopCoroutine(stowingCoroutine);
                        }
                        StartCoroutine(Pass());
                    }
                    else if (amp && ringCollisions.hasRingInRobot && !isAmping && !ringCollisions.isShooting && robotThatRotatesUpForAmping)
                    {
                        StartCoroutine(AmplifyRotation());
                    }

                    if (distanceToTarget <= maxAimDistance && stowedShooter) 
                    {
                        StartCoroutine(UnstowShooter());
                    }
                    else if (distanceToTarget <= maxAimDistance && !stowedShooter)
                    {
                        RotateShooter();
                    }

                    if (alignWholeRobot > 0f && canDoAlign && distanceToTarget <= maxAimDistance)
                    {
                        canDoAlign = false;
                        RotateRobotToTarget();
                    }
                    else if (alignWholeRobot == 0f)
                    {
                        canDoAlign = true;
                    }
                }
            }
            else 
            {
                ringCollisions.canShoot = false;
            }
        }
    }

    private IEnumerator Pass()
    {        
        isPassing = true;
        Quaternion startRotation = shooterPivot.transform.localRotation;

        Quaternion targetRotation = Quaternion.Euler(passingAngle, 0f, 0f);

        float elapsedTime = 0f;
        float duration = 0.05f;

        while (elapsedTime < duration)
        {
            shooterPivot.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        shooterPivot.transform.localRotation = targetRotation;

        yield return new WaitForSeconds(1f);

        isPassing = false;
    }

    private IEnumerator AmplifyRotation()
    {
        isAmping = true;

        Quaternion startRotation = shooterPivot.transform.localRotation;

        Quaternion targetRotation = Quaternion.Euler(-100f, 0f, 0f);

        float elapsedTime = 0f;
        float duration = ampDuration;

        while (elapsedTime < duration)
        {
            shooterPivot.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        shooterPivot.transform.localRotation = targetRotation;
        yield return new WaitForSeconds(0.2f);

        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            shooterPivot.transform.localRotation = Quaternion.Slerp(targetRotation, startRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        shooterPivot.transform.localRotation = startRotation;

        amp = false;
        isAmping = false;
    }

    public void RotateRobotToTarget()
    {
        if (!isShooting && drive.isGrounded)
        {
            isShooting = true;

            Vector3 directionToTarget = target.position - transform.position;
            directionToTarget.y = 0f;

            targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

            StartCoroutine(RotateTowardsTarget(targetRotation));
        }
    }

    private IEnumerator RotateTowardsTarget(Quaternion targetRotation)
    {
        if (alliance == Alliance.Blue) { DriveController.canBlueRotate = false; }
        else { DriveController.canRedRotate = false; }

        while (Quaternion.Angle(robotRigidbody.rotation, targetRotation) > 0.1f)
        {
            robotRigidbody.rotation = Quaternion.RotateTowards(robotRigidbody.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        if (alliance == Alliance.Blue) { DriveController.canBlueRotate = true; }
        else { DriveController.canRedRotate = true; }

        isShooting = false;
    }

    private void RotateShooter()
    {
        Vector3 targetPositionWithOffset = target.position - Vector3.up * downwardOffset;

        float shooterRotationAmount = shooterSpeed * Time.deltaTime;

        Quaternion targetShooterRotation = Quaternion.LookRotation(targetPositionWithOffset - shooterPivot.transform.position, Vector3.up);

        Vector3 euler = targetShooterRotation.eulerAngles;
        if (useLimits) { euler.x = Mathf.Clamp(euler.x, minAngle, maxAngle); }
        euler.y = shooterPivot.transform.rotation.eulerAngles.y;
        euler.z = shooterPivot.transform.rotation.eulerAngles.z;
        targetShooterRotation = Quaternion.Euler(euler);

        if (shooterPivot.transform.localEulerAngles.x >= hardMinAngle && shooterPivot.transform.localEulerAngles.x <= hardMaxAngle) 
        {
            if (transform.localEulerAngles.x < 40 && transform.localEulerAngles.x > -40 && transform.localEulerAngles.z < 40 && transform.localEulerAngles.z > -40) 
            {
                shooterPivot.transform.rotation = Quaternion.RotateTowards(shooterPivot.transform.rotation, targetShooterRotation, shooterRotationAmount);
            }
        }
        else
        {
            Vector3 localEuler = shooterPivot.transform.localEulerAngles;
            localEuler.x = Mathf.Clamp(localEuler.x, minAngle, maxAngle);
            localEuler.y = 0f;
            localEuler.z = 0f;
            shooterPivot.transform.localEulerAngles = localEuler;
        }
    }

    private void StowShooter() 
    {
        stowingCoroutine = StartCoroutine(ShooterToPosition(pivotLocalStartingRot, true));
    }

    public IEnumerator ShooterToPosition(Quaternion pos, bool actionIsStowing)
    {
        if (actionIsStowing) { isStowing = true; }
        else { isTrapping = true; }

        float duration = 0.2f;
        float elapsedTime = 0f;

        Quaternion startPivotLocalRotation = shooterPivot.transform.localRotation;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            shooterPivot.transform.localRotation = Quaternion.Slerp(startPivotLocalRotation, pos, t); // Use local rotation
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (actionIsStowing) 
        {
            shooterPivot.transform.localRotation = pivotLocalStartingRot;
            isStowing = false;
            stowedShooter = true;
        }
        else 
        {
            shooterPivot.transform.localRotation = pos;
        }
    }

    private IEnumerator UnstowShooter()
    {
        stowedShooter = false;
        float duration = 0.2f;
        float elapsedTime = 0f;

        shooterSpeed = 0f;
        while (elapsedTime < duration)
        {
            shooterSpeed += 50;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        shooterSpeed = 600;
    }

    public void OnShoot(InputAction.CallbackContext ctx)
    {
        alignWholeRobot = ctx.ReadValue<float>();
    }

    public void OnPass(InputAction.CallbackContext ctx)
    {
        pass = ctx.ReadValue<float>();
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
        isPassing = false;
        isStowing = false;
        isTrapping = false;
        stowingCoroutine = null;
    }
}