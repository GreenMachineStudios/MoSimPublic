using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class Amp : MonoBehaviour
{
    [SerializeField] private Alliance alliance;

    [SerializeField] private GameObject notePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Collider ampCollider;

    [SerializeField] private Material ampFlashMat;
    [SerializeField] private Material ampLevelOneMat;
    [SerializeField] private Material ampLevelTwoMat;

    [SerializeField] private AudioSource source;
    [SerializeField] private AudioResource scoreSound;

    [SerializeField] private bool useAmpAnimation;

    public int noteScoredWorth { get; private set; }
    public int numOfStoredNotes { get; private set; }

    private bool isFlashing = false;
    private bool isNoteProcessed = false;
    private bool doNotChangeNoteWorth = false;

    private const int NUM_OF_FLASHES_WHEN_AMPED = 10;
    private const int AUTO_NOTE_WORTH = 2;
    private const int TELEOP_NOTE_WORTH = 1;

    private void Start()
    {
        ampFlashMat.SetColor("_EmissionColor", Color.black);

        ResetStoredNotes();
        SetNoteWorth(AUTO_NOTE_WORTH);
    }

    private void Update()
    {
        //Change amp note worth to teleop note worth when auto ends
        if (GameManager.GameState == GameState.Teleop && !doNotChangeNoteWorth)
        {
            doNotChangeNoteWorth = true;
            noteScoredWorth = TELEOP_NOTE_WORTH;
        }

        isNoteProcessed = false;

        if (alliance == Alliance.Red && DriveController.isRedAmped || alliance == Alliance.Blue && DriveController.isAmped)
        {
            if (!isFlashing)
            {
                //Flash the amp light when the amp is amplified, and not already flashing
                StartCoroutine(Flash());
            }
        }
        else
        {
            isFlashing = false;
            UpdateAmpLights();
        }
    }

    private void UpdateAmpLights() 
    {
        //Set amp level light colors according to the number of notes stored
        if (numOfStoredNotes > 0 && numOfStoredNotes < 2)
        {
            if (alliance == Alliance.Blue) { ampLevelOneMat.SetColor("_EmissionColor", Color.blue * 10f); }
            else { ampLevelOneMat.SetColor("_EmissionColor", Color.red * 10f); }
        }
        else if (numOfStoredNotes > 1)
        {
            if (alliance == Alliance.Blue) { ampLevelTwoMat.SetColor("_EmissionColor", Color.blue * 10f); }
            else { ampLevelTwoMat.SetColor("_EmissionColor", Color.red * 10f); }
        }
        else if (numOfStoredNotes == 0)
        {
            ampLevelOneMat.SetColor("_EmissionColor", Color.black);
            ampLevelTwoMat.SetColor("_EmissionColor", Color.black);
        }
    }

    private IEnumerator Flash()
    {
        isFlashing = true;
        for (int i = 0; i < NUM_OF_FLASHES_WHEN_AMPED; i++)
        {
            if (!isFlashing)
            {
                break;
            }
            ampFlashMat.SetColor("_EmissionColor", Color.yellow * 10f);
            yield return new WaitForSeconds(0.5f);
            ampFlashMat.SetColor("_EmissionColor", Color.black);
            yield return new WaitForSeconds(0.5f);
        }
        isFlashing = false;
    }

    public void ResetStoredNotes()
    {
        numOfStoredNotes = 0;
    }

    public void SetNoteWorth(int worth)
    {
        noteScoredWorth = worth;
    }

    public void ResetAmp()
    {
        StopAllCoroutines();
        ResetStoredNotes();
        SetNoteWorth(AUTO_NOTE_WORTH);
        ampFlashMat.SetColor("_EmissionColor", Color.black);
        doNotChangeNoteWorth = false;
    }

    private void AddAmpScore()
    {
        //Adds score appropriately for whichever alliance the amp is
        if (alliance == Alliance.Red)
        {
            if (GameManager.GameState == GameState.Auto)
            {
                GameScoreTracker.RedAutoAmpPoints += noteScoredWorth;
            }
            else
            {
                GameScoreTracker.RedTeleopAmpPoints += noteScoredWorth;
            }
            Score.redScore += noteScoredWorth;
        }
        else if (alliance == Alliance.Blue)
        {
            if (GameManager.GameState == GameState.Auto)
            {
                GameScoreTracker.BlueAutoAmpPoints += noteScoredWorth;
            }
            else
            {
                GameScoreTracker.BlueTeleopAmpPoints += noteScoredWorth;
            }
            Score.blueScore += noteScoredWorth;
        }
    }

    //Method to animate notes into the amp, purely visually
    private void AmpNoteAnimation()
    {
        GameObject note = Instantiate(notePrefab, spawnPoint.position, spawnPoint.rotation);
        Rigidbody notePhysics = note.GetComponent<Rigidbody>();
        notePhysics.velocity += Vector3.down * 5f;

        RingBehaviour noteBehavior = note.GetComponent<RingBehaviour>();
        if (alliance == Alliance.Blue) 
        { 
            noteBehavior.DownSideForceLeft(); 
        }
        else 
        { 
            noteBehavior.DownSideForceRight(); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isValidNote = other.gameObject.CompareTag("Ring") && !isNoteProcessed && ampCollider.bounds.Intersects(other.bounds);

        if (isValidNote)
        {
            bool notAmplified = (alliance == Alliance.Red && !DriveController.isRedAmped || alliance == Alliance.Blue && !DriveController.isAmped);

            //Only count notes if the amp isn't amplified
            if (notAmplified)
            {
                numOfStoredNotes++;
            }

            if (useAmpAnimation)
            {
                AmpNoteAnimation();
            }

            if (GameManager.GameState != GameState.End)
            {
                AddAmpScore();
            }

            isNoteProcessed = true;

            source.resource = scoreSound;
            source.Play();
            Destroy(other.gameObject);
        }
    }
}
