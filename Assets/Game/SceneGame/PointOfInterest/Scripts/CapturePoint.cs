using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class CapturePoint : NetworkBehaviour
{

    public float radius;
    public float captureTime;
    public int resourcePerSecond;

    [SyncVar]
    public float ownership;
    [SyncVar]
    public float resourceTimer;
    [SyncVar]
    public string currentOwner;

    public float Ownership
    {
        get
        {
            return ownership;
        }
        set
        {
            if (value > captureTime * 2)
            {
                ownership = captureTime * 2;
            }
            else if (value < -captureTime * 2)
            {
                ownership = -captureTime * 2;
            }
            else
            {
                if (value < -captureTime)
                {
                    currentOwner = "Team Two";
                }
                else if (value > captureTime)
                {
                    currentOwner = "Team One";
                }
                else
                {
                    currentOwner = "No One";
                }
                ownership = value;
            }
        }
    }
    // Use this for initialization
    void Start()
    {
        resourceTimer = 0;
        Ownership = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GameObject[] tempPlayers = GameObject.FindGameObjectsWithTag("Player");
        float timeAdded = 0f;
        foreach (GameObject tempPlayer in tempPlayers)
        {
            float magnitude = (transform.position - tempPlayer.transform.position).magnitude;
            if (magnitude < radius)
            {
                PlayerTeam tempTeam = tempPlayer.GetComponent<PlayerTeam>();
                if (tempTeam.team == 0)
                {
                    timeAdded += Time.deltaTime;
                }
                else
                {
                    timeAdded -= Time.deltaTime;
                }
            }

        }
        Ownership += timeAdded;
        if (currentOwner != "No One")
        {
            if (resourceTimer >= 1f)
            {
                resourceTimer = 0;
                int teamNumber = currentOwner == "Team One" ? 1 : 2;
                GameObject baseObject = GameObject.Find("Base" + teamNumber + "Center");
                baseObject.GetComponent<ResourceBank>().Add("Stone", resourcePerSecond);
                baseObject.GetComponent<ResourceBank>().Add("Wood", resourcePerSecond);
            }
            else
            {
                resourceTimer += Time.deltaTime;
            }
        }

    }
}
