using UnityEngine;

public class ShotBlocker : MonoBehaviour
{
    [SerializeField] private Alliance alliance;
    [SerializeField] private GameObject shotBlocker;

    private void Start() 
    {
        if (alliance == Alliance.Blue && PlayerPrefs.GetInt("blueShotBlocker") == 1) 
        {
            shotBlocker.SetActive(true);
        }
        else if (alliance == Alliance.Red && PlayerPrefs.GetInt("redShotBlocker") == 1)
        {
            shotBlocker.SetActive(true);
        }
        else 
        {
            shotBlocker.SetActive(false);
        }
    }
}
