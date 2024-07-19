using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private void Start()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("volume");
    }
}
