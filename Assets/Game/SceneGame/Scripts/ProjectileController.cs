using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ProjectileController : NetworkBehaviour {

    [SerializeField]
    float projectileLifetime = 2f;
    [SerializeField]
    bool canKill = false;
    public GameObject hitEffect;

    bool isLive = true;
    float age;
    MeshRenderer projectileRenderer;
    public int firingTeam;
    public int firingPlayer;
    public GameObject triangleBreak;

    // Use this for initialization
    void Start () {
        projectileRenderer = GetComponent<MeshRenderer>();
        NetworkIdentity tempTB = triangleBreak.GetComponent<NetworkIdentity>();
    }
	
	// Projectiles are updated by the server
    [ServerCallback]
	void Update ()
    {
        // if the projectile has been alive too long
        age += Time.deltaTime;
        if (age > projectileLifetime)
        {
            // destroy it on the network
            NetworkServer.Destroy(gameObject);
        }
	}

    void OnCollisionEnter(Collision collision)
    {
        // if the projectile isn't live, leave (we only want to hit one thing and not go through objects)
        if (!isLive)
            return;

        // if the projectile was fired by your team, leave
        PlayerTeam collisionTeam = collision.gameObject.GetComponent<PlayerTeam>();
        if (collisionTeam != null && collisionTeam.team == firingTeam)
        {
            return;
        }

        // the projectile is going to explode and is no longer live
        isLive = false;

        // hide the projectile body
        projectileRenderer.enabled = false;
        // show the explosion particle effect
        GameObject tempHitEffect = Instantiate(hitEffect, gameObject.transform.position, Quaternion.LookRotation(gameObject.transform.forward, Vector3.up));
        Destroy(tempHitEffect, 0.3f);

        if (!isServer)
            return;
        Target collisionTarget;
        bool hasParent = collision.gameObject.transform.parent != null;
        if (hasParent)
        {
            collisionTarget = collision.gameObject.transform.parent.GetComponent<Target>();
        }
        else
        {
            collisionTarget = collision.gameObject.GetComponent<Target>();
        }
        // if the projectile isn't lethal or it hit something that isn't a target, leave
         
        if (!canKill || collisionTarget == null)
            return;

        Vector3 position = collisionTarget.transform.position;
        Quaternion rotation = collisionTarget.transform.localRotation;

        // have the target take damage
        bool died = collisionTarget.TakeDamage(25);

        //explode the dead
        if (died && collisionTeam == null)
        {
            bool isStone = false;
            bool isCore = false;
            if (collisionTarget.bid != null)
            {
                isStone = collisionTarget.bid.isStone;
            }
            else
            {
                isCore = true;
            } 
            Mesh M = new Mesh();
            MeshFilter tempFilter;
            SkinnedMeshRenderer tempSkinnedRenderer;
            MeshRenderer tempRenderer;
            if (hasParent)
            {
                tempFilter = collisionTarget.GetComponentInChildren<MeshFilter>();
                tempSkinnedRenderer = collisionTarget.GetComponentInChildren<SkinnedMeshRenderer>();
                tempRenderer = collisionTarget.GetComponentInChildren<MeshRenderer>();
            }
            else
            {
                tempFilter = collisionTarget.GetComponent<MeshFilter>();
                tempSkinnedRenderer = collisionTarget.GetComponent<SkinnedMeshRenderer>();
                tempRenderer = collisionTarget.GetComponent<MeshRenderer>();
            }

            if (tempFilter != null)
            {
                M = tempFilter.mesh;
            }
            else if (tempSkinnedRenderer != null)
            {
                M = tempSkinnedRenderer.sharedMesh;
            }
            

            TriangleExplosion tempTriangleExplosion = triangleBreak.GetComponent<TriangleExplosion>();
            tempTriangleExplosion.verts = M.vertices;
            tempTriangleExplosion.normals = M.normals;
            tempTriangleExplosion.uvs = M.uv;

            tempTriangleExplosion.indices = M.GetTriangles(0);
            tempTriangleExplosion.isStone = isStone;
            
            //NetworkServer.Lo.SetLocalObject(tempTB.netId, triangleBreak);
            CmdDoBreak(triangleBreak, position, rotation);

        }

    }
    

    [Command]
    public void CmdDoBreak(GameObject triangleBreak, Vector3 position, Quaternion rotation)
    {

        //GameObject triangleBreak = NetworkServer.FindLocalObject(triangleBreakID);
        GameObject instance = (GameObject)Instantiate(triangleBreak, position, rotation);
        NetworkServer.Spawn(instance);
        //StartCoroutine(instance.GetComponent<TriangleExplosion>().SplitMesh(true));
        NetworkIdentity tempNetworkID = instance.GetComponent<NetworkIdentity>();
        RpcDoBreak(tempNetworkID.netId, position, rotation, instance.GetComponent<TriangleExplosion>().isStone);
    }

    [ClientRpc]
    public void RpcDoBreak(NetworkInstanceId triangleBreakID, Vector3 position, Quaternion rotation, bool isStone)
    {

        GameObject triangleBreak = ClientScene.FindLocalObject(triangleBreakID);
        GameObject instance = (GameObject)Instantiate(triangleBreak, position, rotation);
        instance.GetComponent<TriangleExplosion>().isStone = isStone;
        StartCoroutine(instance.GetComponent<TriangleExplosion>().SplitMesh(true));
    }
}
