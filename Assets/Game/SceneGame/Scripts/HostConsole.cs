using UnityEngine;
using UnityEngine.UI;
using System;

public class HostConsole : MonoBehaviour
{
    bool open;
    bool toClose;
    public GameObject panel;
    private Image UIPanelImg;
    public InputField input;

    public string[] tokens;

    public ResourceBank bank1;
    public ResourceBank bank2;

    public GunController playerGun;
    public Target playerTarget;
    public GameObject player;

    void Start()
    {
        UIPanelImg = GameObject.Find("UINotificationCenter").GetComponent<Image>();
        panel = GameObject.Find("UINotificationConsole");
        input = panel.GetComponentInChildren<InputField>();
        panel.SetActive(open);
        SetNotificationCenterBackVisible(open);
    }

    void Update()
    {
        if (bank1 == null)
        {
            GameObject b1c = GameObject.Find("Base1Center");
            if (b1c != null)
                bank1 = b1c.GetComponent<ResourceBank>();
        }
        if (bank2 == null)
        {
            GameObject b2c = GameObject.Find("Base2Center");
            if (b2c != null)
                bank1 = b2c.GetComponent<ResourceBank>();
        }
        if (player == null)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject tempPlayer in players)
            {
                PlayerController tempController = tempPlayer.GetComponent<PlayerController>();
                if (tempController != null)
                {
                    player = tempPlayer;
                    playerTarget = player.GetComponent<Target>();
                    playerGun = player.GetComponent<GunController>();
                }
            }
        }
        //Open close console
        if (toClose || Input.GetKeyDown(KeyCode.BackQuote))
        {
            toClose = false;
            open = !open;
            panel.SetActive(open);
            SetNotificationCenterBackVisible(open);
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
                String msgToParse = input.text.Trim();
                PlayerLobbyInfo lobbyInfo = player.GetComponent<PlayerLobbyInfo>();
                Color playerTeamColor = lobbyInfo.playerColor;
                String playerName = lobbyInfo.playerName;

                if (msgToParse.Length > 0 && msgToParse.StartsWith("/"))
                {
                    msgToParse = msgToParse.Remove(0, 1);
                    tokens = msgToParse.Split(' ');
                    if (tokens.Length >= 3 && tokens[0] == "add")
                    {
                        if (tokens.Length == 4 && tokens[1] == "wood")
                        {
                            if (!IsInt(tokens[2]) || !IsInt(tokens[3]))
                            {
                                NotificationManager.NewNotification(NotificationManager.GetRedString("Team or Amount not numbers"));
                                input.text = "";
                                input.ActivateInputField();
                                return;
                            }
                            ResourceBank temp;
                            if (tokens[2].Equals("1"))
                            {
                                temp = bank1;
                            }
                            else if (tokens[2].Equals("2"))
                            {
                                temp = bank2;
                            }
                            else
                            {
                                NotificationManager.NewNotification(NotificationManager.GetRedString("Invalid team number"));
                                input.text = "";
                                input.ActivateInputField();
                                return;
                            }
                            NotificationManager.NewNotification("Adding " + NotificationManager.GetColoredString(Color.green, Convert.ToInt32(tokens[3]).ToString()) + " to wood...");
                            temp.AddWood(Convert.ToInt32(tokens[3]));
                            input.text = "";
                            input.ActivateInputField();
                            return;
                        }
                        else if (tokens.Length == 4 && tokens[1] == "stone")
                        {
                            if (!IsInt(tokens[2]) || !IsInt(tokens[3]))
                            {
                                NotificationManager.NewNotification(NotificationManager.GetRedString("Team or Amount not numbers"));
                                input.text = "";
                                input.ActivateInputField();
                                return;
                            }
                            ResourceBank temp;
                            if (tokens[2].Equals("1"))
                            {
                                temp = bank1;
                            }
                            else if (tokens[2].Equals("2"))
                            {
                                temp = bank2;
                            }
                            else
                            {
                                NotificationManager.NewNotification(NotificationManager.GetRedString("Invalid team number"));
                                input.text = "";
                                input.ActivateInputField();
                                return;
                            }
                            NotificationManager.NewNotification("Adding " + NotificationManager.GetColoredString(Color.green, Convert.ToInt32(tokens[3]).ToString()) + " to stone...");
                            temp.AddStone(Convert.ToInt32(tokens[3]));
                            input.text = "";
                            input.ActivateInputField();
                            return;
                        }
                        else if (tokens.Length == 4 && tokens[1] == "metal")
                        {
                            if (!IsInt(tokens[2]) || !IsInt(tokens[3]))
                            {
                                NotificationManager.NewNotification(NotificationManager.GetRedString("Team or Amount not numbers"));
                                input.text = "";
                                input.ActivateInputField();
                                return;
                            }
                            ResourceBank temp;
                            if (tokens[2].Equals("1"))
                            {
                                temp = bank1;
                            }
                            else if (tokens[2].Equals("2"))
                            {
                                temp = bank2;
                            }
                            else
                            {
                                NotificationManager.NewNotification(NotificationManager.GetRedString("Invalid team number"));
                                input.text = "";
                                input.ActivateInputField();
                                return;
                            }
                            NotificationManager.NewNotification("Adding " + NotificationManager.GetColoredString(Color.green, Convert.ToInt32(tokens[3]).ToString()) + " to metal...");
                            temp.AddMetal(Convert.ToInt32(tokens[3]));
                            input.text = "";
                            input.ActivateInputField();
                            return;
                        }
                        else if (tokens[1] == "ammo")
                        {
                            if (!IsInt(tokens[2]))
                            {
                                NotificationManager.NewNotification(NotificationManager.GetRedString("Amount must be a number"));
                                input.text = "";
                                input.ActivateInputField();
                                return;
                            }
                            NotificationManager.NewNotification("Adding " + NotificationManager.GetColoredString(Color.green, Convert.ToInt32(tokens[2]).ToString()) + " to reserve ammo...");
                            playerGun.currentAmmoInReserve += int.Parse(tokens[2]);
                            toClose = true;
                        }
                        else
                        {
                            NotificationManager.NewNotification(NotificationManager.GetRedString("Unknown command"));
                            input.text = "";
                            input.ActivateInputField();
                        }
                    }
                    else if (tokens.Length >= 3 && tokens[0] == "set")
                    {
                        if (tokens.Length == 4 && tokens[1] == "wood")
                        {
                            if (!IsInt(tokens[2]) || !IsInt(tokens[3]))
                            {
                                NotificationManager.NewNotification(NotificationManager.GetRedString("Team or Amount not numbers"));
                                input.text = "";
                                input.ActivateInputField();
                                return;
                            }
                            ResourceBank temp;
                            if (tokens[2].Equals("1"))
                            {
                                temp = bank1;
                            }
                            else if (tokens[2].Equals("2"))
                            {
                                temp = bank2;
                            }
                            else
                            {
                                NotificationManager.NewNotification(NotificationManager.GetRedString("Invalid team number"));
                                input.text = "";
                                input.ActivateInputField();
                                return;
                            }
                            NotificationManager.NewNotification("Setting wood to " + NotificationManager.GetColoredString(Color.green, Convert.ToInt32(tokens[3]).ToString()));
                            temp.SetWood(Convert.ToInt32(tokens[3]));
                            input.text = "";
                            input.ActivateInputField();
                            return;
                        }
                        else if (tokens.Length == 4 && tokens[1] == "stone")
                        {
                            if (!IsInt(tokens[2]) || !IsInt(tokens[3]))
                            {
                                NotificationManager.NewNotification(NotificationManager.GetRedString("Team or Amount not numbers"));
                                input.text = "";
                                input.ActivateInputField();
                                return;
                            }
                            ResourceBank temp;
                            if (tokens[2].Equals("1"))
                            {
                                temp = bank1;
                            }
                            else if (tokens[2].Equals("2"))
                            {
                                temp = bank2;
                            }
                            else
                            {
                                NotificationManager.NewNotification(NotificationManager.GetRedString("Invalid team number"));
                                input.text = "";
                                input.ActivateInputField();
                                return;
                            }
                            NotificationManager.NewNotification("Setting stone to " + NotificationManager.GetColoredString(Color.green, Convert.ToInt32(tokens[3]).ToString()));
                            temp.SetStone(Convert.ToInt32(tokens[3]));
                            input.text = "";
                            input.ActivateInputField();
                            return;
                        }
                        else if (tokens.Length == 4 && tokens[1] == "metal")
                        {
                            if (!IsInt(tokens[2]) || !IsInt(tokens[3]))
                            {
                                NotificationManager.NewNotification(NotificationManager.GetRedString("Team or Amount not numbers"));
                                input.text = "";
                                input.ActivateInputField();
                                return;
                            }
                            ResourceBank temp;
                            if (tokens[2].Equals("1"))
                            {
                                temp = bank1;
                            }
                            else if (tokens[2].Equals("2"))
                            {
                                temp = bank2;
                            }
                            else
                            {
                                NotificationManager.NewNotification(NotificationManager.GetRedString("Invalid team number"));
                                input.text = "";
                                input.ActivateInputField();
                                return;
                            }
                            NotificationManager.NewNotification("Setting metal to " + NotificationManager.GetColoredString(Color.green, Convert.ToInt32(tokens[3]).ToString()));
                            temp.SetMetal(Convert.ToInt32(tokens[3]));
                            input.text = "";
                            input.ActivateInputField();
                            return;
                        }
                        else if (tokens[1] == "ammo")
                        {
                            if (!IsInt(tokens[2]))
                            {
                                NotificationManager.NewNotification(NotificationManager.GetRedString("Amount must be a number"));
                                input.text = "";
                                input.ActivateInputField();
                                return;
                            }
                            NotificationManager.NewNotification("Setting reserve ammo to " + NotificationManager.GetColoredString(Color.green, Convert.ToInt32(tokens[2]).ToString()));
                            playerGun.currentAmmoInReserve = int.Parse(tokens[2]);
                            input.text = "";
                            input.ActivateInputField();
                        }
                        else
                        {
                            NotificationManager.NewNotification(NotificationManager.GetRedString("Unknown command"));
                            input.text = "";
                            input.ActivateInputField();
                        }
                    }
                    else if (tokens.Length == 1 && tokens[0] == "kill")
                    {
                        if (playerTeamColor == Color.blue)
                            NotificationManager.NewNotification(NotificationManager.GetBlueString(playerName) + " commited suicide");
                        else
                            NotificationManager.NewNotification(NotificationManager.GetRedString(playerName) + " commited suicide");
                        playerTarget.Die();
                        input.text = "";
                        input.ActivateInputField();
                    }
                    else
                    {
                        NotificationManager.NewNotification(NotificationManager.GetRedString("Unknown command"));
                        input.text = "";
                        input.ActivateInputField();
                    }
                }
                else
                {
                    // Send message to other players here
                    // This message isn't actually implemented yet, but it will be later...possibly.
                    if (playerTeamColor == Color.blue)
                        NotificationManager.NewNotification(NotificationManager.GetBlueString(playerName) + ": " + msgToParse);
                    else
                        NotificationManager.NewNotification(NotificationManager.GetRedString(playerName) + ": " + msgToParse);
                    input.text = "";
                    input.ActivateInputField();
                }
            }
        }
    }

    private void SetNotificationCenterBackVisible(Boolean visible)
    {
        if (visible)
        {
            UIPanelImg.color = new Color(1f, 1f, 1f, 0.32f);
        }
        else
        {
            UIPanelImg.color = new Color(1f, 1f, 1f, 0);
        }
    }

    private Boolean IsInt(string strToTest)
    {
        Boolean output = false;
        try
        {
            int i = int.Parse(strToTest);
            output = true;
        }
        catch (Exception ex)
        {
            output = false;
        }
        return output;
    }
}
