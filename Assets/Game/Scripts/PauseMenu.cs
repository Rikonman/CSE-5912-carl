using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    GameObject pausePanel;
    public delegate void Exit();
    public static event Exit ExitGame;

    public static bool isPaused;

    void OnEnable()
    {
        KeyboardInput.Paused += OpenPauseMenu;
    }

    void Start()
    {
        pausePanel = transform.GetChild(0).gameObject;
        pausePanel.SetActive(true);
        isPaused = true;
    }

    public void MainMenu()
    {
        //Add main menu load
        ExitGame();
        KeyboardInput.Paused -= OpenPauseMenu;
        SceneManager.LoadScene("MenuScene");
    }

    public void ResumeGame()
    {
        OpenPauseMenu();
    }

    public void Save_OnClick()
    {
        SaveController.saveController.Save();
    }

    void OpenPauseMenu()
    {
        if (pausePanel == null)
        {
            pausePanel = transform.GetChild(0).gameObject;
        }
        if (!isPaused)
        {
            pausePanel.SetActive(true);
        }
        else
        {
            pausePanel.SetActive(false);

        }
        isPaused = !isPaused;
    }
}
