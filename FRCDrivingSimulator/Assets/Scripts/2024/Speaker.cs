using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class Speaker : MonoBehaviour
{
    [SerializeField] private Alliance alliance;

    [SerializeField] private Collider speakerCollider;

    [SerializeField] private AudioSource source;
    [SerializeField] private AudioResource scoreSound;
    [SerializeField] private AudioResource ampedScoreSound;

    [field: SerializeField] public int noteScoredWorth { get; set; }
    [field: SerializeField] public int numOfStoredNotes { get; set; }

    [SerializeField] private bool currentlyAmped = false;

    private bool alreadyProcessed = false;

    public DriveController[] drives;

    private const float NOTE_SCORE_REGISTER_DELAY = 1.5f;

    void Start() 
    {
        //Set starting values for instance variables
        numOfStoredNotes = 0;
        noteScoredWorth = 5;
    }

    void Update() 
    {
        if (DriveController.isAmped && alliance == Alliance.Blue || DriveController.isRedAmped && alliance == Alliance.Red)
        {
            currentlyAmped = true;
        }
        else 
        {
            currentlyAmped = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ring") && !alreadyProcessed || other.gameObject.CompareTag("noteShotByRed") && !alreadyProcessed || other.gameObject.CompareTag("noteShotByBlue") && !alreadyProcessed || other.gameObject.CompareTag("noteShotByBlue2") && !alreadyProcessed || other.gameObject.CompareTag("noteShotByRed2") && !alreadyProcessed)
        {
            if (speakerCollider.bounds.Intersects(other.bounds))
            {
                if (DriveController.isAmped && alliance == Alliance.Blue || DriveController.isRedAmped && alliance == Alliance.Red) 
                {
                    source.resource = ampedScoreSound;
                    StartCoroutine(ScoreDelay(true));
                }
                else 
                {
                    source.resource = scoreSound;
                    StartCoroutine(ScoreDelay(false));
                }
                Destroy(other.gameObject);
                alreadyProcessed = true;
            }
        }
        alreadyProcessed = false;
    }

    private IEnumerator ScoreDelay(bool wasAmped)
    {
        bool isLegalScore = false;
        if (GameManager.GameState != GameState.End) { isLegalScore = true; }

        yield return new WaitForSeconds(NOTE_SCORE_REGISTER_DELAY);

        if (wasAmped || currentlyAmped)
        {
            source.resource = ampedScoreSound;
            noteScoredWorth = 5;
        }
        else if (GameManager.GameState == GameState.Auto)
        {
            source.resource = scoreSound;
            noteScoredWorth = 5;
        }
        else 
        {
            source.resource = scoreSound;
            noteScoredWorth = 2;
        }
        
        source.Play();

        if (isLegalScore) 
        {
            if (GameManager.GameState == GameState.Auto)
            {
                if (alliance == Alliance.Red) 
                {
                    GameScoreTracker.RedAutoSpeakerPoints += noteScoredWorth;
                    Score.redScore += noteScoredWorth;
                }
                else 
                {
                    GameScoreTracker.BlueAutoSpeakerPoints += noteScoredWorth;
                    Score.blueScore += noteScoredWorth;
                }
            }
            else
            {
                if (alliance == Alliance.Red) 
                {
                    GameScoreTracker.RedTeleopSpeakerPoints += noteScoredWorth;
                    Score.redScore += noteScoredWorth;
                }
                else 
                {
                    GameScoreTracker.BlueTeleopSpeakerPoints += noteScoredWorth;
                    Score.blueScore += noteScoredWorth;
                }
            }
        }

        if (currentlyAmped)
        {
            numOfStoredNotes++;
            if (numOfStoredNotes >= 4)
            {
                foreach (DriveController drive in drives)
                {
                    if (drive.isActiveAndEnabled) 
                    {
                        drive.StopAmplifiedSpeaker();
                        break;
                    }
                }
                ResetNotes();
            }
        }
    }

    public void ResetNotes() 
    {
        numOfStoredNotes = 0;
        noteScoredWorth = 2;
    }

    public void ResetSpeaker() 
    {
        numOfStoredNotes = 0;
        noteScoredWorth = 5;
        StopAllCoroutines();
    }
}