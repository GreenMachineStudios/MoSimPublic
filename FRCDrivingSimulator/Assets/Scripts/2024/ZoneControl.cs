using System;
using UnityEngine;

public class ZoneControl : MonoBehaviour
{
    public BoxCollider blueZone;
    public BoxCollider redZone;
    
    public GameObject[] blueRobots;
    public GameObject[] redRobots;

    private GameObject blueRobot;
    private GameObject otherBlueRobot;

    private GameObject redRobot;
    private GameObject otherRedRobot;

    public static bool blueRobotInRedZone;
    public static bool blueRobotInRedZoneUpdated;

    public static bool blueOtherRobotInRedZone;
    public static bool blueOtherRobotInRedZoneUpdated;

    public static bool redRobotInBlueZone;
    public static bool redRobotInBlueZoneUpdated;

    public static bool redOtherRobotInBlueZone;
    public static bool redOtherRobotInBlueZoneUpdated;

    private bool gotRobots = false;
    private bool gotFirstRobot = false;

    public void GetRobots()
    {
        blueRobotInRedZone = false;
        blueOtherRobotInRedZone = false;
        redRobotInBlueZone = false;
        redOtherRobotInBlueZone = false;

        foreach (GameObject robot in redRobots) 
        {
            if (robot.activeSelf) 
            {
                if (!gotFirstRobot) 
                {
                    gotFirstRobot = true;
                    redRobot = robot;
                }
                else
                {
                    otherRedRobot = robot;
                }                
            }
        }

        gotFirstRobot = false;

        foreach (GameObject robot in blueRobots) 
        {
            if (robot.activeSelf) 
            {
                if (!gotFirstRobot) 
                {
                    gotFirstRobot = true;
                    blueRobot = robot;
                }
                else
                {
                    otherBlueRobot = robot;
                }                
            }
        }

        if (blueRobot != null || redRobot != null) { gotRobots = true; }
        else { throw new Exception("ZoneControl No Robots Found"); }
    }

    private void Update() 
    {
        if (gotRobots) 
        {
            redRobotInBlueZoneUpdated = false;
            redOtherRobotInBlueZoneUpdated = false;

            blueRobotInRedZoneUpdated = false;
            blueOtherRobotInRedZoneUpdated = false;

            if (redRobot != null) 
            {
                if (blueZone.bounds.Intersects(redRobot.GetComponent<Collider>().bounds))
                {
                    redRobotInBlueZoneUpdated = true;
                }

                if (otherRedRobot != null && blueZone.bounds.Intersects(otherRedRobot.GetComponent<Collider>().bounds)) 
                {
                    redOtherRobotInBlueZoneUpdated = true;
                }
            }
            else if (blueRobot != null) 
            {
                if (redZone.bounds.Intersects(blueRobot.GetComponent<Collider>().bounds))
                {
                    blueRobotInRedZoneUpdated = true;
                }

                if (otherBlueRobot != null && redZone.bounds.Intersects(otherBlueRobot.GetComponent<Collider>().bounds)) 
                {
                    blueOtherRobotInRedZoneUpdated = true;
                }
            }   
        }
    }
    
    public void CheckBlueZoneCollisions()
    {
        blueRobotInRedZone = false;
        blueOtherRobotInRedZone = false;

        if (redZone.bounds.Intersects(blueRobot.GetComponent<Collider>().bounds))
        {
            blueRobotInRedZone = true;
        }
        else if (otherBlueRobot != null && redZone.bounds.Intersects(otherBlueRobot.GetComponent<Collider>().bounds)) 
        {
            blueOtherRobotInRedZone = true;
        }
    }

    public void CheckRedZoneCollisions()
    {
        redRobotInBlueZone = false;
        redOtherRobotInBlueZone = false;

        if (blueZone.bounds.Intersects(redRobot.GetComponent<Collider>().bounds))
        {
            redRobotInBlueZone = true;
        }
        else if (otherRedRobot != null && blueZone.bounds.Intersects(otherRedRobot.GetComponent<Collider>().bounds)) 
        {
            redOtherRobotInBlueZone = true;
        }
    }
}

