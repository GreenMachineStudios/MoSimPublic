using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotatingAmpArm : MonoBehaviour, IResettable
{
    [SerializeField] private GameObject ampPivot;
    [SerializeField] private float ampDuration;
    [SerializeField] private float ampShotSpeed;
    [SerializeField] private Vector3 targetPos = Vector3.zero;

    Quaternion startRotation;

    private float shootingSpeed;
    public bool isAmping;
    private bool amp;

    private bool shotNote;

    private RobotNoteManager ringCollisions;

    private void Start() 
    {
        ringCollisions = GetComponent<RobotNoteManager>();
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
            AmpAction();
        }
        else 
        {
            isAmping = false;
            ringCollisions.isAmping = false;
        }
    }

    private void PrepareAmp() 
    {
        ringCollisions.isAmping = true;
    }

    public void AmpAction()
    {
        if (ringCollisions.isAmping) 
        {
            StartCoroutine(Amp());
            ringCollisions.isAmping = false;
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

        ampPivot.transform.localRotation = targetRotation;

        ringCollisions.shootingSpeed = ampShotSpeed;

        yield return new WaitForSeconds(0.2f);

        ringCollisions.AmpRing();

        yield return new WaitForSeconds(0.3f);

        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            ampPivot.transform.localRotation = Quaternion.Slerp(targetRotation, startRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ampPivot.transform.localRotation = startRotation;
        ringCollisions.shootingSpeed = shootingSpeed;

        isAmping = false;
        ringCollisions.isAmping = false;
    }

    public void Reset() 
    {
        StopAllCoroutines();
        ampPivot.transform.localRotation = startRotation;
        ringCollisions.shootingSpeed = shootingSpeed;
        isAmping = false;
    }

    public void OnAmp(InputAction.CallbackContext ctx)
    {
        amp = ctx.action.triggered;
    }
}
