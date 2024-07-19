using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HighTideClimb : MonoBehaviour, IResettable
{
    [SerializeField] private ConfigurableJoint hanger;
    [SerializeField] private HingeJoint forks;

    private JointBackpackAmpArm ampArm;

    private int startingLayer;

    private bool climb;
    private bool hang;
    private bool prepped;
    private bool isClimbing;

    private Vector3 hangerStartingPos;
    private Quaternion hangerStartingRot;
    private Vector3 forksStartingPos;
    private Quaternion forksStartingRot;
    
    private void Start() 
    {
        hangerStartingPos = hanger.gameObject.transform.localPosition;
        hangerStartingRot = hanger.gameObject.transform.localRotation;

        forksStartingPos = forks.gameObject.transform.localPosition;
        forksStartingRot = forks.gameObject.transform.localRotation;

        startingLayer = hanger.gameObject.layer;

        ampArm = GetComponent<JointBackpackAmpArm>();
    }

    private void Update()
    {
        if (climb && !isClimbing)
        {
            isClimbing = true;
            StartCoroutine(ClimbSequence());
        }
        else if (hang && prepped)
        {
            prepped = false;
            HangSequence();
            ampArm.TrapAmpArm();
        }
    }

    private IEnumerator ClimbSequence() 
    {
        hanger.targetPosition = new Vector3(0f, -3.35f, 0f);
        JointSpring forksSpring = forks.spring;
        forksSpring.targetPosition = -60f;
        forks.spring = forksSpring;
        yield return new WaitForSeconds(0.5f);
        prepped = true;
    }

    private void HangSequence() 
    {
        hanger.targetPosition = new Vector3(0f, 0f, 0f);
    }

    public void OnClimb(InputAction.CallbackContext ctx)
    {
        climb = ctx.action.triggered;
    }

    public void OnHang(InputAction.CallbackContext ctx)
    {
        hang = ctx.action.triggered;
    }

    private IEnumerator WaitToEnable() 
    {
        yield return new WaitForSeconds(0.01f);
        hanger.gameObject.layer = startingLayer;
        forks.gameObject.layer = startingLayer;
    }

    public void Reset() 
    {
        hanger.gameObject.layer = 17;
        forks.gameObject.layer = 17;

        prepped = false;
        isClimbing = false;

        //Reset joints pos and rot and targetPos
        hanger.gameObject.transform.localPosition = hangerStartingPos;
        hanger.gameObject.transform.localRotation = hangerStartingRot;

        forks.gameObject.transform.localPosition = forksStartingPos;
        forks.gameObject.transform.localRotation = forksStartingRot;

        hanger.targetPosition = new Vector3(0f, 0f, 0f);
        
        JointSpring forksSpring = forks.spring;
        forksSpring.targetPosition = 0f;
        forks.spring = forksSpring;
        StartCoroutine(WaitToEnable());
    }
}
