using UnityEngine;

public class Tape : MonoBehaviour, IResettable
{
    private bool triggeredMobilityScore;
    private bool triggeredMobilityScoreForSecondaryPlayer;
    private bool isThereASecondaryPlayer;
    public bool isRedTape;
    
    private void Start()
    {
        Reset();

        if (RobotSpawnController.sameAlliance) 
        {
            isThereASecondaryPlayer = true;
        }
    }

    public void Reset() 
    {
        triggeredMobilityScore = false;
        triggeredMobilityScoreForSecondaryPlayer = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isRedTape) 
        {
            if (other.gameObject.CompareTag("Player") && GameManager.GameState == GameState.Auto && !triggeredMobilityScore)
            {
                triggeredMobilityScore = true;
                Score.blueScore += 2;
                GameScoreTracker.BlueAutoLeavePoints += 2;
            }
            else if (isThereASecondaryPlayer && other.gameObject.CompareTag("Player2") && GameManager.GameState == GameState.Auto && !triggeredMobilityScoreForSecondaryPlayer) 
            {
                triggeredMobilityScoreForSecondaryPlayer = true;
                Score.blueScore += 2;
                GameScoreTracker.BlueAutoLeavePoints += 2;
            }
        }
        else
        {
            if (other.gameObject.CompareTag("RedPlayer") && GameManager.GameState == GameState.Auto && !triggeredMobilityScore)
            {
                triggeredMobilityScore = true;
                Score.redScore += 2;
                GameScoreTracker.RedAutoLeavePoints += 2;
            }
            else if (isThereASecondaryPlayer && other.gameObject.CompareTag("RedPlayer2") && GameManager.GameState == GameState.Auto && !triggeredMobilityScoreForSecondaryPlayer) 
            {
                triggeredMobilityScoreForSecondaryPlayer = true;
                Score.redScore += 2;
                GameScoreTracker.RedAutoLeavePoints += 2;
            }
        }
    }
}
