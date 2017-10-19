using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerTeam : NetworkBehaviour {

    [SyncVar]
    public int team;
    [SyncVar]
    public int playerID;
    public GameObject resourceText;
    Text resources;
    public GameObject baseObject;
    public static int playerCount = 0;
    // Use this for initialization
    void Start()
    {
        playerID = playerCount;
        playerCount++;
        Debug.Log(playerID);
        team = playerID % 2;
        if (isLocalPlayer)
        {
            resourceText = GameObject.Find("ResourceText");
            resources = resourceText.GetComponent<Text>();
        }
    }
    
    private void FixedUpdate()
    {
        if (baseObject == null)
        {
            baseObject = GameObject.Find("Base" + (team + 1) + "Center");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isLocalPlayer)
        {
            if (other.gameObject.CompareTag("Pick Up (Stone)"))
            {
                PickUpController TempController = other.gameObject.GetComponent<PickUpController>();
                CmdAddToResources("Stone", TempController.amount, team);
                TempController.StartRespawnTimer();
                other.gameObject.GetComponent<MeshRenderer>().enabled = false;
                other.gameObject.GetComponent<BoxCollider>().enabled = false;
            }
            else if (other.gameObject.CompareTag("Pick Up (Wood)"))
            {
                PickUpController TempController = other.gameObject.GetComponent<PickUpController>();
                CmdAddToResources("Wood", TempController.amount, team);
                TempController.StartRespawnTimer();
                other.gameObject.GetComponent<MeshRenderer>().enabled = false;
                other.gameObject.GetComponent<BoxCollider>().enabled = false;
            }
            else if (other.gameObject.CompareTag("Pick Up (Metal)"))
            {
                PickUpController TempController = other.gameObject.GetComponent<PickUpController>();
                CmdAddToResources("Metal", TempController.amount, team);
                TempController.StartRespawnTimer();
                other.gameObject.GetComponent<MeshRenderer>().enabled = false;
                other.gameObject.GetComponent<BoxCollider>().enabled = false;
            }
        }
        

    }

    [Command]
    void CmdAddToResources(string Type, int amount, int Team)
    {
        GameObject serverBaseObject = GameObject.Find("Base" + (team + 1) + "Center");
        serverBaseObject.GetComponent<ResourceBank>().Add(Type, amount);
    }
}
