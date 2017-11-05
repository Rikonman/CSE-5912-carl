using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BaseBuildings : NetworkBehaviour
{
    public SyncStructList pointUsed = new SyncStructList();
    public MountPointList mountPoints = new MountPointList();
    //public List<Vector3> placedObjects = new List<Vector3>();
    public SyncStructVector placedObjects = new SyncStructVector();
    // Use this for initialization
    void Start () {

    }

    public void AddPointUsed(SyncStructBool point)
    {
        pointUsed.Add(point);
        //CmdAddPointUsed(point);
    }

    [Command]
    public void CmdAddPointUsed(SyncStructBool point)
    {
        //pointUsed.Add(point);
        RpcAddPointUsed(point);
    }

    [ClientRpc]
    public void RpcAddPointUsed(SyncStructBool point)
    {
        pointUsed.Add(point);
    }

    public void AddMountPoint(MountPoint mountPoint)
    {
        mountPoints.Add(mountPoint);
        //CmdAddMountPoint(mountPoint);
    }

    [Command]
    public void CmdAddMountPoint(MountPoint mountPoint)
    {
        //mountPoints.Add(mountPoint);
        RpcAddMountPoint(mountPoint);
    }

    [ClientRpc]
    public void RpcAddMountPoint(MountPoint mountPoint)
    {
        mountPoints.Add(mountPoint);
    }

    public void AddPlacedObject(Vector3 placedObject)
    {
        placedObjects.Add(placedObject);
        //CmdAddPlacedObject(placedObject);
    }

    [Command]
    public void CmdAddPlacedObject(Vector3 placedObject)
    {
        //placedObjects.Add(placedObject);
        RpcAddPlacedObject(placedObject);
    }

    [ClientRpc]
    public void RpcAddPlacedObject(Vector3 placedObject)
    {
        placedObjects.Add(placedObject);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
