using System.Collections;
using UnityEngine;

public class HingeJointResetter : MonoBehaviour, IResettable
{
    [SerializeField] private HingeJoint hinge;

    private int startingLayer;

    private Vector3 hingeStartingPos;
    private Quaternion hingeStartingRot;

    private void Start() 
    {
        startingLayer = hinge.gameObject.layer;
        hingeStartingPos = hinge.gameObject.transform.localPosition;
        hingeStartingRot = hinge.gameObject.transform.localRotation;
    }

    private IEnumerator WaitToEnable()
    {
        yield return new WaitForSeconds(0.01f);
        hinge.gameObject.layer = startingLayer;
    }

    public void Reset() 
    {
        //Set layer to ignore all physics collisions
        hinge.gameObject.layer = 17;

        //Reset joints pos and rot and targetpos
        hinge.gameObject.transform.localPosition = hingeStartingPos;
        hinge.gameObject.transform.localRotation = hingeStartingRot;
        
        JointSpring hingeSpring = hinge.spring;
        hingeSpring.targetPosition = 0f;
        hinge.spring = hingeSpring;
        StartCoroutine(WaitToEnable());
    }
}
