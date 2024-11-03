using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class DriveController : MonoBehaviour, IResettable
{
    public RobotSettings robotType;
    [SerializeField] private TMP_Text[] bumperNumbers;
    [SerializeField] private bool reverseBumperAllianceText = false;

    [SerializeField] private Transform[] rayCastPoints;
    [SerializeField] private float rayCastDistance;
    [SerializeField] private bool flipRayCastDir = false;

    [SerializeField] private bool flipStartingReverse;

    [SerializeField] private Collider field;

    //Handles climbing logic
    public bool isGrounded = true;
    public bool isTouchingGround = true;
    public bool isClimbed = false;

    public AudioSource redCountDown;
    public AudioSource blueCountDown;
    public AudioSource robotPlayer;
    public AudioSource treadPlayer;
    public AudioSource gearPlayer;
    public AudioResource intakeSound;
    public AudioResource swerveSound;
    public AudioResource gearSound;
    public float moveSpeed = 20f;
    public float rotationSpeed = 15f;
    public bool isRedRobot = false;
    public bool areRobotsTouching;
    public bool startingReversed = false;

    public bool is930;

    public static bool canBlueRotate;
    public static bool canRedRotate;
    public static bool isTouchingWallColliderRed = false;
    public static bool isTouchingWallColliderBlue = false;
    public Vector3 velocity { get; set; }
    public bool canIntake { get; set; }
    public static bool robotsTouching;
    public static bool isPinningRed = false;
    public static bool isPinningBlue = false;
    public static bool isAmped = false;
    public static bool isRedAmped = false;
    public bool isIntaking;

    private Rigidbody rb;
    private Vector2 translateValue;
    private float rotateValue;
    private Vector3 startingDirection;
    private Vector3 startingRotation;
    public float intakeValue = 0f;
    private bool ampSpeaker = false;

    private bool dontPlayDriveSounds = false;
    private bool useSwerveSounds;
    private bool useIntakeSounds;

    public Material materialPrefab;
    [SerializeField] private GameObject bumper;
    private Material bumperMat;
    private Color defaultBumperColor;

    [SerializeField] private Amp allianceAmp;
    [SerializeField] private Speaker allianceSpeaker;

    public float beforeVelocity;
    private bool dontUpdateBeforeVelocity = false;

    private Vector3 centerOfMass;

    [SerializeField] private float maxAngularVelocity = 5f;
    public bool isFieldCentric = false;

    private Coroutine ampTimerCoroutine;

    private GameManager gameManager;

    private Vector3 startingPos;
    private Quaternion startingRot;

    public bool atTargetPos = false;
    public bool atTargetRot = false;

    private void Start()
    {
        canIntake = true;

        startingPos = transform.position;
        startingRot = transform.rotation;

        if (materialPrefab != null)
        {
            bumperMat = Instantiate(materialPrefab);

            if (is930) 
            {
                Material[] mat = bumper.GetComponent<Renderer>().materials;
                mat[3] = bumperMat;
                mat[4] = bumperMat;
            }
            else 
            {
                bumper.GetComponent<Renderer>().material = bumperMat;
            }

            defaultBumperColor = bumperMat.color;
        }
        else { Debug.LogError("Material prefab is not assigned!"); }

        if (!reverseBumperAllianceText) 
        {
            if (isRedRobot && PlayerPrefs.GetString("redName") != "")
            {
                foreach (TMP_Text bumperNumber in bumperNumbers) 
                {
                    bumperNumber.text = PlayerPrefs.GetString("redName");
                }
            }
            else if (!isRedRobot && PlayerPrefs.GetString("blueName") != "") 
            {
                foreach (TMP_Text bumperNumber in bumperNumbers) 
                {
                    bumperNumber.text = PlayerPrefs.GetString("blueName");
                }
            }
        }
        else 
        {
            if (isRedRobot && PlayerPrefs.GetString("blueName") != "")
            {
                foreach (TMP_Text bumperNumber in bumperNumbers) 
                {
                    bumperNumber.text = PlayerPrefs.GetString("blueName");
                }
            }
            else if (!isRedRobot && PlayerPrefs.GetString("redName") != "") 
            {
                foreach (TMP_Text bumperNumber in bumperNumbers) 
                {
                    bumperNumber.text = PlayerPrefs.GetString("redName");
                }
            }
        }
        

        useSwerveSounds = PlayerPrefs.GetInt("swerveSounds") == 1;
        useIntakeSounds = PlayerPrefs.GetInt("intakeSounds") == 1;

        treadPlayer.resource = swerveSound;
        treadPlayer.loop = true;

        gearPlayer.resource = gearSound;
        gearPlayer.loop = true;

        moveSpeed = moveSpeed - (moveSpeed * (PlayerPrefs.GetFloat("movespeed") / 100f));
        rotationSpeed = rotationSpeed - (rotationSpeed * (PlayerPrefs.GetFloat("rotatespeed") / 100f));

        //Resetting static variables on start
        canBlueRotate = true;
        canRedRotate = true;

        isTouchingWallColliderRed = false;
        isTouchingWallColliderBlue = false;

        isPinningRed = false;
        isPinningBlue = false;
        robotsTouching = false;
        velocity = new Vector3(0f, 0f, 0f);
        isAmped = false;
        isRedAmped = false;
        isIntaking = false;

        //Initializing starting transforms
        rb = GetComponent<Rigidbody>();

        if (flipStartingReverse)
        {
            startingReversed = !startingReversed;
        }

        if (!startingReversed)
        {
            startingDirection = gameObject.transform.forward;
            startingRotation = gameObject.transform.right;
        }
        else
        {
            startingDirection = -gameObject.transform.forward;
            startingRotation = -gameObject.transform.right;
        }
        gameManager = GameObject.Find("GameGUI").GetComponent<GameManager>();
    }

    private void Update()
    {
        isGrounded = CheckGround();
        rb.centerOfMass = centerOfMass;
        areRobotsTouching = robotsTouching;

        if (GameManager.GameState == GameState.Endgame || GameManager.endBuzzerPlaying)
        {
            if (robotType == RobotSettings.HighTide || robotType == RobotSettings.CitrusCircuits || robotType == RobotSettings.Killshon) 
            {
                isTouchingGround = CheckTouchingGround();
            }
        }

        if (!dontUpdateBeforeVelocity) 
        {
            if (!isTouchingWallColliderBlue && !isRedRobot || !isTouchingWallColliderRed && isRedRobot) 
            {
                beforeVelocity = rb.velocity.magnitude;
            }
        }

        if (!isRedRobot) 
        {
            if (robotsTouching && isTouchingWallColliderBlue)
            {
                isPinningBlue = true;
            }
            else 
            {
                isPinningBlue = false;
            }
        }
        else 
        {
            if (robotsTouching && isTouchingWallColliderRed)
            {
                isPinningRed = true;
            }
            else 
            {
                isPinningRed = false;
            }
        }
        

        if (!isRedRobot) 
        {
            if (ampSpeaker && allianceAmp.numOfStoredNotes >= 2)
            {
                if (GameManager.GameState != GameState.Auto) 
                {
                    blueCountDown.Play();
                    isAmped = true;
                    AmplifySpeaker();
                    allianceAmp.ResetStoredNotes();
                }
            }
        }
        else 
        {
            if (ampSpeaker && allianceAmp.numOfStoredNotes >= 2) 
            {
                if (GameManager.GameState != GameState.Auto)
                {
                    redCountDown.Play();
                    isRedAmped = true;
                    AmplifySpeaker();
                    allianceAmp.ResetStoredNotes();
                }
            }
        }

        if (intakeValue > 0f && GameManager.canRobotMove && canIntake)
        {
            robotPlayer.resource = intakeSound;
            isIntaking = true;
        }
        else 
        {
            isIntaking = false;
        }
        
        if (useIntakeSounds) 
        {
            if (isIntaking && !robotPlayer.isPlaying) 
            {
                robotPlayer.Play();
            }
            else if (!isIntaking && robotPlayer.isPlaying)
            {
                robotPlayer.Stop();
            }
        }

        if (useSwerveSounds)
        {
            bool isMovingOrRotating = Math.Abs(Math.Round(velocity.x)) > 0f || Math.Abs(Math.Round(velocity.z)) > 0f || Math.Abs(rotateValue) > 0f;

            if (isMovingOrRotating && !dontPlayDriveSounds)
            {
                PlaySwerveSounds();
            }
            else
            {
                StopSwerveSounds();
            }
        }
    }

    void FixedUpdate()
    {
        if (GameManager.canRobotMove)
        {
            if (isGrounded) 
            {
                dontPlayDriveSounds = false;

                Vector3 moveDirection;
                
                if (isFieldCentric) 
                {
                    moveDirection = startingDirection * translateValue.y + startingRotation * translateValue.x;
                }
                else
                {
                    moveDirection = transform.forward * translateValue.y + transform.right * translateValue.x;
                }

                Vector3 rotation = new Vector3(0f, rotateValue * rotationSpeed, 0f);

                rb.AddForce(moveDirection * moveSpeed);

                if (isRedRobot && canRedRotate || !isRedRobot && canBlueRotate)
                {
                    rb.AddTorque(rotation);
                    rb.angularVelocity = Vector3.ClampMagnitude(rb.angularVelocity, maxAngularVelocity);
                }

                velocity = rb.velocity;
            }
            else 
            {
                dontPlayDriveSounds = true;
            }
        }
        else 
        {
            if (useSwerveSounds) 
            {
                dontPlayDriveSounds = true;
                StopSwerveSounds();
            }
        }
    }

    public void StopCountdown()
    {
        redCountDown.Stop();
        blueCountDown.Stop();
    }

    private void PlaySwerveSounds()
    {
        float velocityFactor = Mathf.Clamp01(velocity.magnitude / moveSpeed);
        float accelerationFactor = Mathf.Clamp(1f + (velocity.magnitude / moveSpeed), 1f, 2f);
        
        float rotationFactor = Mathf.Clamp01(Mathf.Abs(rotateValue) / rotationSpeed);

        float volume = velocityFactor + (rotationFactor * 10f);
        
        float pitch = Mathf.Max(accelerationFactor, rotationFactor);

        treadPlayer.volume = volume * 0.8f;
        treadPlayer.pitch = pitch * 0.7f;
        gearPlayer.volume = volume * 0.5f;

        if (!treadPlayer.isPlaying && !gearPlayer.isPlaying)
        {
            gearPlayer.Play();
            treadPlayer.Play();
        }
    }

    private void StopSwerveSounds()
    {
        if (treadPlayer.isPlaying || gearPlayer.isPlaying)
        {
            treadPlayer.Stop();
            gearPlayer.Stop();
        }
    }

    private void AmplifySpeaker() 
    {
        ampTimerCoroutine = StartCoroutine(StartTimer());
    }

    public void StopAmplifiedSpeaker() 
    {
        //Stop the countdown coroutine
        if (ampTimerCoroutine != null) 
        {
            StopCoroutine(ampTimerCoroutine);
        }

        //Reset note worth after amplification ends
        allianceSpeaker.ResetNotes();

        if (isRedRobot) { redCountDown.Stop(); }
        else { blueCountDown.Stop(); }
        
        //Reset isAmped flag to false
        if (!isRedRobot) { isAmped = false; }
        else { isRedAmped = false; }    
    }

    public IEnumerator GrayOutBumpers(float duration) 
    {
        if (is930) 
        {
            Material[] mat = bumper.GetComponent<Renderer>().materials;
            mat[3].color = Color.gray;
            mat[4].color = Color.gray;
        }
        else { bumperMat.color = Color.gray; }
        yield return new WaitForSeconds(duration);
        if (is930) 
        {
            Material[] mat = bumper.GetComponent<Renderer>().materials;
            mat[3].color = defaultBumperColor;
            mat[4].color = defaultBumperColor;
        }
        else { bumperMat.color = defaultBumperColor; }
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(10f);
        StopAmplifiedSpeaker();        
    }

    public void Reset()
    {
        StopAllCoroutines();

        //Reset bumper colors
        if (is930) 
        {
            Material[] mat = bumper.GetComponent<Renderer>().materials;
            mat[3].color = defaultBumperColor;
            mat[4].color = defaultBumperColor;
        }
        else { bumperMat.color = defaultBumperColor; }
        
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //Reset position
        rb.MovePosition(startingPos);
        rb.MoveRotation(startingRot);
    }

    public void OnTranslate(InputAction.CallbackContext ctx)
    {
        translateValue = ctx.ReadValue<Vector2>();
    }

    public void OnRotate(InputAction.CallbackContext ctx) 
    {
        rotateValue = ctx.ReadValue<float>();
    }

    public void OnIntake(InputAction.CallbackContext ctx) 
    {
        intakeValue = ctx.ReadValue<float>();
    }

    public void OnAmpSpeaker(InputAction.CallbackContext ctx) 
    {
        ampSpeaker = ctx.action.triggered;
    }

    public void OnRestart(InputAction.CallbackContext ctx)
    {
        gameManager.ResetMatch();
    }

    public bool CheckGround()
    {
        float distanceToTheGround = rayCastDistance;
        foreach (Transform rayCastPoint in rayCastPoints) 
        {
            if (!flipRayCastDir) 
            {
                if (Physics.Raycast(rayCastPoint.position, -transform.up, distanceToTheGround))
                {
                    return true;
                }
            }
            else 
            {
                if (Physics.Raycast(rayCastPoint.position, transform.up, distanceToTheGround))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool CheckTouchingGround()
    {
        if (field.bounds.Intersects(gameObject.GetComponent<Collider>().bounds)) 
        {
            return true;
        }
        return false;
    }

    public IEnumerator DriveTo(Transform target)
    { 
        atTargetPos = false;
        while (target != null && Vector2.Distance(transform.position, target.position) > 0.1f)
        {
            if (!GameManager.canRobotMove)
            {
                atTargetPos = true;
                break;
            }

            Vector3 targetDirection = target.position - transform.position;
            targetDirection.y = 0f;

            targetDirection.Normalize();

            Vector3 force = targetDirection * moveSpeed * Time.deltaTime;
        
            rb.AddForce(force, ForceMode.VelocityChange);

            velocity = rb.velocity;

            PlaySwerveSounds();
            yield return null;
        }
        atTargetPos = true;
    }

    public IEnumerator RotateTowardsTarget(Transform target)
    {
        atTargetRot = false;
        while (target != null)
        {
            if (!GameManager.canRobotMove || isRedRobot && !canRedRotate || !isRedRobot && !canBlueRotate)
            {
                atTargetRot = true;
                break;
            }

            Vector3 targetDirection = target.position - transform.position;
            targetDirection.y = 0f;

            if (targetDirection == new Vector3(0, 0, 0)) 
            {
                atTargetRot = true;
                break;
            }

            Quaternion targetRotation = Quaternion.LookRotation(-targetDirection, Vector3.up);

            Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            rb.MoveRotation(newRotation);

            if (Quaternion.Angle(transform.rotation, targetRotation) == 0)
            {
                break;
            }

            yield return null;
        }        
        atTargetRot = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isRedRobot) 
        {
            if (other.gameObject.CompareTag("RedPlayer"))
            {
                robotsTouching = true;
            }
            else if (other.gameObject.CompareTag("Field") || other.gameObject.CompareTag("Wall")) 
            {
                dontUpdateBeforeVelocity = true;
                isTouchingWallColliderBlue = true;
            }
        }
        else
        {
            if (other.gameObject.CompareTag("Player"))
            {
                robotsTouching = true;
            }
            else if (other.gameObject.CompareTag("Field") || other.gameObject.CompareTag("Wall"))
            {
                dontUpdateBeforeVelocity = true;
                isTouchingWallColliderRed = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isRedRobot) 
        {
            if (other.gameObject.CompareTag("RedPlayer"))
            {
                robotsTouching = false;
            }
            else if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Field")) 
            {
                dontUpdateBeforeVelocity = false;
                if (!isRedRobot) 
                {
                    isTouchingWallColliderBlue = false;
                }
                else 
                {
                    isTouchingWallColliderRed = false;
                }
            }
        }
        else
        {
            if (other.gameObject.CompareTag("Player"))
            {
                robotsTouching = false;
            }
            else if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Field")) 
            {
                dontUpdateBeforeVelocity = false;
                if (!isRedRobot) 
                {
                    isTouchingWallColliderBlue = false;
                }
                else 
                {
                    isTouchingWallColliderRed = false;
                }
            }
        }
    }
}
