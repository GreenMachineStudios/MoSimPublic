using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CitrusClimb : MonoBehaviour, IResettable
{
    [SerializeField] private ConfigurableJoint climber;

    private JointBackpackAmpArm ampArm;
    private RobotAlignToSpeaker shooter;

    private int startingLayer;

    private bool climb;
    private bool raisedArm = false;
    private bool prepped = false;
    private bool isClimbing = false;

    private Vector3 climberStartingPos;
    private Quaternion climberStartingRot;

    private void Start() 
    {
        climberStartingPos = climber.gameObject.transform.localPosition;
        climberStartingRot = climber.gameObject.transform.localRotation;

        startingLayer = climber.gameObject.layer;

        ampArm = GetComponent<JointBackpackAmpArm>();
        shooter = GetComponent<RobotAlignToSpeaker>();
    }

    private void Update()
    {
        if (climb && !isClimbing)
        {
            isClimbing = true;
            StartCoroutine(ClimbSequence());
        }
        else if (climb && prepped && !raisedArm) 
        {
            StartCoroutine(AmpSequence());
        }
        else if (climb && prepped && raisedArm)
        {
            prepped = false;
            HangSequence();
        }
    }

    private IEnumerator AmpSequence() 
    {
        ampArm.TrapAmpArm();
        yield return new WaitForSeconds(0.5f);
        raisedArm = true;
    }

    private IEnumerator ClimbSequence() 
    {
        StartCoroutine(shooter.ShooterToPosition(Quaternion.Euler(35, 0, 0), false));
        climber.targetRotation = Quaternion.Euler(-100, 0, 0);
        yield return new WaitForSeconds(0.5f);
        prepped = true;
    }

    private void HangSequence() 
    {
        climber.targetRotation = Quaternion.Euler(0, 0, 0);
    }

    public void OnClimb(InputAction.CallbackContext ctx)
    {
        climb = ctx.action.triggered;
    }

    private IEnumerator WaitToEnable() 
    {
        yield return new WaitForSeconds(0.01f);
        climber.gameObject.layer = startingLayer;
    }

    public void Reset() 
    {
        climber.gameObject.layer = 17;

        prepped = false;
        raisedArm = false;
        isClimbing = false;

        //Reset joints pos and rot and targetPos
        climber.gameObject.transform.localPosition = climberStartingPos;
        climber.gameObject.transform.localRotation = climberStartingRot;

        climber.targetRotation = Quaternion.Euler(0, 0, 0);

        StartCoroutine(WaitToEnable());
    }
}
