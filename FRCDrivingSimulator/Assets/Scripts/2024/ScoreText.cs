using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    [SerializeField] RobotSelector redRobotSelector;
    [SerializeField] RobotSelector blueRobotSelector;

    public TextMeshProUGUI redAutoSpeakerScore;
    public TextMeshProUGUI redAutoAmpScore;
    public TextMeshProUGUI redAutoLeaveScore;
    public TextMeshProUGUI redAutoPenalty;

    public TextMeshProUGUI redTeleopSpeakerScore;
    public TextMeshProUGUI redTeleopAmpScore;
    public TextMeshProUGUI redStageScore;
    public TextMeshProUGUI redTeleopPenalty;
    public TextMeshProUGUI redTrapScore;

    public TextMeshProUGUI totalRedScore;

    public TextMeshProUGUI blueAutoSpeakerScore;
    public TextMeshProUGUI blueAutoAmpScore;
    public TextMeshProUGUI blueAutoLeaveScore;
    public TextMeshProUGUI blueAutoPenalty;

    public TextMeshProUGUI blueTeleopSpeakerScore;
    public TextMeshProUGUI blueTeleopAmpScore;
    public TextMeshProUGUI blueStageScore;
    public TextMeshProUGUI blueTeleopPenalty;
    public TextMeshProUGUI blueTrapScore;

    public TextMeshProUGUI totalBlueScore;

    public TextMeshProUGUI redRobot;
    public TextMeshProUGUI blueRobot;

    public TextMeshProUGUI otherRedRobot;
    public TextMeshProUGUI otherBlueRobot;

    private void OnEnable()
    {
        //red auto stuff
        redAutoAmpScore.text = "Amp: " + GameScoreTracker.RedAutoAmpPoints.ToString();
        redAutoSpeakerScore.text = "Speaker: " + GameScoreTracker.RedAutoSpeakerPoints.ToString();
        redAutoLeaveScore.text = "Mobility: " + GameScoreTracker.RedAutoLeavePoints.ToString();
        redAutoPenalty.text = "Penalty: " + GameScoreTracker.RedAutoPenaltyPoints.ToString();

        //red teleop stuff
        redTeleopAmpScore.text = "Amp: " + GameScoreTracker.RedTeleopAmpPoints.ToString();
        redTeleopSpeakerScore.text = "Speaker: " + GameScoreTracker.RedTeleopSpeakerPoints.ToString();
        redStageScore.text = "Stage: " + GameScoreTracker.RedStagePoints.ToString();
        redTrapScore.text = "Trap: " + GameScoreTracker.RedTrapPoints.ToString();
        redTeleopPenalty.text = "Penalty: " + GameScoreTracker.RedTeleopPenaltyPoints.ToString();

        totalRedScore.text = Score.redScore.ToString();

        //blue auto stuff
        blueAutoAmpScore.text = "Amp: " + GameScoreTracker.BlueAutoAmpPoints.ToString();
        blueAutoSpeakerScore.text = "Speaker: " + GameScoreTracker.BlueAutoSpeakerPoints.ToString();
        blueAutoLeaveScore.text = "Mobility: " + GameScoreTracker.BlueAutoLeavePoints.ToString();
        blueAutoPenalty.text = "Penalty: " + GameScoreTracker.BlueAutoPenaltyPoints.ToString();

        //blue teleop stuff
        blueTeleopAmpScore.text = "Amp: " + GameScoreTracker.BlueTeleopAmpPoints.ToString();
        blueTeleopSpeakerScore.text = "Speaker: " + GameScoreTracker.BlueTeleopSpeakerPoints.ToString();
        blueStageScore.text = "Stage: " + GameScoreTracker.BlueStagePoints.ToString();
        blueTrapScore.text = "Trap: " + GameScoreTracker.BlueTrapPoints.ToString();
        blueTeleopPenalty.text = "Penalty: " + GameScoreTracker.BlueTeleopPenaltyPoints.ToString();

        totalBlueScore.text = Score.blueScore.ToString();

        int blue = PlayerPrefs.GetInt("blueRobotSettings");
        int red = PlayerPrefs.GetInt("redRobotSettings");

        if (PlayerPrefs.GetInt("gamemode") == 0) 
        {
            otherRedRobot.text = string.Empty;
            otherBlueRobot.text = string.Empty;
            if (PlayerPrefs.GetString("alliance") == "blue") 
            {
                blueRobot.text = blueRobotSelector.robot[blue].robotNumber.ToString();
                redRobot.text = string.Empty;
            }
            else 
            {
                redRobot.text = redRobotSelector.robot[red].robotNumber.ToString();
                blueRobot.text = string.Empty;
            }
        }
        else if (PlayerPrefs.GetInt("gamemode") == 1)
        {
            otherRedRobot.text = string.Empty;
            otherBlueRobot.text = string.Empty;

            redRobot.text = redRobotSelector.robot[red].robotNumber.ToString();
            blueRobot.text = blueRobotSelector.robot[blue].robotNumber.ToString();
        }
        else if (PlayerPrefs.GetInt("gamemode") == 2) 
        {
            if (PlayerPrefs.GetString("alliance") == "blue") 
            {
                blueRobot.text = blueRobotSelector.robot[blue].robotNumber.ToString();
                otherBlueRobot.text = redRobotSelector.robot[red].robotNumber.ToString();
                redRobot.text = string.Empty;
                otherRedRobot.text = string.Empty;
            }
            else 
            {
                redRobot.text = redRobotSelector.robot[red].robotNumber.ToString();
                otherRedRobot.text = blueRobotSelector.robot[blue].robotNumber.ToString();
                blueRobot.text = string.Empty;
                otherBlueRobot.text = string.Empty;
            }
        }
    }
}
