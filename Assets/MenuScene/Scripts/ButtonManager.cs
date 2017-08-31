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

    public void btnExitGame_OnClick()
    {
        Application.Quit();
    }
}
