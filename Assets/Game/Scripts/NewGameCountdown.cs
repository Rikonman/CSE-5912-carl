using System;
using UnityEngine;
using UnityEngine.UI;

public class NewGameCountdown : MonoBehaviour {

    public Text countdownText;
    public int initialWaitTime = 5;
    public AudioSource audioSource;
    public AudioClip countClip;
    public AudioClip beginClip;

    private DateTime timeNewGameStarted;
    private bool isCountdownActive = true;

    void Start()
    {
        countdownText.text = "";
        timeNewGameStarted = DateTime.MinValue;
        MainCamera.gameStartAnimCompleted += StartCountdown;
        PauseMenu.ExitGame += ExitingGame;
    }

    void ExitingGame()
    {
        MainCamera.gameStartAnimCompleted -= StartCountdown;
    }

    // Update is called once per frame
    void Update () {
        if (timeNewGameStarted == DateTime.MinValue)
            return;

        int timeElapsed = (int)DateTime.Now.Subtract(timeNewGameStarted).TotalSeconds;
        int timeLeft = initialWaitTime - timeElapsed;
        if (timeLeft > 0 && timeLeft.ToString() != countdownText.text)
        {
            audioSource.PlayOneShot(countClip);
            countdownText.text = timeLeft.ToString();
        }
        if (timeLeft == 0 && isCountdownActive)
        {
            isCountdownActive = false;
            PauseMenu.IsPaused = false;
            PauseMenu.IsGameReady = true;
            audioSource.PlayOneShot(beginClip);
            countdownText.text = "GO!";
        }
        else if (timeLeft == -1)
        {
            gameObject.SetActive(false);
        }
	}

    void StartCountdown()
    {
        initialWaitTime = 5;
        timeNewGameStarted = DateTime.Now;
    }

    public void DoShortCountdown(int time)
    {
        initialWaitTime = time;
        countdownText.text = time.ToString();
        timeNewGameStarted = DateTime.Now;
        isCountdownActive = true;
        PauseMenu.IsGameReady = false;
        gameObject.SetActive(true);
    }

}
