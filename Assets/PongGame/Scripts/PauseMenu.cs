using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    
    public static bool isPaused;
    public GameObject pausePanel;

	void Start () {
        KeyboardInput.Paused += OpenPauseMenu;
        pausePanel.SetActive(true);
        isPaused = true;

    }
	
    public void MainMenu()
    {
        //Add main menu load
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
