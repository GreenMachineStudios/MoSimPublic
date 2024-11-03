using System.Collections;
using System.IO;
using PathCreation;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class RobotNoteManager : MonoBehaviour, IResettable
{
    [SerializeField] private Alliance alliance;
    [SerializeField] private RobotSettings robot;

    [SerializeField] private Transform noteAnimationTarget;
    [SerializeField] private PathCreator notePath;
    [SerializeField] Transform notePathEnd;
    [SerializeField] Transform notePathAnchor;

    public GameObject touchedRing { get; set; }

    public GameObject prefabToInstantiate;
    public Transform shootingSpawnPoint;
    public Transform ampSpawnPoint;
    public float speed = 10f;
    public float shootingSpeed = 10f;
    public float passingSpeed = 10f;
    public float ampSpeed = 10f;
    public float noteDrag = 0f;

    public AudioSource player;
    public AudioResource shootSound;
    public AudioResource spedUpSound;

    public float shootingLatency = 0f;
    public float intakeLatency = 0f;
    public float ampingLatency = 0f;
    
    public GameObject hiddenNote;
    public bool hasRingInRobot;

    public bool ringWithinIntakeCollider { get; set; }

    public bool isShooting = false;
    public bool isAmping = false;

    private float shootValue = 0f;
    private bool ampValue = false;

    private ZoneControl zone;

    public bool canShoot = true;

    private DriveController controller;
    private LedStripController ledController;

    public GameObject note;

    public bool isOtherRobot = false;

    private void Start()
    {
        zone = FindFirstObjectByType<ZoneControl>();
        controller = GetComponent<DriveController>();
        ledController = GetComponent<LedStripController>();
        Reset();
    }

    private void Update()
    {
        if (GameManager.canRobotMove) 
        {
            if (notePath != null) 
            {
                if (notePathEnd != null) 
                {
                    notePath.bezierPath.MovePoint(3, notePath.transform.InverseTransformPoint(notePathEnd.position));
                }
                
                if (notePathAnchor != null) 
                {
                    notePath.bezierPath.MovePoint(2, notePath.transform.InverseTransformPoint(notePathAnchor.position));
                }
            }

            if (!isShooting && !isAmping) 
            {
                if (shootValue > 0 && hasRingInRobot && !isAmping)
                {
                    if (!isShooting && canShoot) 
                    {
                        isShooting = true;
                        StartCoroutine(ShootSequence());
                    }
                }
                else if (ampValue && hasRingInRobot)
                {
                    if (!isAmping && !isShooting) 
                    {
                        StartCoroutine(AmpSequence());
                    }
                }

                if (ringWithinIntakeCollider && !hasRingInRobot && controller.isIntaking && !isAmping) 
                {
                    hasRingInRobot = true;
                    IntakeSequence();
                    ledController.Flash();
                }
            }
        }
    }

    private void IntakeSequence() 
    {
        if (robot == RobotSettings.Robotnauts) 
        {
            //Animation of note going into indexer
            StartCoroutine(AnimateNoteToRobotCenter());
        }
        else if (robot == RobotSettings.CitrusCircuits || robot == RobotSettings.MukwonagoBears || robot == RobotSettings.HighTide) 
        {
            StartCoroutine(NoteSplineAnimation());
        }
        else 
        {
            Destroy(touchedRing);
            hiddenNote.SetActive(true);
        }

        ringWithinIntakeCollider = false;
        StartCoroutine(CanNotShootWhenRunning());
    }

    private IEnumerator NoteSplineAnimation() 
    {
        GameObject note = touchedRing;

        note.tag = "ignore";
        Transform child = note.transform.GetChild(0);
        child.gameObject.SetActive(false);
        note.layer = 12;
        Destroy(note.GetComponent<BoxCollider>());

        note.transform.SetParent(hiddenNote.transform.parent);
        float distanceTraveled = 0f;
        float noteSpeed = 1.5f;

        while (distanceTraveled < notePath.path.length)
        {
            distanceTraveled += noteSpeed * Time.deltaTime;
            note.transform.position = notePath.path.GetPointAtDistance(distanceTraveled, EndOfPathInstruction.Stop);
            note.transform.rotation = notePath.path.GetRotationAtDistance(distanceTraveled, EndOfPathInstruction.Stop);

            float t = distanceTraveled / notePath.path.length;
            note.transform.localScale = Vector3.Lerp(note.transform.localScale, hiddenNote.transform.localScale, t);

            if (!hasRingInRobot) 
            {
                break;
            }

            yield return null;
        }

        Destroy(note);

        if (hasRingInRobot) 
        {
            hiddenNote.SetActive(true);
        }
    }

    private IEnumerator AnimateNoteToRobotCenter() 
    {
        GameObject note = touchedRing;
        note.tag = "ignore";
        Transform child = note.transform.GetChild(0);
        child.gameObject.SetActive(false);
        note.layer = 12;
        Destroy(note.GetComponent<BoxCollider>());

        note.transform.SetParent(transform);

        Vector3 initialPosition = note.transform.position;
        Quaternion initialRotation = note.transform.rotation;
        float elapsedTime = 0f;
        float animationTime = 0.2f;

        while (elapsedTime < animationTime)
        {
            float t = elapsedTime / animationTime;

            note.transform.position = Vector3.Lerp(initialPosition, noteAnimationTarget.position, t);

            if (robot != RobotSettings.Robotnauts) 
            {
                note.transform.rotation = Quaternion.Slerp(initialRotation, noteAnimationTarget.rotation, t);
            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        note.transform.position = transform.position;

        Destroy(note);
        hiddenNote.SetActive(true);
    }

    public void ShootRing()
    {
        hiddenNote.SetActive(false);

        note = Instantiate(prefabToInstantiate, shootingSpawnPoint.position, shootingSpawnPoint.rotation);
        Rigidbody rb = note.GetComponent<Rigidbody>();
            
        float speedVariation = controller.velocity.magnitude * 0.2f;
        float angleVariation = controller.velocity.magnitude * 0.2f;

        Vector3 shootingDirection = Quaternion.Euler(0, angleVariation, 0) * shootingSpawnPoint.forward;
        Vector3 finalVelocity = (speed + speedVariation) * shootingDirection;

        rb.drag = noteDrag;
        rb.velocity = finalVelocity;

        hasRingInRobot = false;

        if (alliance == Alliance.Blue) 
        {
            zone.CheckBlueZoneCollisions();
            if (isOtherRobot) { note.tag = "noteShotByBlue2"; }
            else { note.tag = "noteShotByBlue"; }
        }
        else 
        {
            zone.CheckRedZoneCollisions();
            if (isOtherRobot) { note.tag = "noteShotByRed2"; }
            else { note.tag = "noteShotByRed"; }
        }

        note.transform.localScale = new Vector3(0.48f, 0.55f, 0.6f);
        note.GetComponent<RingBehaviour>().StartCoroutine(note.GetComponent<RingBehaviour>().UnSquishhhh());
    }

    public void AmpRing()
    {
        hasRingInRobot = false;

        hiddenNote.SetActive(false);

        note = Instantiate(prefabToInstantiate, ampSpawnPoint.position, ampSpawnPoint.rotation);
        Rigidbody rb = note.GetComponent<Rigidbody>();

        Vector3 parentVelocity = GetComponent<Rigidbody>().velocity;
        
        rb.drag = noteDrag;
        rb.velocity = parentVelocity + (ampSpawnPoint.forward.normalized * ampSpeed);
    }

    public IEnumerator ShootSequence()
    {
        player.resource = spedUpSound;
        player.Play();

        yield return new WaitForSeconds(shootingLatency);
        
        ShootRing();
        isShooting = false;
    }

    public IEnumerator AmpSequence()
    {
        isAmping = true;
        yield return new WaitForSeconds(ampingLatency);
        player.resource = shootSound;
        player.Play();
        AmpRing();
        isAmping = false;
    }   

    private IEnumerator CanNotShootWhenRunning() 
    {
        canShoot = false;
        yield return new WaitForSeconds(intakeLatency);
        canShoot = true;
    }

    public void Reset() 
    {
        player.Stop();
        StopAllCoroutines();
        isShooting = false;
        isAmping = false;
        canShoot = true;
        shootValue = 0f;
        ampValue = false;
        ringWithinIntakeCollider = false;

        hiddenNote.SetActive(true);
        hasRingInRobot = true;
    }

    public void OnShoot(InputAction.CallbackContext ctx)
    {
        shootValue = ctx.ReadValue<float>();
    }

    public void OnAmp(InputAction.CallbackContext ctx) 
    {
        ampValue = ctx.action.triggered;
    }
}
