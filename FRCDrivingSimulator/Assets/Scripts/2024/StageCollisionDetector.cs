using UnityEngine;

public class StageCollisionDetector : MonoBehaviour
{
    public bool robotInStage = false;
    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player") || other.CompareTag("Player2") || other.CompareTag("RedPlayer") || other.CompareTag("RedPlayer2")) 
        {
            robotInStage = true;
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.CompareTag("Player") || other.CompareTag("Player2") || other.CompareTag("RedPlayer") || other.CompareTag("RedPlayer2")) 
        {
            robotInStage = false;
        }
    }
}
