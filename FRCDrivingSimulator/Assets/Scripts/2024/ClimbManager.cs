using UnityEngine;

public class ClimbManager : MonoBehaviour
{
    [SerializeField] private DriveController[] blueRobots;
    [SerializeField] private DriveController[] redRobots;

    private void Update() 
    {
        if (GameManager.GameState == GameState.Endgame || GameManager.endBuzzerPlaying)
        {
            foreach (DriveController robot in blueRobots)
            {
                //Only climb for enabled robots that are hightide & citrusciruits (only robot that can climb)
                if (robot.isActiveAndEnabled)
                {
                    if (robot.robotType == RobotSettings.HighTide || robot.robotType == RobotSettings.CitrusCircuits || robot.robotType == RobotSettings.Killshon) 
                    {
                        if (!robot.isTouchingGround && !robot.isClimbed && ChainDetector.isBlueTouchingChain)
                        {
                            robot.isClimbed = true;
                            GameScoreTracker.BlueStagePoints += 3;
                            Score.blueScore += 3;
                        }
                        else if (robot.isClimbed && robot.isTouchingGround)
                        {
                            robot.isClimbed = false;
                            GameScoreTracker.BlueStagePoints -= 3;
                            Score.blueScore -= 3;
                        }
                    }
                }
            }

            foreach (DriveController robot in redRobots)
            {
                //Only climb for enabled robots that are hightide & citrusciruits (only robot that can climb)
                if (robot.isActiveAndEnabled)
                {
                    if (robot.robotType == RobotSettings.HighTide || robot.robotType == RobotSettings.CitrusCircuits || robot.robotType == RobotSettings.Killshon) 
                    {
                        if (!robot.isTouchingGround && !robot.isClimbed && ChainDetector.isRedTouchingChain)
                        {
                            robot.isClimbed = true;
                            GameScoreTracker.RedStagePoints += 3;
                            Score.redScore += 3;
                        }
                        else if (robot.isClimbed && robot.isTouchingGround) 
                        {
                            robot.isClimbed = false;
                            GameScoreTracker.RedStagePoints -= 3;
                            Score.redScore -= 3;
                        }
                    }
                }
            }
        }
    }
}
