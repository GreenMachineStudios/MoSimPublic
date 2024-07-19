using UnityEngine;

public class G414Penalty : MonoBehaviour
{
    [SerializeField] private Alliance alliance;
    [SerializeField] private AudioSource alliancePlayer;

    private bool gavePenalty;

    private const int G414_PENALTY_WORTH = 2;

    private void PenalizeScore() 
    {
        gavePenalty = true;

        alliancePlayer.Play();

        bool matchEnded = GameManager.GameState == GameState.End;
        if (!matchEnded) 
        {
            if (GameManager.GameState == GameState.Auto)
            {
                if (alliance == Alliance.Red) 
                {
                    GameScoreTracker.BlueAutoPenaltyPoints += G414_PENALTY_WORTH;
                }
                else 
                {
                    GameScoreTracker.RedAutoPenaltyPoints += G414_PENALTY_WORTH;
                }
            }
            else
            {
                if (alliance == Alliance.Red)
                {
                    GameScoreTracker.BlueTeleopPenaltyPoints += G414_PENALTY_WORTH;
                }
                else 
                {
                    GameScoreTracker.RedTeleopPenaltyPoints += G414_PENALTY_WORTH;
                }
                
            }
            
            if (alliance == Alliance.Blue) { Score.redScore += G414_PENALTY_WORTH; }
            else { Score.blueScore += G414_PENALTY_WORTH; }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!gavePenalty) 
        {
            if (alliance == Alliance.Blue) 
            {
                if (other.gameObject.CompareTag("noteShotByBlue") && ZoneControl.blueRobotInRedZone || other.gameObject.CompareTag("noteShotByBlue2") && ZoneControl.blueOtherRobotInRedZone)
                {
                    other.tag = "Ring";
                    PenalizeScore();
                }
            }
            else 
            {
                if (other.gameObject.CompareTag("noteShotByRed") && ZoneControl.redRobotInBlueZone || other.gameObject.CompareTag("noteShotByRed2") && ZoneControl.redOtherRobotInBlueZone) 
                {
                    other.tag = "Ring";
                    PenalizeScore();
                }
            }
        }
        gavePenalty = false;
    }
}
