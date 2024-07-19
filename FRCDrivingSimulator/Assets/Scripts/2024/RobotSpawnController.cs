using UnityEngine;
using UnityEngine.InputSystem;

public class RobotSpawnController : MonoBehaviour
{    
    private int gamemode;
    private int cameraMode;
    private int blueRobot;
    private int redRobot;

    private bool multiplayer;
    public static bool sameAlliance = false;

    [SerializeField] private GameObject[] blueRobots;
    [SerializeField] private GameObject[] blueCameras;

    [SerializeField] private GameObject[] redRobots;
    [SerializeField] private GameObject[] redCameras;

    [SerializeField] private GameObject[] secondaryBlueRobots;
    [SerializeField] private GameObject[] secondaryBlueCameras;

    [SerializeField] private GameObject[] secondaryRedRobots;
    [SerializeField] private GameObject[] secondaryRedCameras;

    [SerializeField] private GameObject cameraBorder;

    private NoteBugHandler noteHandler;

    private ZoneControl zoneCtrl;

    private void Start() 
    {
        noteHandler = FindFirstObjectByType<NoteBugHandler>();

        zoneCtrl = FindFirstObjectByType<ZoneControl>();

        cameraBorder.SetActive(false);

        gamemode = PlayerPrefs.GetInt("gamemode");
        cameraMode = PlayerPrefs.GetInt("cameraMode");
        redRobot = PlayerPrefs.GetInt("redRobotSettings");
        blueRobot = PlayerPrefs.GetInt("blueRobotSettings");

        if (gamemode == 1) 
        { 
            multiplayer = true;
        }
        else if (gamemode == 2) 
        { 
            sameAlliance = true; 
        }
        else 
        {
            sameAlliance = false;
            multiplayer = false;
        }

        HideAll();

        if (multiplayer)
        {
            cameraBorder.SetActive(true);

            blueRobots[blueRobot].GetComponent<PlayerInput>().defaultControlScheme = "Controls 1";
            redRobots[redRobot].GetComponent<PlayerInput>().defaultControlScheme = "Controls 2";
            
            redRobots[redRobot].SetActive(true);
            redCameras[cameraMode+3].SetActive(true);

            if (cameraMode == 0)
            {
                redRobots[redRobot].GetComponent<DriveController>().isFieldCentric = true;
                redRobots[redRobot].GetComponent<DriveController>().startingReversed = true;
            }
            else if (cameraMode == 1)
            {
                redRobots[redRobot].GetComponent<DriveController>().isFieldCentric = true;

                if (redRobots[redRobot].GetComponent<DriveController>().robotType == RobotSettings.StealthRobotics) 
                {
                    redRobots[redRobot].GetComponent<DriveController>().startingReversed = true;
                }
                else 
                {
                    redRobots[redRobot].GetComponent<DriveController>().startingReversed = false;
                }
            }
            else if (cameraMode == 2)
            {
                redRobots[redRobot].GetComponent<DriveController>().isFieldCentric = false;
                redRobots[redRobot].GetComponent<DriveController>().startingReversed = true;
            }

            blueRobots[blueRobot].SetActive(true);
            blueCameras[cameraMode+3].SetActive(true);

            if (cameraMode == 0)
            {
                blueRobots[blueRobot].GetComponent<DriveController>().isFieldCentric = true;
                blueRobots[blueRobot].GetComponent<DriveController>().startingReversed = true;
            }
            else if (cameraMode == 1) 
            {
                blueRobots[blueRobot].GetComponent<DriveController>().isFieldCentric = true;
                if (blueRobots[blueRobot].GetComponent<DriveController>().robotType == RobotSettings.StealthRobotics) 
                {
                    blueRobots[blueRobot].GetComponent<DriveController>().startingReversed = true;
                }
                else 
                {
                    blueRobots[blueRobot].GetComponent<DriveController>().startingReversed = false;
                }
            }
            else if (cameraMode == 2) 
            {
                blueRobots[blueRobot].GetComponent<DriveController>().isFieldCentric = false;
                blueRobots[blueRobot].GetComponent<DriveController>().startingReversed = false;
            }
            noteHandler.GetRobots();
        }
        else if (sameAlliance) 
        {
            cameraBorder.SetActive(true);

            if (PlayerPrefs.GetString("alliance") == "red")
            {
                redRobots[redRobot].GetComponent<PlayerInput>().defaultControlScheme = "Controls 1";
                secondaryRedRobots[blueRobot].GetComponent<PlayerInput>().defaultControlScheme = "Controls 2";
                
                redRobots[redRobot].SetActive(true);
                redCameras[cameraMode+3].SetActive(true);

                secondaryRedRobots[blueRobot].SetActive(true);
                secondaryRedCameras[cameraMode].SetActive(true);

                secondaryRedRobots[blueRobot].GetComponent<RobotNoteManager>().isOtherRobot = true;

                if (cameraMode == 0)
                {
                    redRobots[redRobot].GetComponent<DriveController>().isFieldCentric = true;
                    redRobots[redRobot].GetComponent<DriveController>().startingReversed = !redRobots[redRobot].GetComponent<DriveController>().startingReversed;;

                    secondaryRedRobots[blueRobot].GetComponent<DriveController>().isFieldCentric = true;
                    secondaryRedRobots[blueRobot].GetComponent<DriveController>().startingReversed = !secondaryRedRobots[blueRobot].GetComponent<DriveController>().startingReversed;;
                }
                else if (cameraMode == 1)
                {
                    redRobots[redRobot].GetComponent<DriveController>().isFieldCentric = true;
                    redRobots[redRobot].GetComponent<DriveController>().startingReversed = false;

                    secondaryRedRobots[blueRobot].GetComponent<DriveController>().isFieldCentric = true;
                    secondaryRedRobots[blueRobot].GetComponent<DriveController>().startingReversed = false;

                    if (redRobots[redRobot].GetComponent<DriveController>().robotType == RobotSettings.StealthRobotics) 
                    {
                        redRobots[redRobot].GetComponent<DriveController>().startingReversed = true;
                    }

                    if (secondaryRedRobots[blueRobot].GetComponent<DriveController>().robotType == RobotSettings.StealthRobotics) 
                    {
                        secondaryRedRobots[blueRobot].GetComponent<DriveController>().startingReversed = true;
                    }
                }
                else if (cameraMode == 2)
                {
                    redRobots[redRobot].GetComponent<DriveController>().isFieldCentric = false;
                    redRobots[redRobot].GetComponent<DriveController>().startingReversed = true;

                    secondaryRedRobots[blueRobot].GetComponent<DriveController>().isFieldCentric = false;
                    secondaryRedRobots[blueRobot].GetComponent<DriveController>().startingReversed = true;
                }
            }
            else 
            {
                blueRobots[blueRobot].GetComponent<PlayerInput>().defaultControlScheme = "Controls 1";
                secondaryBlueRobots[redRobot].GetComponent<PlayerInput>().defaultControlScheme = "Controls 2";
                
                blueRobots[blueRobot].SetActive(true);
                blueCameras[cameraMode+3].SetActive(true);

                secondaryBlueRobots[redRobot].SetActive(true);
                secondaryBlueCameras[cameraMode].SetActive(true);

                secondaryBlueRobots[redRobot].GetComponent<RobotNoteManager>().isOtherRobot = true;

                if (cameraMode == 0)
                {
                    blueRobots[blueRobot].GetComponent<DriveController>().isFieldCentric = true;
                    blueRobots[blueRobot].GetComponent<DriveController>().startingReversed = !blueRobots[blueRobot].GetComponent<DriveController>().startingReversed;

                    secondaryBlueRobots[redRobot].GetComponent<DriveController>().isFieldCentric = true;
                    secondaryBlueRobots[redRobot].GetComponent<DriveController>().startingReversed = !secondaryBlueRobots[redRobot].GetComponent<DriveController>().startingReversed;;
                }
                else if (cameraMode == 1)
                {
                    blueRobots[blueRobot].GetComponent<DriveController>().isFieldCentric = true;
                    blueRobots[blueRobot].GetComponent<DriveController>().startingReversed = false;

                    secondaryBlueRobots[redRobot].GetComponent<DriveController>().isFieldCentric = true;
                    secondaryBlueRobots[redRobot].GetComponent<DriveController>().startingReversed = false;

                    if (blueRobots[blueRobot].GetComponent<DriveController>().robotType == RobotSettings.StealthRobotics) 
                    {
                        blueRobots[blueRobot].GetComponent<DriveController>().startingReversed = true;
                    }

                    if (secondaryBlueRobots[redRobot].GetComponent<DriveController>().robotType == RobotSettings.StealthRobotics) 
                    {
                        secondaryBlueRobots[redRobot].GetComponent<DriveController>().startingReversed = true;
                    }
                }
                else if (cameraMode == 2)
                {
                    blueRobots[blueRobot].GetComponent<DriveController>().isFieldCentric = false;
                    blueRobots[blueRobot].GetComponent<DriveController>().startingReversed = true;

                    secondaryBlueRobots[redRobot].GetComponent<DriveController>().isFieldCentric = false;
                    secondaryBlueRobots[redRobot].GetComponent<DriveController>().startingReversed = true;
                }
            }
            noteHandler.GetRobots();
        }
        else 
        {
            //Set correct robots & cameras active
            if (PlayerPrefs.GetString("alliance") == "red")
            {
                redRobots[redRobot].GetComponent<PlayerInput>().defaultControlScheme = "Controls 1";

                redRobots[redRobot].SetActive(true);
                redCameras[cameraMode].SetActive(true);

                if (cameraMode == 0)
                {
                    redRobots[redRobot].GetComponent<DriveController>().startingReversed = !redRobots[redRobot].GetComponent<DriveController>().startingReversed;
                    redRobots[redRobot].GetComponent<DriveController>().isFieldCentric = true;
                }
                else if (cameraMode == 1) 
                {
                    redRobots[redRobot].GetComponent<DriveController>().isFieldCentric = true;
                }
                else if (cameraMode == 2) 
                {
                    redRobots[redRobot].GetComponent<DriveController>().isFieldCentric = false;
                }
            }
            else 
            {
                blueRobots[blueRobot].GetComponent<PlayerInput>().defaultControlScheme = "Controls 1";

                blueRobots[blueRobot].SetActive(true);
                blueCameras[cameraMode].SetActive(true);

                if (cameraMode == 0)
                {
                    blueRobots[blueRobot].GetComponent<DriveController>().startingReversed = !blueRobots[blueRobot].GetComponent<DriveController>().startingReversed;
                    blueRobots[blueRobot].GetComponent<DriveController>().isFieldCentric = true;
                }
                else if (cameraMode == 1) 
                {
                    blueRobots[blueRobot].GetComponent<DriveController>().isFieldCentric = true;
                }
                else if (cameraMode == 2)
                {
                    blueRobots[blueRobot].GetComponent<DriveController>().isFieldCentric = false;
                }
            }
        }

        if (zoneCtrl != null) { zoneCtrl.GetRobots(); }
    }

    private void HideAll() 
    {
        foreach (var robot in blueRobots) 
        {
            robot.SetActive(false);
        }

        foreach (var robot in redRobots) 
        {
            robot.SetActive(false);
        }

        foreach (var camera in blueCameras) 
        {
            camera.SetActive(false);
        }

        foreach (var camera in redCameras) 
        {
            camera.SetActive(false);
        }

        foreach (var robot in secondaryBlueRobots) 
        {
            robot.SetActive(false);
        }

        foreach (var robot in secondaryRedRobots) 
        {
            robot.SetActive(false);
        }

        foreach (var camera in secondaryBlueCameras) 
        {
            camera.SetActive(false);
        }

        foreach (var camera in secondaryRedCameras) 
        {
            camera.SetActive(false);
        }
    }
}
