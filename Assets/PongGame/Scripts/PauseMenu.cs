using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {
    
    public static bool isPaused;
    GameObject pausePanel;

	void Start () {
        KeyboardInput.Paused += OpenPauseMenu;
        pausePanel = transform.GetChild(0).gameObject;
        pausePanel.SetActive(true);
        isPaused = true;

    }
	
    public void MainMenu()
    {
        //Add main menu load
        //Application.LoadLevel(1);
    }

    public void ExitGame()
    {
        Application.Quit();
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
