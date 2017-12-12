using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;

public class ProjectileController : NetworkBehaviour {

    [SerializeField]
    public float projectileLifetime = 2f;
    [SerializeField]
    bool canKill = false;
    public GameObject hitEffect;
    public Quaternion originalRotation;

    bool isLive = true;
    public bool isBouncy;
    float age;
    MeshRenderer projectileRenderer;
    public int firingTeam;
    public int firingPlayer;
    public string firingPlayerName;
    public int firingGun;
    public GameObject triangleBreak;
    public float damage;
    Rigidbody rb;
    private Vector3 oldVelocity;
    private Quaternion targetRotation;
    bool firingIgnorer;
    // Use this for initialization
    void Start ()
    {
        projectileRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        NetworkIdentity tempTB = triangleBreak.GetComponent<NetworkIdentity>();
        originalRotation = transform.rotation;
        targetRotation = transform.rotation;
        firingIgnorer = true;
        StartCoroutine(Delayer());
        //triangleBreak = Resources.Load()
    }

    public IEnumerator Delayer()
    {
        float remainingTime = .25f;

        while (remainingTime > 0)
        {
            yield return null;

            remainingTime -= Time.deltaTime;

        }
        firingIgnorer = false;
    }

    // Projectiles are updated by the server
    void Update ()
    {
        transform.rotation = originalRotation;
        rb.angularVelocity = Vector3.zero;
        if (originalRotation != targetRotation)
        {
            originalRotation = Quaternion.Lerp(originalRotation, targetRotation, .2f);
        }
        // if the projectile has been alive too long
        age += Time.deltaTime;
        if (age > projectileLifetime)
        {
            // destroy it on the network
            Destroy(gameObject);
        }
	}
    

    private void FixedUpdate()
    {
        oldVelocity = rb.velocity;
    }

    [Command]
    public void CmdChangeTargetRotation(Quaternion rotation)
    {
        targetRotation = rotation;
        RpcChangeTargetRotation(rotation);
    }

    [ClientRpc]
    public void RpcChangeTargetRotation(Quaternion rotation)
    {
        targetRotation = rotation;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Projectile")
            return;
        // if the projectile isn't live, leave (we only want to hit one thing and not go through objects)
        if (!isLive)
            return;

        PlayerTeam collisionTeam = collision.gameObject.GetComponent<PlayerTeam>();

        if (isBouncy)
        {
            Vector3 totalNormal = Vector3.zero;
            foreach (ContactPoint tempContact in collision.contacts)
            {
                totalNormal += tempContact.normal;
                Debug.DrawRay(tempContact.point, tempContact.normal, Color.white);
            }
            totalNormal = totalNormal.normalized;
            Vector3 reflectVector = Vector3.Reflect(oldVelocity, totalNormal);
            rb.velocity = reflectVector;

            Quaternion rotation = Quaternion.FromToRotation(oldVelocity, reflectVector);
            CmdChangeTargetRotation(rotation * targetRotation);
            
        }
        // if the projectile was fired by your team, leave
        if (collisionTeam != null && collisionTeam.team == firingTeam ||
            collision.gameObject.tag == "RedSpawnCore" && firingTeam == 0 ||
            collision.gameObject.tag == "BlueSpawnCore" && firingTeam == 1)
        {
            return;
        }
        BuildIdentifier collisionBID = collision.gameObject.GetComponent<BuildIdentifier>();


        if (!isBouncy)
        {
            // the projectile is going to explode and is no longer live
            isLive = false;
            // hide the projectile body
            projectileRenderer.enabled = false;
            HideProj();
        }

        ExplosionController tempExp = gameObject.GetComponent<ExplosionController>();
        if (tempExp != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            tempExp.StartExplosion();
            return;
        }
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

        DamageTarget(collisionTarget, collisionTeam == null, collision.gameObject.tag, hasParent, 1f, collisionBID != null && collisionBID.team == firingTeam);

    }

    public void HideProj()
    {
        // the projectile is going to explode and is no longer live
        isLive = false;
        // hide the projectile body
        projectileRenderer.enabled = false;
    }

