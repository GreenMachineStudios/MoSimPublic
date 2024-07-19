using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public static float blueScore = 0f;
    public static float redScore = 0f;
    public TextMeshProUGUI redScoreText;
    public TextMeshProUGUI blueScoreText;

    private void Start()
    {
        ResetScores();
    }

    private void Update()
    {
        redScoreText.text = redScore.ToString();
        blueScoreText.text = blueScore.ToString();
    }

    public static void AddScore(int score, Alliance alliance) 
    {
        if (alliance == Alliance.Blue) 
        {
            blueScore += score;
        }
        else 
        {
            redScore += score;
        }
    }

    public static void SubScore(int score, Alliance alliance) 
    {
        if (alliance == Alliance.Blue) 
        {
            blueScore -= score;
        }
        else 
        {
            redScore -= score;
        }
    }

    public static void ResetScores()
    {
        blueScore = 0f;
        redScore = 0f;
    }
}
