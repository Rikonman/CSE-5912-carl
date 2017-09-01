using System;
using UnityEngine;
using UnityEngine.UI;

public class NewGameCountdown : MonoBehaviour {

    public Text countdownText;
    public int initialWaitTime = 5;

    private DateTime timeNewGameStarted;

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
            countdownText.text = timeLeft.ToString();
        }
        if (timeLeft == 0)
        {
            PauseMenu.IsPaused = false;
            PauseMenu.IsGameReady = true;
            countdownText.text = "GO!";
        }
        else if (timeLeft == -1)
        {
            gameObject.SetActive(false);
        }
	}

    void StartCountdown()
    {
        timeNewGameStarted = DateTime.Now;
    }

}
