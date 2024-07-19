using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotatingAmpArm : MonoBehaviour, IResettable
{
    [SerializeField] private Alliance alliance;
    [SerializeField] private GameObject ampPivot;
    [SerializeField] private Collider ampCollider;
    [SerializeField] private Transform ampTarget;
    [SerializeField] private float ampSpeed;
    [SerializeField] private float ampDuration;
    [SerializeField] private Vector3 targetPos = Vector3.zero;
    [SerializeField] private Vector3 ampPos = Vector3.zero;

    Quaternion startRotation;

    private float shootingSpeed;
    public bool isAmping;
    private bool amp;

    private RobotNoteManager ringCollisions;
    private TurretAlignToSpeaker turret;

    private void Start() 
    {
        ringCollisions = GetComponent<RobotNoteManager>();
        turret = GetComponent<TurretAlignToSpeaker>();
        shootingSpeed = ringCollisions.shootingSpeed;

        startRotation = ampPivot.transform.localRotation;
    }

    private void Update()
    {
        if (GameManager.canRobotMove) 
        {
            if (amp && !isAmping && !ringCollisions.isShooting)
            {
                if (ringCollisions.hasRingInRobot)
                {
                    PrepareAmp();
                }
            }
        }
        else 
        {
            isAmping = false;
            ringCollisions.isAmping = false;
            turret.isAmping = false;
        }
    }

    private void PrepareAmp() 
    {
        ringCollisions.isAmping = true;
        turret.turretSpeed = 300;
        turret.TurretAmp(0.5f);
    }

    public void AmpAction()
    {
        if (!isAmping) 
        {
            StartCoroutine(Amp());
        }
    }

    public IEnumerator Amp()
    {
        isAmping = true;

        Quaternion targetRotation = Quaternion.Euler(targetPos.x, targetPos.y, targetPos.z);

        float elapsedTime = 0f;
        float duration = ampDuration;

        while (elapsedTime < duration)
        {
            ampPivot.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Quaternion ampRotation = Quaternion.Euler(ampPos);
        turret.StartCoroutine(turret.RotateShooterToAmp(ampRotation, 0.2f, 0.5f));

        ampPivot.transform.localRotation = targetRotation;

        yield return new WaitForSeconds(0.4f);

        ringCollisions.shootingSpeed = 10;
        ringCollisions.ShootRing();
        GameObject note = ringCollisions.note;

        elapsedTime = 0f;
        while (elapsedTime < 0.3f) 
        {
            if (ampCollider.bounds.Intersects(note.GetComponent<Collider>().bounds))
            {
                Destroy(note);
                ringCollisions.StartCoroutine(ringCollisions.AmpSequence());
                break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            ampPivot.transform.localRotation = Quaternion.Slerp(targetRotation, startRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ampPivot.transform.localRotation = startRotation;
        ringCollisions.shootingSpeed = shootingSpeed;

        //Allow the turret to go back
        turret.isAmping = false;
        isAmping = false;
        ringCollisions.isAmping = false;

        //Unstow turret
        StartCoroutine(turret.UnstowTurret());
    }

    public void Reset() 
    {
        StopAllCoroutines();
        ampPivot.transform.localRotation = startRotation;
        ringCollisions.shootingSpeed = shootingSpeed;
        turret.isAmping = false;
        isAmping = false;
    }

    public void OnAmp(InputAction.CallbackContext ctx)
    {
        amp = ctx.action.triggered;
    }
}
