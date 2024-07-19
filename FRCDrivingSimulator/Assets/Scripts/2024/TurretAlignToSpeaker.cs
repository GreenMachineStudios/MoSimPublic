using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurretAlignToSpeaker : MonoBehaviour, IResettable
{
    [SerializeField] private RobotSettings robot;
    [SerializeField] private Alliance alliance;
    [SerializeField] private TurretMode turretMode;

    [SerializeField] private float turretWrapDuration;

    public Transform target;
    
    public GameObject shooterPivot;
    public GameObject turret;

    public float shooterSpeed;
    public float turretSpeed;

    //Stores default speeds to revert to when needed
    private float defaultTurretSpeed;
    private float defaultShooterSpeed;

    [SerializeField] private Vector2 turretLimits;

    //Max distance away from the target that you can be to still track it
    public float maxAimDistance;

    //Does the robot use the above maxAimDistance?
    public bool continuousTracking;

    //Stores starting rotations for stowing
    private Quaternion turretLocalStartingRot;
    private Quaternion pivotLocalStartingRot;

    //Track state of turret/shooter
    [SerializeField] private bool stowedShooter = false;
    [SerializeField] private bool stowedTurret = false;

    //Offset that accounts for shooter not exactly aligning with shooterPivot
    public float downwardOffset;

    //Option to reverse the turret/shooter directions for different robot setups
    [SerializeField] private bool reverseTurret;
    [SerializeField] private bool reverseShooter;

    //State trackers
    private bool isPassing;
    public bool isAmping;
    public bool isClimbing;

    //Limits for shooter
    public float minAngle;
    public float maxAngle;

    //Input for passing
    private float pass = 0f;

    [SerializeField] private float passingPause;

    //Angle at which the shooter tilts to, to pass
    [SerializeField] private float passingAngle;

    private RobotNoteManager ringCollisions;
    private DriveController controller;
    private RotatingAmpArm ampArm;

    private bool stopCoroutines = true;

    private float turretRot = 0f;

    private bool rotate = true;
    private bool onlyDoAmpOnce = true;

    private void Start()
    {
        turretLocalStartingRot = turret.transform.localRotation;
        pivotLocalStartingRot = shooterPivot.transform.localRotation;
        defaultTurretSpeed = turretSpeed;
        defaultShooterSpeed = shooterSpeed;

        controller = GetComponent<DriveController>();
        ringCollisions = GetComponent<RobotNoteManager>();

        if (GetComponent<RotatingAmpArm>() != null) 
        {
            ampArm = GetComponent<RotatingAmpArm>();
        }
    }

    private void Update()
    {
        if (GameManager.canRobotMove)
        {
            //Reset flag
            stopCoroutines = true;

            float distanceToTarget = Vector3.Distance(shooterPivot.transform.position, target.position);

            if (!isAmping && !isClimbing)
            {
                if (distanceToTarget <= maxAimDistance && !stowedTurret)
                {
                    //Regular turret logic if within your wing
                    if (!controller.isIntaking || continuousTracking)
                    {
                        stowedShooter = false;
                        stowedTurret = false;
                        RotateShooter();
                        RotateTurret();
                    }
                    else if (!continuousTracking)
                    {
                        //Stow turret and shooter when intaking if not continusTracking
                        if (!stowedTurret) { StartCoroutine(StowTurret()); }
                        if (!stowedShooter) { StartCoroutine(StowShooter()); }
                    }
                }
                else if (distanceToTarget <= maxAimDistance && stowedTurret && !continuousTracking)
                {
                    //Unstow the turret when re-entering the maxDistance
                    StartCoroutine(UnstowTurret());
                }
                else if (distanceToTarget > maxAimDistance && !continuousTracking && !isPassing && !stowedShooter && !stowedTurret)
                {
                    //Stow the turret/shooter if not passing and out of maxDistance
                    if (!stowedTurret)
                    {
                        StartCoroutine(StowTurret());
                    }

                    if (!stowedShooter)
                    {
                        StartCoroutine(StowShooter());
                    }
                }
                else if (continuousTracking && !isPassing) 
                {
                    RotateTurret();
                    RotateShooter();
                }
                else if (continuousTracking) 
                {
                    RotateTurret();
                }

                //Pass if outside maxDistance and passing is trigged and passing isn't in progress
                if (distanceToTarget > maxAimDistance && pass > 0f && !isPassing)
                {
                    StartCoroutine(Pass());
                }
            }

            //Handle shooting speeds
            if (distanceToTarget <= maxAimDistance) 
            {
                ringCollisions.speed = ringCollisions.shootingSpeed;
            }
            else 
            {
                ringCollisions.speed = ringCollisions.passingSpeed;
            }
        }
        else
        {
            //Stop all processes when robot cant move
            if (stopCoroutines)
            {
                StopAllCoroutines();
                rotate = true;
                onlyDoAmpOnce = true;
                isPassing = false;
                isAmping = false;
                isClimbing = false;
                stopCoroutines = false;
            }
        }
    }

    private void RotateTurret()
    {
        Vector3 targetDirection = target.position - turret.transform.position;

        float turretAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;

        if (!reverseTurret)
        {
            turretAngle += 180f;
        }

        float turretRotationAmount = turretSpeed * Time.deltaTime;

        Quaternion currentRotation = turret.transform.rotation;
        Quaternion targetTurretRotation;

        //Special turret rotation settings per robot
        if (robot == RobotSettings.MukwonagoBears) 
        {
            targetTurretRotation = Quaternion.Euler(0f, turretAngle, 0f);
        }
        else
        {
            targetTurretRotation = Quaternion.Euler(0f, turretAngle, 0f);
        }

        if (turretMode == TurretMode.Wrapping)
        {
            bool withinLimits = turretRot >= turretLimits.x && turretRot <= turretLimits.y;

            if (withinLimits && rotate)
            {
                turret.transform.rotation = Quaternion.RotateTowards(currentRotation, targetTurretRotation, turretRotationAmount);
            }
            else if (!withinLimits && rotate)
            {
                int wrapDirection = (turretRot < turretLimits.x) ? 1 : -1;

                StartCoroutine(WrapAround(wrapDirection));
            }

            float angleDiff = Mathf.DeltaAngle(currentRotation.eulerAngles.y, turret.transform.rotation.eulerAngles.y);

            turretRot += angleDiff;
        }
        else 
        {
            turret.transform.rotation = Quaternion.RotateTowards(currentRotation, targetTurretRotation, turretRotationAmount);
        }
    }

    private IEnumerator WrapAround(int direction)
    {
        rotate = false;

        float elapsedTime = 0f;
        float targetAngle;

        if (direction == 1)
        {
            targetAngle = 360f;
            turretRot += 360f;
        }
        else
        {
            targetAngle = -360f;
            turretRot -= 360f;
        }

        float duration = turretWrapDuration;

        while (elapsedTime < duration)
        {
            float angularSpeed = targetAngle / duration;
            float rotationPerFrame = angularSpeed * Time.deltaTime;

            turret.transform.localEulerAngles += new Vector3(0f, rotationPerFrame, 0f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        rotate = true;
    }

    private void RotateShooter()
    {
        //Shooter
        Vector3 targetPositionWithOffset = target.position - Vector3.up * downwardOffset;

        float shooterRotationAmount = shooterSpeed * Time.deltaTime;

        Quaternion targetShooterRotation;

        if (reverseShooter)
        {
            targetShooterRotation = Quaternion.LookRotation(shooterPivot.transform.position - targetPositionWithOffset, Vector3.up);
        }
        else
        {
            targetShooterRotation = Quaternion.LookRotation(targetPositionWithOffset - shooterPivot.transform.position, Vector3.up);
        }

        Vector3 euler = targetShooterRotation.eulerAngles;

        euler.y = shooterPivot.transform.rotation.eulerAngles.y;
        euler.z = shooterPivot.transform.rotation.eulerAngles.z;
        euler.x = Mathf.Clamp(euler.x, minAngle, maxAngle);
        targetShooterRotation = Quaternion.Euler(euler);

        shooterPivot.transform.rotation = Quaternion.RotateTowards(shooterPivot.transform.rotation, targetShooterRotation, shooterRotationAmount);
    }

    private IEnumerator Pass()
    {        
        isPassing = true;
        Quaternion startRotation = shooterPivot.transform.localRotation;

        Quaternion targetRotation = Quaternion.Euler(passingAngle, 0f, 0f);

        float elapsedTime = 0f;
        float duration = 0.35f;

        while (elapsedTime < duration)
        {
            shooterPivot.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        shooterPivot.transform.localRotation = targetRotation;

        yield return new WaitForSeconds(passingPause);

        while (pass > 0f)
        {
            yield return null;
        }

        if (continuousTracking)
        {
            StartCoroutine(UnstowShooter());
        }
        else 
        {
            StartCoroutine(StowTurret());
            StartCoroutine(StowShooter());
        }
    }

    //Rotates shooter up to shoot into amp arm which puts note into amp
    public IEnumerator RotateShooterToAmp(Quaternion ampRotation, float duration, float delay)
    {
        float elapsedTime = 0f;
        Quaternion startRotation = shooterPivot.transform.localRotation;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            shooterPivot.transform.localRotation = Quaternion.Slerp(startRotation, ampRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        shooterPivot.transform.localRotation = ampRotation;
        yield return new WaitForSeconds(delay);
        StartCoroutine(UnstowShooter());
    }

    //Rotates turret to amp
    public IEnumerator RotateTurretToAmp(float duration)
    {
        isAmping = true;
        float elapsedTime = 0f;

        Quaternion startRotation = turret.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, 0f);

        float initialRotationAngle = Quaternion.Angle(startRotation, targetRotation);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            turret.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        turret.transform.rotation = targetRotation;

        float finalRotationAngle = Quaternion.Angle(turret.transform.rotation, targetRotation);

        float rotationDistance = initialRotationAngle - finalRotationAngle;

        if (alliance == Alliance.Blue) 
        {
            turretRot += rotationDistance;
        }
        else 
        {
            turretRot -= rotationDistance;
        }

        ampArm.AmpAction();
        onlyDoAmpOnce = true;
    }

    public void TurretAmp(float duration) 
    {
        if (onlyDoAmpOnce) 
        {
            StartCoroutine(RotateTurretToAmp(duration));
        }
        onlyDoAmpOnce = false;
    }

    public IEnumerator StowTurret()
    {
        float duration = 0.3f;
        float elapsedTime = 0f;

        Quaternion startTurretLocalRotation = turret.transform.localRotation;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            turret.transform.localRotation = Quaternion.Slerp(startTurretLocalRotation, turretLocalStartingRot, t); // Use local rotation
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        turret.transform.localRotation = turretLocalStartingRot;
        turretRot = 0f;
        stowedTurret = true;
        isPassing = false;
    }

    //Unstow the turret when the robot re-enters the allowed aimDistance
    public IEnumerator UnstowTurret()
    {
        stowedTurret = false;
        float duration = 0.8f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            turretSpeed = defaultTurretSpeed * 0.5f;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        turretSpeed = defaultTurretSpeed;
    }

    public IEnumerator StowShooter()
    {
        stowedShooter = true;
        float duration = 0.3f;
        float elapsedTime = 0f;

        Quaternion startShooterLocalRotation = shooterPivot.transform.localRotation.normalized;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            shooterPivot.transform.localRotation = Quaternion.Slerp(startShooterLocalRotation, pivotLocalStartingRot, t); // Use local rotation
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        shooterPivot.transform.localRotation = pivotLocalStartingRot;
        isPassing = false;
    }


    //Unstow shooter back to tracking the target
    private IEnumerator UnstowShooter()
    {
        stowedShooter = false;
        float duration = 0.5f;
        float elapsedTime = 0f;

        shooterSpeed = 0;
        while (elapsedTime < duration)
        {
            RotateShooter();
            shooterSpeed += 2f;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        shooterSpeed = defaultShooterSpeed;
        isPassing = false;
    }

    //Takes input from the new unity input system
    public void OnPass(InputAction.CallbackContext ctx)
    {
        pass = ctx.ReadValue<float>();
    }

    //Method for match resetting
    public void Reset() 
    {
        StopAllCoroutines();
        stowedShooter = false;
        stowedTurret = false;
        isAmping = false;
        isClimbing = false;
        isPassing = false;
        turret.transform.localRotation = turretLocalStartingRot;
        shooterPivot.transform.localRotation = pivotLocalStartingRot;
        turretRot = 0f;
        rotate = true;
        onlyDoAmpOnce = true;
        StartCoroutine(UnstowShooter());
        StartCoroutine(UnstowTurret());
        stopCoroutines = true;
    }

    private enum TurretMode 
    {
        Infinite,
        Wrapping
    }
}