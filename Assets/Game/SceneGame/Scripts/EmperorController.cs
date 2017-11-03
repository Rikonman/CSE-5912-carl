using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EmperorController : NetworkBehaviour
{
    [SyncVar(hook = "RedFavorChanged")]
    public int redFavor;
    [SyncVar(hook = "BlueFavorChanged")]
    public int blueFavor;

    [SyncVar(hook = "EntertainmentChanged")]
    public int entertainment;
    public string mood;
    private float boredomTimer;
    public float boredomCooldown;
    public GameObject RoundManager;
    public RoundManager RoundScript;
    public GameObject emperorText;
    public Text emperorOutput;
    public float popupDelay;


    public int moodChangeCost;
	// Use this for initialization
	void Start () {
        redFavor = 0;
        blueFavor = 0;
        entertainment = 20;
        boredomTimer = 0f;
        RoundManager = GameObject.Find("Round Manager");
        RoundScript = RoundManager.GetComponent<RoundManager>();
        StartCoroutine(TextDelayer());
    }

    public IEnumerator TextDelayer()
    {
        float remainingTime = 1f;

        while (remainingTime > 0)
        {
            yield return null;

            remainingTime -= Time.deltaTime;

        }

        emperorText = GameObject.Find("EmperorText");
        emperorOutput = emperorText.GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        if (RoundScript != null)
        {
            if (RoundScript.buildRoundSecondsLeft <= 0)
            {

                boredomTimer += Time.deltaTime;
                if (boredomTimer >= boredomCooldown)
                {
                    entertainment -= 1;
                    boredomTimer = 0;
                }
            }
        }
    }

    private void RedFavorChanged(int newVal)
    {
        redFavor = newVal;
    }

    private void BlueFavorChanged(int newVal)
    {
        blueFavor = newVal;
    }

    private void EntertainmentChanged(int newVal)
    {
        entertainment = newVal;
        string previousMood = mood;
        if (entertainment >= moodChangeCost * 2)
        {
            mood = "Captivated";
        }
        else if (entertainment >= moodChangeCost)
        {
            mood = "Entertained";
        }
        else if (entertainment >= 0)
        {
            mood = "Interested";
        }
        else if (entertainment >= -moodChangeCost)
        {
            mood = "Losing Interest";
        }
        else if (entertainment >= -moodChangeCost * 2)
        {
            mood = "Bored";
        }
        else
        {
            mood = "Displeased";
        }
        if (emperorOutput != null)
        {
            if (previousMood != mood)
            {
                emperorOutput.enabled = true;
                emperorOutput.text = "The Emperor is " + mood + "!";
                StartCoroutine(MoodChangeDelay());
            }
        }
    }


    public IEnumerator MoodChangeDelay()
    {
        float remainingTime = popupDelay;

        while (remainingTime > 0)
        {
            yield return null;

            remainingTime -= Time.deltaTime;

        }
        emperorOutput.enabled = false;
    }

    [ClientRpc]
    public void RpcAddRedFavor(int Amount)
    {
        redFavor += Amount;
    }

    [ClientRpc]
    public void RpcAddBlueFavor(int Amount)
    {
        blueFavor += Amount;
    }

    [ClientRpc]
    public void RpcAddEntertainment(int Amount)
    {
        entertainment += Amount;
    }
}
