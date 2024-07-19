using UnityEngine;

//This script is for detecting collisions on rotated colliders, like the source
public class PenaltyCollisions : MonoBehaviour
{
    [SerializeField] private Alliance alliance;

    public DriveController playerInside;
    public DriveController enemyInside;
    public bool inside;
    private bool checkCols;

    void Start() 
    {
        inside = false;
        checkCols = PlayerPrefs.GetInt("gamemode") == 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (checkCols) 
        {
            if (alliance == Alliance.Blue)
            {
                //If the blue player is in the blue zone
                if (other.CompareTag("Player")) 
                {
                    inside = true;
                    enemyInside = other.GetComponent<DriveController>();
                    playerInside = GameObject.FindWithTag("RedPlayer").GetComponent<DriveController>();
                }
                //If the opponent (red-player) is in the blue zone
                else if (other.CompareTag("RedPlayer")) 
                {
                    inside = true;
                    enemyInside = GameObject.FindWithTag("Player").GetComponent<DriveController>();
                    playerInside = other.GetComponent<DriveController>();
                }
            }
            else 
            {
                //If the red player is in the red zone
                if (other.CompareTag("RedPlayer")) 
                {
                    inside = true;
                    enemyInside = other.GetComponent<DriveController>();
                    playerInside = GameObject.FindWithTag("Player").GetComponent<DriveController>();
                }  
                //If the opponent (blue-player) is in the red zone
                else if (other.CompareTag("Player")) 
                {
                    inside = true;
                    enemyInside = GameObject.FindWithTag("RedPlayer").GetComponent<DriveController>();
                    playerInside = other.GetComponent<DriveController>();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (checkCols) 
        {
            if (alliance == Alliance.Blue)
            {
                if (other.CompareTag("Player"))
                {
                    inside = false;
                    enemyInside = null;
                    playerInside = null;
                }  
                else if (other.CompareTag("RedPlayer"))
                {
                    inside = false;
                    playerInside = null;
                    enemyInside = null;
                }
            }
            else 
            {
                if (other.CompareTag("RedPlayer")) 
                {
                    inside = false;
                    enemyInside = null;
                    playerInside = null;
                }  
                else if (other.CompareTag("Player")) 
                {
                    inside = false;
                    playerInside = null;
                    enemyInside = null;
                }
            }
        }
    }
}
