using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField blueRobotName;
    [SerializeField] private TMP_InputField redRobotName;

    [SerializeField] private Toggle blueShotBlocker;
    [SerializeField] private Toggle redShotBlocker;

    [SerializeField] private TMP_Dropdown graphicsDropdown;
    [SerializeField] private TMP_Dropdown gamemodeDropdown;
    [SerializeField] private TMP_Dropdown cameraDropdown;

    [SerializeField] private TMP_Dropdown blueSourceDropdown;
    [SerializeField] private TMP_Dropdown redSourceDropdown;

    [SerializeField] private Slider volumeSlider;

    [SerializeField] private GameObject allianceToggle;

    [SerializeField] private GameObject blueRobotSelector;
    [SerializeField] private GameObject redRobotSelector;

    [SerializeField] private Slider movespeed;
    [SerializeField] private Slider rotatespeed;

    [SerializeField] private TextMeshProUGUI blueAllianceHeader;
    [SerializeField] private TextMeshProUGUI redAllianceHeader;

    [SerializeField] private GameObject blueNameField;
    [SerializeField] private GameObject redNameField;

    [SerializeField] private TextMeshProUGUI blueNameFieldText;
    [SerializeField] private TextMeshProUGUI redNameFieldText;

    [SerializeField] private Color blueAllianceHeaderColor;
    [SerializeField] private Color redAllianceHeaderColor;

    [SerializeField] private Color blueNameFieldColor;
    [SerializeField] private Color redNameFieldColor;

    [SerializeField] private Color blueNameFieldTextColor;
    [SerializeField] private Color redNameFieldTextColor;

    [SerializeField] private Color blueShotBlockerToggleImageColor;
    [SerializeField] private Color redShotBlockerToggleImageColor;

    [SerializeField] private Image blueShotBlockerToggleImage;
    [SerializeField] private Image redShotBlockerToggleImage;

    [SerializeField] private Text blueShotBlockerToggleText;
    [SerializeField] private Text redShotBlockerToggleText;

    private bool toggled = false;

    [SerializeField] private Toggle matchVideo;

    [SerializeField] private Toggle swerveSounds;
    [SerializeField] private Toggle bumpSounds;
    [SerializeField] private Toggle intakeSounds;
    [SerializeField] private Toggle logoFlash;

    [SerializeField] private GameObject stealthLogo;
    private readonly float activationInterval = 0.15f;

    private void Start()
    {
        blueAllianceHeader.color = blueAllianceHeaderColor;
        redAllianceHeader.color = redAllianceHeaderColor;

        blueNameField.GetComponent<Image>().color = blueNameFieldColor;
        redNameField.GetComponent<Image>().color = redNameFieldColor;

        blueNameFieldText.color = blueNameFieldTextColor;
        redNameFieldText.color = redNameFieldTextColor;

        blueShotBlockerToggleImage.color = blueShotBlockerToggleImageColor;
        redShotBlockerToggleImage.color = redShotBlockerToggleImageColor;

        blueShotBlockerToggleText.color = blueShotBlockerToggleImageColor;
        redShotBlockerToggleText.color = redShotBlockerToggleImageColor;
        
        blueRobotName.text = PlayerPrefs.GetString("blueName");
        redRobotName.text = PlayerPrefs.GetString("redName");

        volumeSlider.value = PlayerPrefs.GetFloat("volumeSlider");

        blueShotBlocker.isOn = PlayerPrefs.GetInt("blueShotBlocker") == 1;
        redShotBlocker.isOn = PlayerPrefs.GetInt("redShotBlocker") == 1;

        swerveSounds.isOn = PlayerPrefs.GetInt("swerveSounds") == 1;
        bumpSounds.isOn = PlayerPrefs.GetInt("bumpSounds") == 1;
        intakeSounds.isOn = PlayerPrefs.GetInt("intakeSounds") == 1;
        logoFlash.isOn = PlayerPrefs.GetInt("logoFlash") == 1;

        matchVideo.isOn = PlayerPrefs.GetFloat("endVideo") == 1f;
        movespeed.value = PlayerPrefs.GetFloat("movespeed");
        rotatespeed.value = PlayerPrefs.GetFloat("rotatespeed");

        blueSourceDropdown.value = PlayerPrefs.GetInt("blueSource");
        redSourceDropdown.value = PlayerPrefs.GetInt("redSource");

        graphicsDropdown.value = PlayerPrefs.GetInt("quality");
        gamemodeDropdown.value = PlayerPrefs.GetInt("gamemode");
        cameraDropdown.value = PlayerPrefs.GetInt("cameraMode");
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("quality"));

        if (PlayerPrefs.GetInt("gamemode") == 0) 
        {
            redRobotSelector.SetActive(PlayerPrefs.GetString("alliance") == "red");
            blueRobotSelector.SetActive(PlayerPrefs.GetString("alliance") == "blue");
            allianceToggle.SetActive(true);
        }
        else if (PlayerPrefs.GetInt("gamemode") == 2) 
        {
            redRobotSelector.SetActive(true);
            blueRobotSelector.SetActive(true);
            allianceToggle.SetActive(true);

            if (PlayerPrefs.GetString("alliance") == "blue") 
            {
                redSourceDropdown.gameObject.SetActive(false);
                blueSourceDropdown.gameObject.SetActive(true);

                redNameField.GetComponent<Image>().color = blueNameFieldColor;
                blueNameField.GetComponent<Image>().color = blueNameFieldColor;

                redNameFieldText.color = blueNameFieldTextColor;
                blueNameFieldText.color = blueNameFieldTextColor;

                blueShotBlockerToggleImage.color = blueShotBlockerToggleImageColor;
                redShotBlockerToggleImage.color = blueShotBlockerToggleImageColor;
                blueShotBlockerToggleText.color = blueShotBlockerToggleImageColor;
                redShotBlockerToggleText.color = blueShotBlockerToggleImageColor;
            }
            else 
            {
                blueSourceDropdown.gameObject.SetActive(false);
                redSourceDropdown.gameObject.SetActive(true);

                redNameField.GetComponent<Image>().color = redNameFieldColor;
                blueNameField.GetComponent<Image>().color = redNameFieldColor;

                redNameFieldText.color = redNameFieldTextColor;
                blueNameFieldText.color = redNameFieldTextColor;

                blueShotBlockerToggleImage.color = redShotBlockerToggleImageColor;
                redShotBlockerToggleImage.color = redShotBlockerToggleImageColor;
                blueShotBlockerToggleText.color = redShotBlockerToggleImageColor;
                redShotBlockerToggleText.color = redShotBlockerToggleImageColor;
            }
        }
        else 
        {
            redRobotSelector.SetActive(true);
            blueRobotSelector.SetActive(true);
            allianceToggle.SetActive(false);
        }
    }

    private void OnEnable() 
    {
        if (PlayerPrefs.GetInt("logoFlash") == 1) 
        {
            StartCoroutine(ActivateDeactivateCoroutine());
        }
        else 
        {
            StopCoroutine(ActivateDeactivateCoroutine());
            stealthLogo.SetActive(true);
        }
    }

    private IEnumerator ActivateDeactivateCoroutine()
    {
        while (PlayerPrefs.GetInt("logoFlash") == 1)
        {
            yield return new WaitForSeconds(Random.Range(0, activationInterval * 2));

            stealthLogo.SetActive(!stealthLogo.activeSelf);
        }
    }

    public void PlayGame()
    {
        LevelManager.Instance.LoadScene("Crescendo", "CrossFade");
    }

    public void SetVolume(float volume) 
    {
        PlayerPrefs.SetFloat("volume", volume);
        PlayerPrefs.SetFloat("volumeSlider", volumeSlider.value);
    }

    public void SaveMoveSpeed() 
    {
        PlayerPrefs.SetFloat("movespeed", movespeed.value);
    }

    public void SaveRotateSpeed() 
    {
        PlayerPrefs.SetFloat("rotatespeed", rotatespeed.value);
    }

    public void SetBlueName() 
    {
        PlayerPrefs.SetString("blueName", blueRobotName.text);
    }

    public void SetRedName() 
    {
        PlayerPrefs.SetString("redName", redRobotName.text);
    }

    public void ToggleEndVideo(bool value) 
    {
        if (value) 
        {
            PlayerPrefs.SetFloat("endVideo", 1f);
        }
        else 
        {
            PlayerPrefs.SetFloat("endVideo", 0f);
        }
    }

    public void ToggleSwerveSounds(bool value)
    {
        if (value) { PlayerPrefs.SetInt("swerveSounds", 1); }
        else { PlayerPrefs.SetInt("swerveSounds", 0); }
    }

    public void ToggleBumpSounds(bool value)
    {
        if (value) { PlayerPrefs.SetInt("bumpSounds", 1); }
        else { PlayerPrefs.SetInt("bumpSounds", 0); }
    }

    public void ToggleIntakeSounds(bool value)
    {
        if (value) { PlayerPrefs.SetInt("intakeSounds", 1); }
        else { PlayerPrefs.SetInt("intakeSounds", 0); }
    }

    public void ToggleLogoFlash(bool value)
    {
        if (value) { PlayerPrefs.SetInt("logoFlash", 1); }
        else { PlayerPrefs.SetInt("logoFlash", 0); }
    }

    public void ToggleBlueShotBlocker(bool value)
    {
        if (value) { PlayerPrefs.SetInt("blueShotBlocker", 1); }
        else { PlayerPrefs.SetInt("blueShotBlocker", 0); }
    }

    public void ToggleRedShotBlocker(bool value)
    {
        if (value) { PlayerPrefs.SetInt("redShotBlocker", 1); }
        else { PlayerPrefs.SetInt("redShotBlocker", 0); }
    }

    public void ToggleBot(bool value) 
    {
        if (value) { PlayerPrefs.SetInt("bot", 1); }
        else { PlayerPrefs.SetInt("bot", 0); }
    }

    public void SetCamera() 
    {
        PlayerPrefs.SetInt("cameraMode", cameraDropdown.value);
    }

    public void SetGamemode() 
    {
        PlayerPrefs.SetInt("gamemode", gamemodeDropdown.value);
        if (gamemodeDropdown.value == 0) 
        {
            blueAllianceHeader.color = blueAllianceHeaderColor;
            redAllianceHeader.color = redAllianceHeaderColor;

            redAllianceHeader.text = "Red Alliance";
            blueAllianceHeader.text = "Blue Alliance";

            redRobotSelector.SetActive(PlayerPrefs.GetString("alliance") == "red");
            blueRobotSelector.SetActive(PlayerPrefs.GetString("alliance") == "blue");
            allianceToggle.SetActive(true);

            redSourceDropdown.gameObject.SetActive(true);
            blueSourceDropdown.gameObject.SetActive(true);

            redNameField.GetComponent<Image>().color = redNameFieldColor;
            blueNameField.GetComponent<Image>().color = blueNameFieldColor;

            redNameFieldText.color = redNameFieldTextColor;
            blueNameFieldText.color = blueNameFieldTextColor;

            blueShotBlockerToggleImage.color = blueShotBlockerToggleImageColor;
            redShotBlockerToggleImage.color = redShotBlockerToggleImageColor;
            blueShotBlockerToggleText.color = blueShotBlockerToggleImageColor;
            redShotBlockerToggleText.color = redShotBlockerToggleImageColor;
        }
        else if (gamemodeDropdown.value == 2) 
        {
            redRobotSelector.SetActive(true);
            blueRobotSelector.SetActive(true);
            allianceToggle.SetActive(true);

            if (PlayerPrefs.GetString("alliance") == "blue") 
            {
                redAllianceHeader.text = "Blue Alliance";
                blueAllianceHeader.text = "Blue Alliance";
                blueAllianceHeader.color = blueAllianceHeaderColor;
                redAllianceHeader.color = blueAllianceHeaderColor;

                redSourceDropdown.gameObject.SetActive(false);
                blueSourceDropdown.gameObject.SetActive(true);

                redNameField.GetComponent<Image>().color = blueNameFieldColor;
                blueNameField.GetComponent<Image>().color = blueNameFieldColor;

                redNameFieldText.color = blueNameFieldTextColor;
                blueNameFieldText.color = blueNameFieldTextColor;

                blueShotBlockerToggleImage.color = blueShotBlockerToggleImageColor;
                redShotBlockerToggleImage.color = blueShotBlockerToggleImageColor;
                blueShotBlockerToggleText.color = blueShotBlockerToggleImageColor;
                redShotBlockerToggleText.color = blueShotBlockerToggleImageColor;
            }
            else 
            {
                blueAllianceHeader.text = "Red Alliance";
                redAllianceHeader.text = "Red Alliance";
                redAllianceHeader.color = redAllianceHeaderColor;
                blueAllianceHeader.color = redAllianceHeaderColor;

                blueSourceDropdown.gameObject.SetActive(false);
                redSourceDropdown.gameObject.SetActive(true);

                redNameField.GetComponent<Image>().color = redNameFieldColor;
                blueNameField.GetComponent<Image>().color = redNameFieldColor;

                redNameFieldText.color = redNameFieldTextColor;
                blueNameFieldText.color = redNameFieldTextColor;

                blueShotBlockerToggleImage.color = redShotBlockerToggleImageColor;
                redShotBlockerToggleImage.color = redShotBlockerToggleImageColor;
                blueShotBlockerToggleText.color = redShotBlockerToggleImageColor;
                redShotBlockerToggleText.color = redShotBlockerToggleImageColor;
            }
        }
        else 
        {
            blueAllianceHeader.color = blueAllianceHeaderColor;
            redAllianceHeader.color = redAllianceHeaderColor;

            redAllianceHeader.text = "Red Alliance";
            blueAllianceHeader.text = "Blue Alliance";

            redRobotSelector.SetActive(true);
            blueRobotSelector.SetActive(true);
            allianceToggle.SetActive(false);

            redSourceDropdown.gameObject.SetActive(true);
            blueSourceDropdown.gameObject.SetActive(true);

            redNameField.GetComponent<Image>().color = redNameFieldColor;
            blueNameField.GetComponent<Image>().color = blueNameFieldColor;

            redNameFieldText.color = redNameFieldTextColor;
            blueNameFieldText.color = blueNameFieldTextColor;

            blueShotBlockerToggleImage.color = blueShotBlockerToggleImageColor;
            redShotBlockerToggleImage.color = redShotBlockerToggleImageColor;
            blueShotBlockerToggleText.color = blueShotBlockerToggleImageColor;
            redShotBlockerToggleText.color = redShotBlockerToggleImageColor;
        }
    }

    public void ToggleSingleplayerAlliance()
    {
        if (PlayerPrefs.GetInt("gamemode") == 0) 
        {
            if (toggled) 
            {
                toggled = false;
                PlayerPrefs.SetString("alliance", "blue");
                redRobotSelector.SetActive(false);
                blueRobotSelector.SetActive(true);
            }
            else 
            {
                toggled = true;
                PlayerPrefs.SetString("alliance", "red");
                blueRobotSelector.SetActive(false);
                redRobotSelector.SetActive(true);
            }
        }
        else 
        {
            if (toggled)
            {
                toggled = false;
                PlayerPrefs.SetString("alliance", "blue");
                redAllianceHeader.text = "Blue Alliance";
                blueAllianceHeader.text = "Blue Alliance";
                blueAllianceHeader.color = blueAllianceHeaderColor;
                redAllianceHeader.color = blueAllianceHeaderColor;

                redSourceDropdown.gameObject.SetActive(false);
                blueSourceDropdown.gameObject.SetActive(true);

                redNameField.GetComponent<Image>().color = blueNameFieldColor;
                blueNameField.GetComponent<Image>().color = blueNameFieldColor;

                redNameFieldText.color = blueNameFieldTextColor;
                blueNameFieldText.color = blueNameFieldTextColor;

                blueShotBlockerToggleImage.color = blueShotBlockerToggleImageColor;
                redShotBlockerToggleImage.color = blueShotBlockerToggleImageColor;
                blueShotBlockerToggleText.color = blueShotBlockerToggleImageColor;
                redShotBlockerToggleText.color = blueShotBlockerToggleImageColor;
            }
            else 
            {
                toggled = true;
                PlayerPrefs.SetString("alliance", "red");
                blueAllianceHeader.text = "Red Alliance";
                redAllianceHeader.text = "Red Alliance";
                redAllianceHeader.color = redAllianceHeaderColor;
                blueAllianceHeader.color = redAllianceHeaderColor;

                blueSourceDropdown.gameObject.SetActive(false);
                redSourceDropdown.gameObject.SetActive(true);

                redNameField.GetComponent<Image>().color = redNameFieldColor;
                blueNameField.GetComponent<Image>().color = redNameFieldColor;

                redNameFieldText.color = redNameFieldTextColor;
                blueNameFieldText.color = redNameFieldTextColor;

                blueShotBlockerToggleImage.color = redShotBlockerToggleImageColor;
                redShotBlockerToggleImage.color = redShotBlockerToggleImageColor;
                blueShotBlockerToggleText.color = redShotBlockerToggleImageColor;
                redShotBlockerToggleText.color = redShotBlockerToggleImageColor;
            }
        }
    }

    public void SetBlueSource() 
    {
        PlayerPrefs.SetInt("blueSource", blueSourceDropdown.value);
    }

    public void SetRedSource() 
    {
        PlayerPrefs.SetInt("redSource", redSourceDropdown.value);
    }

    public void SetQuality(int index) 
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("quality", graphicsDropdown.value);
    }

    public void SetFullscreen(bool isFullscreen) 
    {
        Screen.fullScreen = isFullscreen;
    }

    public void ResetAllPrefs() 
    {
        PlayerPrefs.DeleteAll();
        volumeSlider.value = PlayerPrefs.GetFloat("volumeSlider");
        swerveSounds.isOn = PlayerPrefs.GetInt("swerveSounds") == 1;
        bumpSounds.isOn = PlayerPrefs.GetInt("bumpSounds") == 1;
        intakeSounds.isOn = PlayerPrefs.GetInt("intakeSounds") == 1;
        logoFlash.isOn = PlayerPrefs.GetInt("logoFlash") == 1;
        graphicsDropdown.value = PlayerPrefs.GetInt("quality");
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("quality"));
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
