using UnityEngine;

public class RollerAnimations : MonoBehaviour
{
    [SerializeField] private GameObject[] rollers;
    [SerializeField] private float speed;
    private DriveController controller;

    private void Start() 
    {
        controller = GetComponent<DriveController>();
    }

    private void Update()
    {
        if (controller.isIntaking) 
        {
            foreach (GameObject roller in rollers)
            {
                roller.transform.Rotate(Vector3.right, speed * Time.deltaTime);
            }
        }
    }
}
