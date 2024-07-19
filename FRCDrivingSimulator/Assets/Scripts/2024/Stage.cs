using UnityEngine;

public class Stage : MonoBehaviour
{
    public bool isRedStage;
    public Collider stageCollider;

    private bool robotIsInStage = false;
    private bool secondaryRobotIsInStage = false;

    private void Update()
    {
        if (GameManager.GameState == GameState.Endgame || GameManager.endBuzzerPlaying)
        {
            stageCollider.enabled = true;
        }
        else
        {
            stageCollider.enabled = false;
        }

        //Just to be safe and catch any cases where a score glitch could happen
        if (isRedStage && GameScoreTracker.RedStagePoints < 0)
        {
            GameScoreTracker.RedStagePoints = 0;
        }
        else if (!isRedStage && GameScoreTracker.BlueStagePoints < 0)  
        {
            GameScoreTracker.BlueStagePoints = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (stageCollider.bounds.Intersects(other.bounds) && GameManager.GameState != GameState.End)
        {
            if ((isRedStage && other.gameObject.CompareTag("RedPlayer")) || (!isRedStage && other.gameObject.CompareTag("Player")))
            {
                if (!robotIsInStage)
                {
                    if (isRedStage)
                    {
                        GameScoreTracker.RedStagePoints += 1;
                        Score.redScore += 1;
                        robotIsInStage = true;
                    }
                    else
                    {
                        GameScoreTracker.BlueStagePoints += 1;
                        Score.blueScore += 1;
                        robotIsInStage = true;
                    }
                }
            }
            else if ((isRedStage && other.gameObject.CompareTag("RedPlayer2")) || (!isRedStage && other.gameObject.CompareTag("Player2"))) 
            {
                if (!secondaryRobotIsInStage)
                {
                    if (isRedStage)
                    {
                        GameScoreTracker.RedStagePoints += 1;
                        Score.redScore += 1;
                        secondaryRobotIsInStage = true;
                    }
                    else
                    {
                        GameScoreTracker.BlueStagePoints += 1;
                        Score.blueScore += 1;
                        secondaryRobotIsInStage = true;
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (robotIsInStage && GameManager.GameState != GameState.End)
        {
            if (isRedStage && other.gameObject.CompareTag("RedPlayer"))
            {
                GameScoreTracker.RedStagePoints -= 1;
                Score.redScore -= 1;
                robotIsInStage = false;
            }
            else if (!isRedStage && other.gameObject.CompareTag("Player"))
            {
                GameScoreTracker.BlueStagePoints -= 1;
                Score.blueScore -= 1;
                robotIsInStage = false;
            }
        }
        else if (secondaryRobotIsInStage && GameManager.GameState != GameState.End)
        {
            if (isRedStage && other.gameObject.CompareTag("RedPlayer2"))
            {
                GameScoreTracker.RedStagePoints -= 1;
                Score.redScore -= 1;
                secondaryRobotIsInStage = false;
            }
            else if (!isRedStage && other.gameObject.CompareTag("Player2"))
            {
                GameScoreTracker.BlueStagePoints -= 1;
                Score.blueScore -= 1;
                secondaryRobotIsInStage = false;
            }
        }
    }
}
