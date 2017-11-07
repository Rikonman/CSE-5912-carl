using Prototype.NetworkLobby;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerTeam : NetworkBehaviour {

    [SyncVar]
    public int team;
    public string teamName;
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
        PlayerLobbyInfo lobbyInfo = GetComponentInChildren<PlayerLobbyInfo>();
        if (lobbyInfo.playerColor == Color.blue)
        {
            team =  1;
            teamName = "Blue Team";
        }
        else
        {
            team = 0;
            teamName = "Red Team";
        }


        if (isLocalPlayer)
        {
            resourceText = GameObject.Find("ResourceText");
            resources = resourceText.GetComponent<Text>();
        }
        LobbyManager.s_Singleton.CheckIfSpawnsCreated();
        StartCoroutine(SpawnDelayer());
    }


    public IEnumerator SpawnDelayer()
    {
        float remainingTime = 1f;

        while (remainingTime > 0)
        {
            yield return null;

            remainingTime -= Time.deltaTime;

        }
        RpcChangeLocation(LobbyManager.s_Singleton.GetSpawnLocation(team));
    }

    [ClientRpc]
    public void RpcChangeLocation(Vector3 spawnLocation)
    {
        transform.SetPositionAndRotation(spawnLocation, Quaternion.identity);
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
