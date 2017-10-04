using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class HostConsole : MonoBehaviour
{
    bool open;
    bool toClose;
    GameObject panel;
    InputField input;

    public string[] tokens;

    ResourceBank bank1;
    ResourceBank bank2;

    GunController playerGun;
    Target playerTarget;
    GameObject player;

    void Start()
    {
        panel = GameObject.Find("Console");
        input = panel.GetComponentInChildren<InputField>();
        panel.SetActive(open);       

        player = GameObject.Find("Player(Clone)");
        playerTarget = player.GetComponent<Target>();
        playerGun = player.GetComponent<GunController>();
    }

    void Update()
    {
        if(bank1 == null)
        {
            bank1 = GameObject.Find("Base1Center").GetComponent<ResourceBank>();
            bank2 = GameObject.Find("Base2Center").GetComponent<ResourceBank>();
        }
        //Open close console
        if (toClose || Input.GetKeyDown(KeyCode.BackQuote))
        {
            toClose = false;
            open = !open;
            panel.SetActive(open);
            if (open)
            {
                player.GetComponent<PlayerController>().enabled = false;
                player.GetComponent<BuildScript>().enabled = false;
                input.ActivateInputField();
            }
            else
            {
                player.GetComponent<PlayerController>().enabled = true;
                player.GetComponent<BuildScript>().enabled = true;
                input.text = "";
            }
        }
        if (open)
        {
            
            if (Input.GetKeyDown(KeyCode.Return))
            {
                tokens = input.text.Split(' ');
                ResourceBank temp;
                switch (tokens[0])
                {
                    case "add":
                         temp = bank1;
                        if (tokens[2].Equals("1"))
                        {
                            temp = bank1;
                        }
                        else if(tokens[2].Equals("2"))
                        {
                            temp = bank2;
                        }
                        if (tokens[1].Equals("stone"))
                        {
                            temp.AddStone(Convert.ToInt32(tokens[3]));
                            toClose = true;
                            return;
                        }
                        else if (tokens[1].Equals("wood"))
                        {
                            temp.AddWood(Convert.ToInt32(tokens[3]));
                            toClose = true;
                            return;
                        }
                        input.text = "Usage : add <type> <team> <amount>";
                        break;
                    case "set":
                        temp = bank1;
                        if (tokens[2].Equals("1"))
                        {
                            temp = bank1;
                        }
                        else if (tokens[2].Equals("2"))
                        {
                            temp = bank2;
                        }
                        if (tokens[1].Equals("stone"))
                        {
                            temp.SetStone(Convert.ToInt32(tokens[3]));
                            toClose = true;
                            break;
                        }
                        else if (tokens[1].Equals("wood"))
                        {
                            temp.SetWood(Convert.ToInt32(tokens[3]));
                            toClose = true;
                            break;
                        }
                        input.text = "Usage : set <type> <team> <amount>";
                        break;

                    case "setAmmo":
                        playerGun.currentAmmoInReserve = Convert.ToInt32(tokens[1]);
                        toClose = true;
                        break;
                    case "setHealth":
                        playerTarget.currentHealth = Convert.ToInt32(tokens[1])%100;
                        toClose = true;
                        break;
                    case "kill":
                        playerTarget.Die();
                        toClose = true;
                        break;
                    default:
                        break;
                }
            }
        }



    }
}
