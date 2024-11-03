using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class KillshonClimb : MonoBehaviour, IResettable
{
    [SerializeField] private ConfigurableJoint climber;

    private int startingLayer;

    private bool climb;
    private bool prepped = false;

    private Vector3 climberStartingPos;
    private Quaternion climberStartingRot;

    private void Start() 
    {
        climberStartingPos = climber.gameObject.transform.localPosition;
        climberStartingRot = climber.gameObject.transform.localRotation;

        startingLayer = climber.gameObject.layer;
    }

    private void Update()
    {
        if (climb && !prepped)
        {
            StartCoroutine(ClimbSequence());
        }
        else if (climb && prepped)
        {
            StartCoroutine(HangSequence());
        }
    }

    private IEnumerator ClimbSequence() 
    {
        climber.targetRotation = Quaternion.Euler(90, 0, 0);
        yield return new WaitForSeconds(0.5f);
        prepped = true;
    }

    private IEnumerator HangSequence() 
    {
        climber.targetRotation = Quaternion.Euler(0, 0, 0);
        yield return new WaitForSeconds(0.5f);
        prepped = false;
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

        //Reset joints pos and rot and targetPos
        climber.gameObject.transform.localPosition = climberStartingPos;
        climber.gameObject.transform.localRotation = climberStartingRot;

        climber.targetRotation = Quaternion.Euler(0, 0, 0);

        StartCoroutine(WaitToEnable());
    }
}
