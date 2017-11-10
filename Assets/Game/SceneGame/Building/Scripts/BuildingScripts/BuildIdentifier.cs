﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BuildIdentifier : NetworkBehaviour {
    [SyncVar]
    public int team;
    [SyncVar]
    public int id;
    [SyncVar]
    public int parentMountPoint;
    [SyncVar]
    public int parentMountBool;
    // Use this for initialization
    void Start ()
    {
    }

    public void DecrementIDIfHigher(int childMountPoint)
    {

        if (id > childMountPoint)
        {
            id--;
        }
    }

    public void DecrementMPIfHigher(int childMountPoint)
    {

        if (parentMountPoint > childMountPoint)
        {
            parentMountPoint--;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
