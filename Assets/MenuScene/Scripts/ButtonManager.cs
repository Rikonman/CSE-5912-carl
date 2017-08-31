using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public void btnNewGame_OnClick()
    {
        SceneManager.LoadScene("pong");
    }

    public void Load_OnClick()
    {
        SceneManager.LoadScene("pong");
        SaveController.saveController.Load();
    }

    public void btnExitGame_OnClick()
    {
        Application.Quit();
    }
}
