using System.Collections;
using UnityEngine;

public class RSLLight : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private float maxIntensity;
    [SerializeField] private float minIntensity;

    [SerializeField] private float flashDelay;
    [SerializeField] private Color color;

    private void Start() 
    {
        StartCoroutine(RSLLightFlash());
    }

    private IEnumerator RSLLightFlash() 
    {
        while (true) 
        {
            while (GameManager.isDisabled)
            {
                material.SetColor("_EmissionColor", color * maxIntensity);
                yield return null;
            }

            material.SetColor("_EmissionColor", color * maxIntensity);
            yield return new WaitForSeconds(flashDelay);
            material.SetColor("_EmissionColor", color * minIntensity);
            yield return new WaitForSeconds(flashDelay);
        }
    }
}
