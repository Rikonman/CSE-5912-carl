using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ResourceBank : NetworkBehaviour
{
    [SyncVar(hook = "SetStone")]
    public int stone;
    [SyncVar(hook = "SetWood")]
    public int wood;
    [SyncVar(hook = "SetMetal")]
    public int metal;
    // Use this for initialization
    void Start()
    {
    }

    public void ResetBank()
    {
        stone = 500;
        wood = 500;
        metal = 200;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetStone(int Stone)
    {
        stone = Stone;
    }

    public void SetWood(int Wood)
    {
        wood = Wood;
    }

    public void SetMetal(int Metal)
    {
        metal = Metal;
    }

    public void AddStone(int Amount)
    {
        stone += Amount;
    }

    public void AddWood(int Amount)
    {
        wood += Amount;
    }

    public void AddMetal(int Amount)
    {
        metal += Amount;
    }

    public void Add(string Type, int Amount)
    {
        if (Type == "Stone")
        {
            AddStone(Amount);
        }
        else if (Type == "Metal")
        {
            AddMetal(Amount);
        }
        else
        {
            AddWood(Amount);
        }
    }


}
