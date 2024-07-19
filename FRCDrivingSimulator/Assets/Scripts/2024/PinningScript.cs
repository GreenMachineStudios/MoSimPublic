using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class PinningScript : MonoBehaviour
{
    public GameObject redPin;
    public GameObject bluePin;

    public TextMeshProUGUI redText;
    public TextMeshProUGUI blueText;
    public GameObject rtext;
    public GameObject btext;

    public AudioSource player;
    public AudioResource countdown;
    public AudioResource penalty;

    private Coroutine redCoroutine;
    private Coroutine blueCoroutine;

    private bool reachedEndOfGame = false;

    void Update() 
    {
        if (!reachedEndOfGame) 
        {
            if (GameManager.GameState != GameState.End)
            {
                if (!IsTimerRunning(redCoroutine) && DriveController.isPinningBlue && DriveController.isTouchingWallColliderBlue && !DriveController.isTouchingWallColliderRed)
                {
                    StartTimer(DoBlue, ref blueCoroutine);
                }
                else if (!IsTimerRunning(blueCoroutine) && DriveController.isPinningRed && DriveController.isTouchingWallColliderRed && !DriveController.isTouchingWallColliderBlue)
                {
                    StartTimer(DoRed, ref redCoroutine);
                }
                else
                {
                    StopTimers();
                }
            }
            else
            {
                reachedEndOfGame = true;
                StopTimers();
            }
        }
    }

    void StartTimer(System.Func<IEnumerator> timerMethod, ref Coroutine timerCoroutine)
    {
        if (timerCoroutine == null)
        {
            timerCoroutine = StartCoroutine(timerMethod());
        }
    }

    bool IsTimerRunning(Coroutine timerCoroutine)
    {
        return timerCoroutine != null;
    }

    void StopTimers()
    {
        player.Stop();
        if (redCoroutine != null)
        {
            StopCoroutine(redCoroutine);
            redCoroutine = null;
        }

        if (blueCoroutine != null)
        {
            StopCoroutine(blueCoroutine);
            blueCoroutine = null;
        }

        redPin.SetActive(false);
        bluePin.SetActive(false);
        rtext.SetActive(false);
        btext.SetActive(false);
    }

    IEnumerator DoBlue()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (DriveController.isPinningBlue && DriveController.isTouchingWallColliderBlue && !DriveController.isTouchingWallColliderRed)
            {
                rtext.SetActive(false);
                redPin.SetActive(false);
                btext.SetActive(true);
                bluePin.SetActive(true);
                player.resource = countdown;
                player.Play();
                for (int i = 4; i > 0; i--)
                {
                    blueText.text = i.ToString();
                    yield return new WaitForSeconds(1f);
                }
                Score.blueScore += 5;
                player.resource = penalty;
                player.Play();
                if (GameManager.GameState == GameState.Auto)
                {
                    GameScoreTracker.BlueAutoPenaltyPoints += 5;
                }
                else
                {
                    GameScoreTracker.BlueTeleopPenaltyPoints += 5;
                }

                if (bluePin.activeSelf)
                {
                    continue;
                }
            }
            break;
        }
    }

    IEnumerator DoRed()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (DriveController.isPinningRed && DriveController.isTouchingWallColliderRed && !DriveController.isTouchingWallColliderBlue)
            {
                btext.SetActive(false);
                rtext.SetActive(true);
                redPin.SetActive(true);
                bluePin.SetActive(false);
                player.resource = countdown;
                player.Play();
                for (int i = 4; i > 0; i--)
                {
                    redText.text = i.ToString();
                    yield return new WaitForSeconds(1f);
                }
                Score.redScore += 5;
                player.resource = penalty;
                player.Play();
                if (GameManager.GameState == GameState.Auto)
                {
                    GameScoreTracker.RedAutoPenaltyPoints += 5;
                }
                else
                {
                    GameScoreTracker.RedTeleopPenaltyPoints += 5;
                }

                if (redPin.activeSelf)
                {
                    continue;
                }
            }
            break;
        }
    }
}
