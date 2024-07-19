using UnityEngine;

public class GameScoreTracker : MonoBehaviour
{
    public static int BlueTeleopSpeakerPoints { get; set; }
    public static int BlueAutoSpeakerPoints { get; set; }
    public static int BlueTeleopAmpPoints { get; set; }
    public static int BlueAutoAmpPoints { get; set; }
    public static int BlueAutoLeavePoints { get; set; }
    public static int BlueAutoPenaltyPoints { get; set; }
    public static int BlueTeleopPenaltyPoints { get; set; }
    public static int BlueStagePoints { get; set; }
    public static int BlueTrapPoints { get; set; }

    public static int RedTeleopSpeakerPoints { get; set; }
    public static int RedAutoSpeakerPoints { get; set; }
    public static int RedTeleopAmpPoints { get; set; }
    public static int RedAutoAmpPoints { get; set; }
    public static int RedAutoLeavePoints { get; set; }
    public static int RedAutoPenaltyPoints { get; set; }
    public static int RedTeleopPenaltyPoints { get; set; }
    public static int RedStagePoints { get; set; }
    public static int RedTrapPoints { get; set; }

    private void Start()
    {
        ResetScore();
    }

    public static void ResetScore() 
    {
        BlueTeleopSpeakerPoints = 0;
        BlueAutoSpeakerPoints = 0;
        BlueTeleopAmpPoints = 0;
        BlueAutoAmpPoints = 0;
        BlueAutoLeavePoints = 0;
        BlueAutoPenaltyPoints = 0;
        BlueTeleopPenaltyPoints = 0;
        BlueStagePoints = 0;
        BlueTrapPoints = 0;

        RedTeleopSpeakerPoints = 0;
        RedAutoSpeakerPoints = 0;
        RedTeleopAmpPoints = 0;
        RedAutoAmpPoints = 0;
        RedAutoLeavePoints = 0;
        RedAutoPenaltyPoints = 0;
        RedTeleopPenaltyPoints = 0;
        RedStagePoints = 0;
        RedTrapPoints = 0;
    }
}
