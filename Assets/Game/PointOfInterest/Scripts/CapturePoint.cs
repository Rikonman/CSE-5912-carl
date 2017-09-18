<<<<<<< HEAD
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturePoint : MonoBehaviour {
    public float radius;
    public float ownership;
    public float captureTime;
    public int resourcePerSecond;
    public float resourceTimer;
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
	void Start () {
        resourceTimer = 0;

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        GameObject[] tempPlayers = GameObject.FindGameObjectsWithTag("Player");
        int TeamOnePlayers = 0;
        int TeamTwoPlayers = 0;
        foreach (GameObject tempPlayer in tempPlayers)
        {
            float magnitude = (transform.position - tempPlayer.transform.position).magnitude;
            if (magnitude < radius)
            {
                if (tempPlayer.GetComponent<PlayerController>().team == 0)
                {
                    TeamOnePlayers++;
                }
                else
                {
                    TeamTwoPlayers++;
                }
            }
        }
        Ownership += (TeamOnePlayers - TeamTwoPlayers) * Time.deltaTime;
        if (currentOwner != "No One")
        {
            if (resourceTimer >= 1f)
            {
                resourceTimer = 0;
                int teamNumber = currentOwner == "Team One" ? 1 : 2;
                GameObject baseObject = GameObject.Find("Base" + teamNumber);
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
=======
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturePoint : MonoBehaviour {
    public float radius;
    public float ownership;
    public float captureTime;
    public int resourcePerSecond;
    public float resourceTimer;
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
	void Start () {
        resourceTimer = 0;

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        GameObject[] tempPlayers = GameObject.FindGameObjectsWithTag("Player");
        int TeamOnePlayers = 0;
        int TeamTwoPlayers = 0;
        foreach (GameObject tempPlayer in tempPlayers)
        {
            float magnitude = (transform.position - tempPlayer.transform.position).magnitude;
            if (magnitude < radius)
            {
                if (tempPlayer.GetComponent<PlayerController>().team == 0)
                {
                    TeamOnePlayers++;
                }
                else
                {
                    TeamTwoPlayers++;
                }
            }
        }
        Ownership += (TeamOnePlayers - TeamTwoPlayers) * Time.deltaTime;
        if (currentOwner != "No One")
        {
            if (resourceTimer >= 1f)
            {
                resourceTimer = 0;
                int teamNumber = currentOwner == "Team One" ? 1 : 2;
                GameObject baseObject = GameObject.Find("Base" + teamNumber);
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
>>>>>>> parent of 14d3936... Merge remote-tracking branch 'origin/master'
