using UnityEngine;

public class IntakeHandler : MonoBehaviour
{
    [SerializeField] private RobotNoteManager robotCollisionsScript;
    private GameObject note;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ring") || other.gameObject.CompareTag("noteShotByRed") || other.gameObject.CompareTag("noteShotByBlue"))
        {
            note = other.gameObject;
            robotCollisionsScript.ringWithinIntakeCollider = true;
            robotCollisionsScript.touchedRing = other.gameObject;
        }
    }

    private void OnTriggerStay(Collider other) 
    {
        if (other.gameObject.CompareTag("Ring") || other.gameObject.CompareTag("noteShotByRed") || other.gameObject.CompareTag("noteShotByBlue"))
        {
            note = other.gameObject;
            robotCollisionsScript.ringWithinIntakeCollider = true;
            robotCollisionsScript.touchedRing = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ring"))
        {
            note = null;
            robotCollisionsScript.ringWithinIntakeCollider = false;
            robotCollisionsScript.touchedRing = null;
        }
    }

    private void Update() 
    {
        if (note != null && !note.activeSelf || note == null)
        {
            note = null;
            robotCollisionsScript.ringWithinIntakeCollider = false;
            robotCollisionsScript.touchedRing = null;
        }
    }
} 
