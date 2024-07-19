using System.Collections;
using UnityEngine;

public class SlapDownIntake : MonoBehaviour, IResettable
{
    [SerializeField] private Transform intakePivot;

    private Quaternion stowRotation;
    private Quaternion ampRotation;

    [SerializeField] private float stowSpeed = 2f;
    [SerializeField] private float rotationSpeed = 45f;

    private bool atTarget = false;
    private bool isRotating = false;
    private bool isStowing = false;
    public bool isAmping { get; set; }

    private DriveController controller;

    private void Start() 
    {
        controller = GetComponent<DriveController>();

        stowRotation = intakePivot.localRotation;
        ampRotation = Quaternion.Euler(intakePivot.localEulerAngles.x + 20f, intakePivot.localEulerAngles.y, intakePivot.localEulerAngles.z);
    }

    private void Update()
    {
        if (controller.isIntaking && !isStowing && !isAmping) 
        {
            RotateIntake(110f);
        }
        else if (!isRotating && !isAmping)
        {
            StowIntake();
        }
        else if (!isAmping)
        {
            RotateIntake(110f);
        }
    }

    private void RotateIntake(float targetAngle)
    {
        if (!atTarget)
        {
            isRotating = true;
            Quaternion targetRotation = Quaternion.Euler(targetAngle, 0, 0);
            intakePivot.localRotation = Quaternion.RotateTowards(intakePivot.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
            if (Quaternion.Angle(intakePivot.localRotation, targetRotation) < 0.1f)
            {
                isRotating = false;
                atTarget = true;
            }
        }
    }

    public void StowIntake()
    {
        isStowing = true;
        atTarget = false;
        intakePivot.localRotation = Quaternion.RotateTowards(intakePivot.localRotation, stowRotation, stowSpeed * Time.deltaTime);
        if (intakePivot.localRotation == stowRotation)
        {
            isStowing = false;
        }
    }

    public void MoveForAmp()
    {
        isAmping = true;
        if (isRotating) { isRotating = false; }
        StartCoroutine(RotateToAmpRotation());
    }

    private IEnumerator RotateToAmpRotation()
    {
        float t = 0f;
        Quaternion startRotation = intakePivot.localRotation;
        
        while (t < 1f)
        {
            t += Time.deltaTime * 8f;
            intakePivot.localRotation = Quaternion.Slerp(startRotation, ampRotation, t);
            yield return null;
        }
        
        intakePivot.localRotation = ampRotation;
    }

    public void Reset() 
    {   
        StopAllCoroutines();
        intakePivot.localRotation = stowRotation;
        atTarget = false;
        isRotating = false;
        isStowing = false;
        isAmping = false;
    }
}
