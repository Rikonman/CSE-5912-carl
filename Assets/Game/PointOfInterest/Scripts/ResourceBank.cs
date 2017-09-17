using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBank : MonoBehaviour {
    public int Stone;
    public int Wood;
    // Use this for initialization
    void Start () {
        Stone = 100;
        Wood = 100;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Add(string Type, int Amount)
    {
        if (Type == "Stone")
        {
            Stone += Amount;
        }
        else
        {
            Wood += Amount;
        }
    }
}
