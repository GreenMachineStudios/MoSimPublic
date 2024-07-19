using UnityEngine;
using TMPro;

public class RedRobotManager : MonoBehaviour 
{
    public RobotSelector rs;

    public GameObject shotBlocker;

    public TextMeshProUGUI nameText;
    public UnityEngine.UI.Image robotImage;

    private int selectedOption = 0;
    
    void Start() 
    {
        if (PlayerPrefs.HasKey("redRobotSettings")) 
        {
            selectedOption = PlayerPrefs.GetInt("redRobotSettings");
        }
        UpdateCharacter(selectedOption);
        UpdateShotBlockerToggle();
    }

    public void NextOption() 
    {
        selectedOption++;

        if (selectedOption >= rs.RobotCount) 
        {
            selectedOption = 0;
        }

        UpdateCharacter(selectedOption);
        PlayerPrefs.SetInt("redRobotSettings", selectedOption);

        UpdateShotBlockerToggle();
    }

    public void BackOption() 
    {
        selectedOption--;

        if (selectedOption < 0) 
        {
            selectedOption = rs.RobotCount - 1;
        }

        UpdateCharacter(selectedOption);
        PlayerPrefs.SetInt("redRobotSettings", selectedOption);

        UpdateShotBlockerToggle();
    }

    private void UpdateShotBlockerToggle() 
    {
        //Shotblocker only for citrus circuits
        if (selectedOption == 2)
        {
            shotBlocker.SetActive(true);
        }
        else 
        {
            shotBlocker.SetActive(false);
        }
    }

    private void UpdateCharacter(int selectedOption) 
    {   
        Robot robot = rs.GetRobot(selectedOption);
        robotImage.sprite = robot.robotImage;
        nameText.text = robot.robotName;
    }
}
