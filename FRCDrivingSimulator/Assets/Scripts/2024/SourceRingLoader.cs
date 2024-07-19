using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SourceRingLoader : MonoBehaviour, IResettable
{
    [SerializeField] private Alliance alliance;

    [SerializeField] private float noteSpawnDelay;
    [SerializeField] private TMP_Text counterText;
    [SerializeField] private BoxCollider sourceCollider;
    private int numberOfNotesInSource;

    private SourceMode mode;

    public Transform spawnPointCenter;
    public Transform spawnPointLeft;
    public Transform spawnPointRight;

    public GameObject notePrefab;

    private bool canSpawnRing = true;
    private float drop;

    private int lastRandomNum = -1;
    private int noteCount = 45;

    private void Start() 
    {
        mode = GetSourceMode();
    }

    private void Update() 
    {
        ResetNotes();
        CheckNumOfNotesInSource();

        if (canSpawnRing && drop > 0f && noteCount > 0 && numberOfNotesInSource < 2 && GameManager.GameState != GameState.Auto)
        {
            canSpawnRing = false;

            int randomNum = GetRandom();
            lastRandomNum = randomNum;

            Transform spawnPoint;

            if (mode == SourceMode.Random) 
            {
                if (randomNum == 2)
                {
                    spawnPoint = spawnPointLeft;
                }
                else if (randomNum == 3)
                {
                    spawnPoint = spawnPointRight;
                }
                else 
                {
                    spawnPoint = spawnPointCenter;
                }
            }
            else if (mode == SourceMode.Left) 
            {
                spawnPoint = spawnPointLeft;
            }
            else if (mode == SourceMode.Center) 
            {
                spawnPoint = spawnPointCenter;
            }
            else 
            {
                spawnPoint = spawnPointRight;
            }
            
            SpawnNote(spawnPoint);
        }
    }

    private void SpawnNote(Transform spawnPoint) 
    {
        noteCount--;
        counterText.text = noteCount.ToString();
        Instantiate(notePrefab, spawnPoint.position, spawnPoint.rotation);
        StartCoroutine(NoteSpawnPause());
    }

    private int GetRandom() 
    {
        int randomNum;
        do
        {
            randomNum = Random.Range(1, 4);

        } while (randomNum == lastRandomNum);

        return randomNum;
    }

    private void CheckNumOfNotesInSource() 
    {
        Collider[] colliders = Physics.OverlapBox(sourceCollider.bounds.center, sourceCollider.bounds.extents, Quaternion.identity);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Ring") || collider.CompareTag("noteShotByRed") || collider.CompareTag("noteShotByBlue") || collider.CompareTag("noteShotByBlue2") || collider.CompareTag("noteShotByRed2"))
            {
                numberOfNotesInSource++;
            }
        }
    }

    private void ResetNotes() 
    {
        numberOfNotesInSource = 0;
    }

    private SourceMode GetSourceMode()
    {
        int mode;
        if (alliance == Alliance.Blue) 
        {
            mode =  PlayerPrefs.GetInt("blueSource");
        }
        else 
        {
            mode = PlayerPrefs.GetInt("redSource");
        }
        
        switch (mode) 
        {
            case 0:
                return SourceMode.Random;
            case 1:
                return SourceMode.Left;
            case 2: 
                return SourceMode.Center;
            default:
                return SourceMode.Right;
        }
    }

    public void Reset() 
    {
        noteCount = 45;
        counterText.text = noteCount.ToString();
    }

    private IEnumerator NoteSpawnPause()
    {
        yield return new WaitForSeconds(noteSpawnDelay);
        canSpawnRing = true;
    }

    public void OnSourceDrop(InputAction.CallbackContext ctx) 
    {
        drop = ctx.ReadValue<float>();
    }
}
