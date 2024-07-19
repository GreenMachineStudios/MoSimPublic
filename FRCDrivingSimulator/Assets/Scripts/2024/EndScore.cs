using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScore : MonoBehaviour
{
    [SerializeField] private List<Sprite> robotIcons;
    [SerializeField] private List<int> robotNumbers;

    [SerializeField] private Sprite firstLogo;

    [SerializeField] private GameObject blueWins;
    [SerializeField] private GameObject redWins;

    //Red Alliance
    [SerializeField] private TextMeshProUGUI redLeave;
    [SerializeField] private TextMeshProUGUI redSpeaker;
    [SerializeField] private TextMeshProUGUI redAmp;
    [SerializeField] private TextMeshProUGUI redStage;
    [SerializeField] private TextMeshProUGUI redPenalty;

    [SerializeField] private TextMeshProUGUI redRobotNumber1;
    [SerializeField] private TextMeshProUGUI redRobotNumber2;
    [SerializeField] private TextMeshProUGUI redRobotNumber3;

    [SerializeField] private Image redRobotIcon1;
    [SerializeField] private Image redRobotIcon2;
    [SerializeField] private Image redRobotIcon3;

    [SerializeField] private TextMeshProUGUI redScore;

    //Blue Alliance
    [SerializeField] private TextMeshProUGUI blueLeave;
    [SerializeField] private TextMeshProUGUI blueSpeaker;
    [SerializeField] private TextMeshProUGUI blueAmp;
    [SerializeField] private TextMeshProUGUI blueStage;
    [SerializeField] private TextMeshProUGUI bluePenalty;

    [SerializeField] private TextMeshProUGUI blueRobotNumber1;
    [SerializeField] private TextMeshProUGUI blueRobotNumber2;
    [SerializeField] private TextMeshProUGUI blueRobotNumber3;

    [SerializeField] private Image blueRobotIcon1;
    [SerializeField] private Image blueRobotIcon2;
    [SerializeField] private Image blueRobotIcon3;

    [SerializeField] private TextMeshProUGUI blueScore;

    private void OnEnable()
    {
        //Red Alliance Score Calculation
        redLeave.text = GameScoreTracker.RedAutoLeavePoints.ToString();
        redSpeaker.text = (GameScoreTracker.RedAutoSpeakerPoints + GameScoreTracker.RedTeleopSpeakerPoints).ToString();
        redAmp.text = (GameScoreTracker.RedAutoAmpPoints + GameScoreTracker.RedTeleopAmpPoints).ToString();
        redStage.text = (GameScoreTracker.RedStagePoints + GameScoreTracker.RedTrapPoints).ToString();
        redPenalty.text = (GameScoreTracker.RedAutoPenaltyPoints + GameScoreTracker.RedTeleopPenaltyPoints).ToString();

        redScore.text = Score.redScore.ToString();

        //Blue Alliance Score Calculation
        blueLeave.text = GameScoreTracker.BlueAutoLeavePoints.ToString();
        blueSpeaker.text = (GameScoreTracker.BlueAutoSpeakerPoints + GameScoreTracker.BlueTeleopSpeakerPoints).ToString();
        blueAmp.text = (GameScoreTracker.BlueAutoAmpPoints + GameScoreTracker.BlueTeleopAmpPoints).ToString();
        blueStage.text = (GameScoreTracker.BlueStagePoints + GameScoreTracker.BlueTrapPoints).ToString();
        bluePenalty.text = (GameScoreTracker.BlueAutoPenaltyPoints + GameScoreTracker.BlueTeleopPenaltyPoints).ToString();

        blueScore.text = Score.blueScore.ToString();
        
        ResetRobotFields();

        if (Score.blueScore > Score.redScore) 
        {
            blueWins.SetActive(true);
        }
        else if (Score.redScore > Score.blueScore) 
        {
            redWins.SetActive(true);
        }
        else 
        {
            //its a tie so show tie ui
        }

        int redRobot = PlayerPrefs.GetInt("redRobotSettings");
        int blueRobot = PlayerPrefs.GetInt("blueRobotSettings");

        int gamemode = PlayerPrefs.GetInt("gamemode");
        string alliance = PlayerPrefs.GetString("alliance");

        if (gamemode == 0) 
        {
            if (alliance == "blue") 
            {
                blueRobotNumber1.text = robotNumbers[blueRobot].ToString();
                blueRobotIcon1.sprite = robotIcons[blueRobot];
            }
            else 
            {
                redRobotNumber1.text = robotNumbers[redRobot].ToString();
                redRobotIcon1.sprite = robotIcons[redRobot];
            }
        }       
        else if (gamemode == 1) 
        {
            blueRobotNumber1.text = robotNumbers[blueRobot].ToString();
            blueRobotIcon1.sprite = robotIcons[blueRobot];

            redRobotNumber1.text = robotNumbers[redRobot].ToString();
            redRobotIcon1.sprite = robotIcons[redRobot];
        }
        else 
        {
            if (alliance == "blue") 
            {
                blueRobotNumber1.text = robotNumbers[blueRobot].ToString();
                blueRobotIcon1.sprite = robotIcons[blueRobot];

                blueRobotNumber2.text = robotNumbers[redRobot].ToString();
                blueRobotIcon2.sprite = robotIcons[redRobot];
            }
            else 
            {
                redRobotNumber1.text = robotNumbers[redRobot].ToString();
                redRobotIcon1.sprite = robotIcons[redRobot];

                redRobotNumber2.text = robotNumbers[blueRobot].ToString();
                redRobotIcon2.sprite = robotIcons[blueRobot];
            }
        }
    }

    private void ResetRobotFields() 
    {
        string startingNumber = "0000";

        redRobotNumber1.text = startingNumber;
        redRobotNumber2.text = startingNumber;
        redRobotNumber3.text = startingNumber;

        blueRobotNumber1.text = startingNumber;
        blueRobotNumber2.text = startingNumber;
        blueRobotNumber3.text = startingNumber;

        redRobotIcon1.sprite = firstLogo;
        redRobotIcon2.sprite = firstLogo;
        redRobotIcon3.sprite = firstLogo;

        blueRobotIcon1.sprite = firstLogo;
        blueRobotIcon2.sprite = firstLogo;
        blueRobotIcon3.sprite = firstLogo;

        blueWins.SetActive(false);
        redWins.SetActive(false);
    }
}
