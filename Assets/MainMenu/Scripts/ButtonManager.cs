using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public void btnNewGame_OnClick()
    {
        SaveController.newGame = true;
        SceneManager.LoadScene("LoadingScene");
    }

    public void Load_OnClick()
    {
        SaveController.newGame = false;
        SceneManager.LoadScene("LoadingScene");
    }

    public void btnExitGame_OnClick()
    {
        Application.Quit();
    }
}
