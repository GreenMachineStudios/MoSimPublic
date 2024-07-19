using UnityEngine;
using UnityEngine.SceneManagement;

public class ToggleScoreCard : MonoBehaviour
{
    public GameObject scoreCard;
    private bool isEnabled = false;

    public void ToggleCard()
    {
        if (!isEnabled)
        {
            scoreCard.SetActive(true);
            isEnabled = true;
        }
        else 
        {
            scoreCard.SetActive(false);
            isEnabled = false;
        }
    }

    public void LoadEndScoreScene() 
    {
        LevelManager.Instance.LoadScene("ScoreDisplay", "CrossFade");
    }
}
