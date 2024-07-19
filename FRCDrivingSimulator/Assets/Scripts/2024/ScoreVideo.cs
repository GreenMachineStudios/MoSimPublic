using UnityEngine;
using UnityEngine.Video;

public class ScoreVideo : MonoBehaviour
{
    public VideoClip blueWins;
    public VideoClip redWins;
    public VideoClip tie;

    public VideoPlayer player;
    public GameObject screen;
    public GameObject button;

    private bool videoEnded = false;

    public void OnEnable() 
    {
        screen.SetActive(true);

        if (Score.redScore > Score.blueScore) 
        {
            player.clip = redWins;
        }
        else if (Score.blueScore > Score.redScore) 
        {
            player.clip = blueWins;
        }
        else 
        {
            player.clip = tie;
        }

        player.loopPointReached += EndOfVideo;
        player.Play();
        GameManager.ToggleCanRobotMove();
    }

    private void EndOfVideo(VideoPlayer vp)
    {
        videoEnded = true;
    }

    private void Update()
    {
        if (videoEnded && !player.isPlaying)
        {
            GameManager.ToggleCanRobotMove();
            screen.SetActive(false);
            button.SetActive(true);
            videoEnded = false;
        }
    }

    private void OnDisable()
    {
        player.loopPointReached -= EndOfVideo;
    }
}
