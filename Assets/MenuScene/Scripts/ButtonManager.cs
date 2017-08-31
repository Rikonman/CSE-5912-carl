using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public void btnNewGame2D_OnClick()
    {
        SceneManager.LoadScene("pong");
    }

    public void btnNewGame3D_OnClick()
    {
        SceneManager.LoadScene("Pong3D");
    }

    public void btnExitGame_OnClick()
    {
        Application.Quit();
    }
}
