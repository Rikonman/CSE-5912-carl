using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerTeam : NetworkBehaviour {

    [SyncVar]
    public int team;
    [SyncVar]
    public int playerID;
    public GameObject resourceText;
    public GameObject baseObject;
    public static int playerCount = 0;
    // Use this for initialization
    void Start () {
        try
        {
            playerID = playerCount;
            playerCount++;
            Debug.Log(playerID);
            team = playerID % 2;
            if (isLocalPlayer)
            {
                resourceText = GameObject.Find("ResourceText");
            }
            baseObject = GameObject.Find("Base" + (team + 1) + "Center");
        }
        catch
        {

        }
    }
    
    private void Update()
    {
        if (baseObject == null)
        {
            
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isLocalPlayer)
        {
            if (other.gameObject.CompareTag("Pick Up (Stone)"))
            {
                PickUpController TempController = other.gameObject.GetComponent<PickUpController>();
                //baseObject.GetComponent<ResourceBank>().Add("Stone", TempController.amount);
                CmdAddToResources("Stone", TempController.amount, team);
                TempController.StartRespawnTimer();
                other.gameObject.GetComponent<MeshRenderer>().enabled = false;
                other.gameObject.GetComponent<BoxCollider>().enabled = false;
            }
            else if (other.gameObject.CompareTag("Pick Up (Wood)"))
            {
                PickUpController TempController = other.gameObject.GetComponent<PickUpController>();
                //baseObject.GetComponent<ResourceBank>().Add("Wood", TempController.amount);
                CmdAddToResources("Wood", TempController.amount, team);
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
