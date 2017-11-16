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
    public GameObject baseObject;
    public static int playerCount = 0;
    float uiRefreshTimer;
    
    Text trWoodResources;
    Text trStoneResources;
    Text trMetalResources;
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
            if (isLocalPlayer)
            {
                trWoodResources = GameObject.Find("TRWoodPanel").GetComponentInChildren<Text>();
                trStoneResources = GameObject.Find("TRStonePanel").GetComponentInChildren<Text>();
                trMetalResources = GameObject.Find("TRMetalPanel").GetComponentInChildren<Text>();
                Text trTeamText = GameObject.Find("TRTeamText").GetComponentInChildren<Text>();
                Image img1 = GameObject.Find("TRTeamPanel").GetComponent<Image>();
                Image img2 = GameObject.Find("TRWoodPanel").GetComponent<Image>();
                Image img3 = GameObject.Find("TRStonePanel").GetComponent<Image>();
                Image img4 = GameObject.Find("TRMetalPanel").GetComponent<Image>();
                trTeamText.text = teamName;
                img1.color = new Color(0, 0, 1f, 0.3f);
                img2.color = new Color(0, 0, 1f, 0.3f);
                img3.color = new Color(0, 0, 1f, 0.3f);
                img4.color = new Color(0, 0, 1f, 0.3f);
            }
        }
        else
        {
            team = 0;
            teamName = "Red Team";
            if (isLocalPlayer)
            {
                trWoodResources = GameObject.Find("TRWoodPanel").GetComponentInChildren<Text>();
                trStoneResources = GameObject.Find("TRStonePanel").GetComponentInChildren<Text>();
                trMetalResources = GameObject.Find("TRMetalPanel").GetComponentInChildren<Text>();
                Text trTeamText = GameObject.Find("TRTeamText").GetComponentInChildren<Text>();
                Image img1 = GameObject.Find("TRTeamPanel").GetComponent<Image>();
                Image img2 = GameObject.Find("TRWoodPanel").GetComponent<Image>();
                Image img3 = GameObject.Find("TRStonePanel").GetComponent<Image>();
                Image img4 = GameObject.Find("TRMetalPanel").GetComponent<Image>();
                trTeamText.text = teamName;
                img1.color = new Color(1f, 0, 0, 0.3f);
                img2.color = new Color(1f, 0, 0, 0.3f);
                img3.color = new Color(1f, 0, 0, 0.3f);
                img4.color = new Color(1f, 0, 0, 0.3f);
            }
        }

        uiRefreshTimer = 0;
        if (isLocalPlayer)
        {
            LobbyManager.s_Singleton.CheckIfSpawnsCreated();

        }
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

        baseObject = GameObject.Find("Base" + (team + 1) + "Center");
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

        if (isLocalPlayer && baseObject != null)
        {
            uiRefreshTimer += Time.deltaTime;
            if (uiRefreshTimer >= .5f)
            {
                ResourceBank tempBank = baseObject.GetComponent<ResourceBank>();
                trWoodResources.text = tempBank.wood.ToString();
                trStoneResources.text = tempBank.stone.ToString();
                trMetalResources.text = tempBank.metal.ToString();

                uiRefreshTimer = 0f;
            }
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
                NotificationManager.NewNotification("Picked up <color=#00FF00>" + TempController.amount.ToString() + "</color> stone");
            }
            else if (other.gameObject.CompareTag("Pick Up (Wood)"))
            {
                PickUpController TempController = other.gameObject.GetComponent<PickUpController>();
                CmdAddToResources("Wood", TempController.amount, team);
                TempController.StartRespawnTimer();
                other.gameObject.GetComponent<MeshRenderer>().enabled = false;
                other.gameObject.GetComponent<BoxCollider>().enabled = false;
                NotificationManager.NewNotification("Picked up <color=#00FF00>" + TempController.amount.ToString() + "</color> wood");
            }
            else if (other.gameObject.CompareTag("Pick Up (Metal)"))
            {
                PickUpController TempController = other.gameObject.GetComponent<PickUpController>();
                CmdAddToResources("Metal", TempController.amount, team);
                TempController.StartRespawnTimer();
                other.gameObject.GetComponent<MeshRenderer>().enabled = false;
                other.gameObject.GetComponent<BoxCollider>().enabled = false;
                NotificationManager.NewNotification("Picked up <color=#00FF00>" + TempController.amount.ToString() + "</color> metal");
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
