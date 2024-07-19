using UnityEngine;

public class NoteBugHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject[] blueRobots;
    [SerializeField]
    private GameObject[] otherBlueRobots;
    [SerializeField]
    private GameObject[] redRobots;
    [SerializeField]
    private GameObject[] otherRedRobots;

    private RobotNoteManager blueRing;
    private RobotNoteManager redRing;
    private DriveController blueDrive;
    private DriveController redDrive;

    private RobotNoteManager otherBlueRing;
    private RobotNoteManager otherRedRing;
    private DriveController otherBlueDrive;
    private DriveController otherRedDrive;

    private bool robotsGot = false;
    private bool sameAlliance = false;
    private bool isBlueAlliance;

    public void GetRobots()
    {
        isBlueAlliance = PlayerPrefs.GetString("alliance") == "blue";

        if (RobotSpawnController.sameAlliance) { sameAlliance = true; }
        else { sameAlliance = false; }

        if (!sameAlliance)
        {
            foreach (var robot in blueRobots) 
            {
                if (robot.activeSelf)
                {
                    blueRing = robot.GetComponent<RobotNoteManager>();
                    blueDrive = robot.GetComponent<DriveController>();
                    break;
                }
            }

            foreach (var robot in redRobots) 
            {
                if (robot.activeSelf) 
                {
                    redRing = robot.GetComponent<RobotNoteManager>();
                    redDrive = robot.GetComponent<DriveController>();
                    break;
                }
            }
        }
        else 
        {
            if (isBlueAlliance)
            {
                foreach (var robot in blueRobots)
                {
                    if (robot.activeSelf) 
                    {
                        blueRing = robot.GetComponent<RobotNoteManager>();
                        blueDrive = robot.GetComponent<DriveController>();
                        break;
                    }
                }

                foreach (var robot in otherBlueRobots)
                {
                    if (robot.activeSelf) 
                    {
                        otherBlueRing = robot.GetComponent<RobotNoteManager>();
                        otherBlueDrive = robot.GetComponent<DriveController>();
                        break;
                    }
                }
            }
            else 
            {
                foreach (var robot in redRobots) 
                {
                    if (robot.activeSelf) 
                    {
                        redRing = robot.GetComponent<RobotNoteManager>();
                        redDrive = robot.GetComponent<DriveController>();
                        break;
                    }
                }

                foreach (var robot in otherRedRobots) 
                {
                    if (robot.activeSelf) 
                    {
                        otherRedRing = robot.GetComponent<RobotNoteManager>();
                        otherRedDrive = robot.GetComponent<DriveController>();
                        break;
                    }
                }
            }
        }
        
        robotsGot = true;
    }

    private void Update()
    {
        if (robotsGot)
        {
            if (!sameAlliance) 
            {
                if (!blueRing.hasRingInRobot && blueRing.ringWithinIntakeCollider && blueDrive.intakeValue > 0 && !redRing.hasRingInRobot && redRing.ringWithinIntakeCollider && redDrive.intakeValue > 0) 
                {
                    //Randomly choose between blue and red robot
                    bool giveToBlue = Random.value < 0.5f; //50% chance for each robot

                    if (giveToBlue)
                    {
                        blueRing.ringWithinIntakeCollider = true;
                        redRing.ringWithinIntakeCollider = false;
                    }
                    else
                    {
                        redRing.ringWithinIntakeCollider = true;
                        blueRing.ringWithinIntakeCollider = false;
                    }
                }
            }
            else 
            {
                if (isBlueAlliance) 
                {
                    if (!blueRing.hasRingInRobot && blueRing.ringWithinIntakeCollider && blueDrive.intakeValue > 0 && !otherBlueRing.hasRingInRobot && otherBlueRing.ringWithinIntakeCollider && otherBlueDrive.intakeValue > 0) 
                    {
                        //Randomly choose between blue and other blue robot
                        bool giveToBlue = Random.value < 0.5f; //50% chance for each robot

                        if (giveToBlue)
                        {
                            blueRing.ringWithinIntakeCollider = true;
                            otherBlueRing.ringWithinIntakeCollider = false;
                        }
                        else
                        {
                            otherBlueRing.ringWithinIntakeCollider = true;
                            blueRing.ringWithinIntakeCollider = false;
                        }
                    }
                }
                else 
                {
                    if (!redRing.hasRingInRobot && redRing.ringWithinIntakeCollider && redDrive.intakeValue > 0 && !otherRedRing.hasRingInRobot && otherRedRing.ringWithinIntakeCollider && otherRedDrive.intakeValue > 0) 
                    {
                        //Randomly choose between red and other red robot
                        bool giveToRed = Random.value < 0.5f; //50% chance for each robot

                        if (giveToRed)
                        {
                            redRing.ringWithinIntakeCollider = true;
                            otherRedRing.ringWithinIntakeCollider = false;
                        }
                        else
                        {
                            otherRedRing.ringWithinIntakeCollider = true;
                            redRing.ringWithinIntakeCollider = false;
                        }
                    }
                }
            }
        }
    }
}
