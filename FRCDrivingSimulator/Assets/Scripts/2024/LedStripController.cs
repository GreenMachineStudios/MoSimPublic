using System.Collections;
using UnityEngine;

public class LedStripController : MonoBehaviour
{
    private Material mat;
    public GameObject[] leds;

    private int numOfFlashes = 3;

    public static bool redFlash = false;
    public static bool blueFlash = false;

    public bool isFlashing = false;

    public bool isRedRobot;
    public float intensity = 200f;

    [SerializeField] private bool useCustomColors = false;

    [SerializeField] private Color unlitColor;
    [SerializeField] private Color hasNoteColor;
    [SerializeField] private Color noNoteColor;
    [SerializeField] private Color flashColor;

    private RobotNoteManager roboCollisions;

    private void Start() 
    {
        roboCollisions = GetComponent<RobotNoteManager>();

        mat = new Material(Shader.Find("Standard"));
        mat.EnableKeyword("_EMISSION");

        foreach (GameObject led in leds) 
        {
            led.GetComponent<Renderer>().material = mat;
        }

        //Use default color scheme
        if (!useCustomColors) 
        {
            unlitColor = Color.white;
            hasNoteColor = Color.green;
            noNoteColor = Color.red;
            flashColor = Color.yellow;
        }

        redFlash = false;
        blueFlash = false;
        mat.color = unlitColor;
    }
    
    private void Update()
    {
        if (!isFlashing)
        {
            if (roboCollisions.hasRingInRobot)
            {
                mat.SetColor("_EmissionColor", hasNoteColor * intensity);
            }
            else
            {
                mat.SetColor("_EmissionColor", noNoteColor * intensity);
            }
        }
    }

    public void Flash() 
    {
        StartCoroutine(FlashSequence());
    }

    private IEnumerator FlashSequence()
    {
        isFlashing = true;
        for (int i = 0; i < numOfFlashes; i++)
        {
            mat.SetColor("_EmissionColor", flashColor * 0f);
            yield return new WaitForSeconds(0.12f);
            mat.SetColor("_EmissionColor", flashColor * intensity);
            yield return new WaitForSeconds(0.12f);
        }
        isFlashing = false;
    }
}
