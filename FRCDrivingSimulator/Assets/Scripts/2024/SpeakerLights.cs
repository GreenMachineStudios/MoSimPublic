using System.Collections;
using UnityEngine;

public class SpeakerLights : MonoBehaviour
{
    public Material[] lightMats;

    private bool doWholeLightArrayThing = true;
    public bool isRedSpeaker = false;


    private bool stopLights = false;
    
    void Start()
    {
        ResetLights();
    }

    void Update()
    {
        if (isRedSpeaker) 
        {
            if (DriveController.isRedAmped && doWholeLightArrayThing) 
            {
                stopLights = false;
                foreach (var mat in lightMats)
                {
                    mat.SetColor("_EmissionColor", Color.yellow * 5f);
                }
                StartCoroutine(BlackOutLightsOneByOne());
                doWholeLightArrayThing = false;
            }
            else if (!DriveController.isRedAmped) 
            {
                stopLights = true;
            }
        }
        else 
        {
            if (DriveController.isAmped && doWholeLightArrayThing) 
            {
                stopLights = false;
                foreach (var mat in lightMats)
                {
                    mat.SetColor("_EmissionColor", Color.yellow * 5f);
                }
                StartCoroutine(BlackOutLightsOneByOne());
                doWholeLightArrayThing = false;
            }
            else if (!DriveController.isAmped) 
            {
                stopLights = true;
            }
        }
        
    }

    IEnumerator BlackOutLightsOneByOne() 
    {
        foreach (var mat in lightMats)
        {
            if (stopLights) 
            {
                stopLights = false;
                break;
            }

            mat.SetColor("_EmissionColor", Color.black);
            yield return new WaitForSeconds(1f);
        }

        if (!isRedSpeaker)
        {
            foreach (var mat in lightMats) 
            {
                mat.SetColor("_EmissionColor", Color.blue * 10f);
            }
        }
        else 
        {
            foreach (var mat in lightMats) 
            {
                mat.SetColor("_EmissionColor", Color.red * 10f);
            }
        }
        doWholeLightArrayThing = true;
    }

    public void ResetLights() 
    {
        if (!isRedSpeaker)
        {
            foreach (var mat in lightMats) 
            {
                mat.SetColor("_EmissionColor", Color.blue * 10f);
            }
        }
        else 
        {
            foreach (var mat in lightMats) 
            {
                mat.SetColor("_EmissionColor", Color.red * 10f);
            }
        }
    }
}
