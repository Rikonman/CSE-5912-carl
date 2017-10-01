using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class HostConsole : MonoBehaviour
{
    bool open;
    GameObject panel;
    InputField input;
    void Start()
    {
        panel = GameObject.Find("Console");
        input = panel.GetComponentInChildren<InputField>();
        panel.SetActive(open);
    }

    void Update()
    {
        //Open close console
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            open = !open;
            panel.SetActive(open);
            if (open)
            {
                input.ActivateInputField();
            }
            else
            {
                input.text = "";
            }
        }
        if (open)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                switch (input.text)
                {
                    case "":
                        //Do something
                        break;
                    default:
                        break;
                }
            }
        }



    }
}
