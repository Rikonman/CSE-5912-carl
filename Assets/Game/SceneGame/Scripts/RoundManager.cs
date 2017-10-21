using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RoundManager : NetworkBehaviour {
    [SerializeField]
    private Text txtRoundManager;

    [SerializeField]
    private int buildRoundSeconds = 120;

    [SyncVar(hook = "buildRoundSecondsLeftChanged")]
    private int buildRoundSecondsLeft;

    private DateTime lastTick;

	// Use this for initialization
	void Start () {
        lastTick = DateTime.Now;
        buildRoundSecondsLeft = buildRoundSeconds;
    }
	
	// Update is called once per frame
	void Update () {
        if (!isServer)
            return;

		if (DateTime.Now.Subtract(lastTick).TotalSeconds > 1)
        {
            lastTick = DateTime.Now;
            buildRoundSecondsLeft--;
        }
	}

    private void buildRoundSecondsLeftChanged(int newVal)
    {
        buildRoundSecondsLeft = newVal;
        if (newVal > 0)
        {
            // The countdown is still happening. Show the current time left.
            int minutes = (int)((double)newVal / 60);
            int seconds = buildRoundSecondsLeft - (minutes * 60);
            txtRoundManager.text = "Build Round - " + String.Format("{0:00}", minutes) + ":" + String.Format("{0:00}", seconds);
        }
        else if (newVal == 0)
        {
            // The countdown is now at 0. Show the countdown has ended.
            txtRoundManager.text = "Build Round Ended!";
        }
        else
        {
            // The countdown has ended and we are showing that to the user.
            // Do something here to disable/change what is showing to the user.
            // For example, newVal == -3 means do this 3 seconds after the countdown has ended.
            if (newVal == -3)
            {
                txtRoundManager.text = "PvP now enabled!";
            }
            else if (newVal == -5)
            {
                // Take the message off the screen.
                txtRoundManager.gameObject.SetActive(false);
            }
        }
        
    }
}