    [Command]
    public void CmdAddKillMessage(string killerPlayer, int killerTeam, string victimName, int victimTeam, int firingGun)
    {
        RpcAddKillMessage(killerPlayer, killerTeam, victimName, victimTeam, firingGun);
    }

    [ClientRpc]
    public void RpcAddKillMessage(string killerPlayer, int killerTeam, string victimName, int victimTeam, int firingGun)
    {
        GameObject.Find("KillsPanel").GetComponent<KillManager>().AddKill(killerPlayer, killerTeam, victimName, victimTeam, firingGun);
    }

    public void DamageTarget(Target collisionTarget, bool isBuilding, string collisionTag, bool hasParent, float damageModifier, bool isFriendlyFire)
    {
        Vector3 position = collisionTarget.transform.position;
        Quaternion rotation = collisionTarget.transform.localRotation;

        // have the target take damage
        bool died = collisionTarget.TakeDamage(damage / damageModifier / (isFriendlyFire ? 4f : 1f));
        if (died && !isBuilding)
        {

            CmdAddKillMessage(firingPlayerName, firingTeam, collisionTarget.gameObject.GetComponent<PlayerLobbyInfo>().playerName,
                collisionTarget.gameObject.GetComponent<PlayerTeam>().team, firingGun);
        }
        else if (died && isBuilding)
        {
            bool isStone = false;
            bool isCore = false;
            if (collisionTarget.bid != null)
            {
                isStone = collisionTarget.bid.isStone;
            }
            else if (collisionTag == "RedSpawnCore" || collisionTag == "BlueSpawnCore")
            {
                CmdAddKillMessage(firingPlayerName, firingTeam, "Spawn Core", firingTeam == 0 ? 1 : 0, firingGun);
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
            tempTriangleExplosion.isCore = isCore;

            //NetworkServer.Lo.SetLocalObject(tempTB.netId, triangleBreak);
            CmdDoBreak(position, rotation, M.vertices, M.normals, M.uv, M.GetTriangles(0), isStone, isCore);

        }
    }

    [Command]
    public void CmdDoBreak(Vector3 position, Quaternion rotation, 
        Vector3[] verts, Vector3[] normals, Vector2[] uvs, int[] indices, bool isStone, bool isCore)
    {

        //GameObject triangleBreak = NetworkServer.FindLocalObject(triangleBreakID);
        GameObject instance = (GameObject)Instantiate(triangleBreak, position, rotation);
        NetworkServer.Spawn(instance);
        TriangleExplosion tempTriangleExplosion = instance.GetComponent<TriangleExplosion>();
        tempTriangleExplosion.verts = verts;
        tempTriangleExplosion.normals = normals;
        tempTriangleExplosion.uvs = uvs;

        tempTriangleExplosion.indices = indices;
        tempTriangleExplosion.isStone = isStone;
        tempTriangleExplosion.isCore = isCore;
        //StartCoroutine(instance.GetComponent<TriangleExplosion>().SplitMesh(true));
        NetworkIdentity tempNetworkID = instance.GetComponent<NetworkIdentity>();
        RpcDoBreak(tempNetworkID.netId, position, rotation, verts, normals, uvs, indices, isStone, isCore);
    }

    [ClientRpc]
    public void RpcDoBreak(NetworkInstanceId triangleBreakID, Vector3 position, Quaternion rotation,
        Vector3[] verts, Vector3[] normals, Vector2[] uvs, int[] indices, bool isStone, bool isCore)
    {

        GameObject triangleBreak = ClientScene.FindLocalObject(triangleBreakID);
        if (triangleBreak != null)
        {
            GameObject instance = (GameObject)Instantiate(triangleBreak, position, rotation);
            TriangleExplosion tempTriangleExplosion = instance.GetComponent<TriangleExplosion>();
            tempTriangleExplosion.verts = verts;
            tempTriangleExplosion.normals = normals;
            tempTriangleExplosion.uvs = uvs;

            tempTriangleExplosion.indices = indices;
            tempTriangleExplosion.isStone = isStone;
            tempTriangleExplosion.isCore = isCore;
            StartCoroutine(instance.GetComponent<TriangleExplosion>().SplitMesh(true));
        }
        
    }
}
