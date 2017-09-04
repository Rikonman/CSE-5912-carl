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

    public void btnExitGameQuery_OnClick()
    {
        GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.GetChild(2).gameObject.SetActive(true);
    }

    public void CancelQuery_OnClick()
    {
        GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.GetChild(2).gameObject.SetActive(false);
    }

    public void btnExitGame_OnClick()
    {
        Application.Quit();
    }
}
