using UnityEngine;

public class TrapScore : MonoBehaviour, IResettable
{
    [SerializeField] private Alliance alliance;
    private bool trappedNote;
    private bool changeScore;

    private const int TRAP_NOTE_WORTH = 5;

    private void Update() 
    {
        if (GameManager.GameState == GameState.Endgame) 
        {
            if (trappedNote && changeScore) 
            {
                AddScore();
                changeScore = false;
            }
            else if (!trappedNote && changeScore) 
            {
                SubScore();
                changeScore = false;
            }
        }
    }

    private void AddScore() 
    {
        if (alliance == Alliance.Blue) 
        {
            Score.AddScore(TRAP_NOTE_WORTH, Alliance.Blue);
            GameScoreTracker.BlueTrapPoints += TRAP_NOTE_WORTH;
        }
        else 
        {
            Score.AddScore(TRAP_NOTE_WORTH, Alliance.Red);
            GameScoreTracker.RedTrapPoints += TRAP_NOTE_WORTH;
        }
    }

    private void SubScore() 
    {
        if (alliance == Alliance.Blue) 
        {
            Score.SubScore(TRAP_NOTE_WORTH, Alliance.Blue);
            GameScoreTracker.BlueTrapPoints -= TRAP_NOTE_WORTH;
        }
        else 
        {
            Score.SubScore(TRAP_NOTE_WORTH, Alliance.Red);
            GameScoreTracker.RedTrapPoints -= TRAP_NOTE_WORTH;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Ring")) 
        {
            trappedNote = true;
            changeScore = true;
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.CompareTag("Ring")) 
        {
            trappedNote = false;
            changeScore = true;
        }
    }

    public void Reset() 
    {
        trappedNote = false;
        changeScore = false;
    }
}
