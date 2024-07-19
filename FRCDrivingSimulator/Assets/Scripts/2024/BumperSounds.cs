using UnityEngine;
using UnityEngine.Audio;

public class BumperSounds : MonoBehaviour
{
    [SerializeField] private Alliance alliance;

    [SerializeField] private AudioSource player;
    [SerializeField] private AudioResource[] hitSounds;

    private DriveController controller;

    private bool triggerSound;
    private bool useSounds;

    private void Start()
    {
        useSounds = PlayerPrefs.GetInt("bumpSounds") == 1;
        controller = GetComponent<DriveController>();
    }

    private void Update() 
    {
        if (useSounds) 
        {
            bool touchingWall = (alliance == Alliance.Red && DriveController.isTouchingWallColliderRed) || (alliance == Alliance.Blue && DriveController.isTouchingWallColliderBlue);
        
            if (touchingWall && !player.isPlaying && triggerSound)
            {
                player.volume = controller.beforeVelocity * 0.02f;
                player.resource = hitSounds[Random.Range(0, hitSounds.Length)];
                player.Play();
            }

            if (!touchingWall)
            {
                triggerSound = true;
            }
            else 
            {
                triggerSound = false;
            }
        }
    }
} 
