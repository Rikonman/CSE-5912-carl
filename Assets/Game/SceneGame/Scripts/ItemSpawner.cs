using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemSpawner : NetworkBehaviour {
    public GameObject spawnedObject;
    public bool isProjectile;
    public float timer;
    public float spawnCooldown;
    ProjectileController pc;
    Rigidbody rb;
	// Use this for initialization
	void Start () {
        if (isServer)
        {
            timer = 0f;
            pc = gameObject.GetComponent<ProjectileController>();
            rb = gameObject.GetComponent<Rigidbody>();
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (isServer)
        {
            timer += Time.deltaTime;
            if (timer >= spawnCooldown)
            {
                timer = 0;
                Vector3 newPos = transform.position;
                newPos = newPos - rb.velocity.normalized;
                CmdSpawnProjectile(pc.firingTeam, pc.firingPlayer, pc.damage, pc.firingGun, pc.firingPlayerName, newPos,
                    transform.rotation, rb.velocity / 2f, isProjectile);
            }
        }
	}

    // This command is called from the localPlayer and run on the server. Note that Commands must begin with 'Cmd'
    [Command]
    void CmdSpawnProjectile(int team, int playerID, float damage, int gunChoice, string playerName, Vector3 position, Quaternion rotation, Vector3 startVelocity, bool isProj)
    {
        GameObject instance = Instantiate(spawnedObject, position, rotation);
        if (isProj)
        {
            ProjectileController pc = instance.GetComponent<ProjectileController>();
            instance.GetComponent<Rigidbody>().velocity = startVelocity;
            pc.firingTeam = team;
            pc.firingPlayer = playerID;
            pc.firingPlayerName = playerName;
            pc.damage = damage;
            pc.firingGun = gunChoice;
            NetworkServer.Spawn(instance);
            RpcUpdateProjectileData(instance.GetComponent<NetworkIdentity>().netId, team, playerID, damage, gunChoice, pc.projectileLifetime, playerName);
        }
    }

    [ClientRpc]
    public void RpcUpdateProjectileData(NetworkInstanceId nid, int team, int playerID, float damage, int gunChoice, float projectileLifetime, string playerName)
    {
        GameObject projectile = ClientScene.FindLocalObject(nid);
        ProjectileController pc = projectile.GetComponent<ProjectileController>();
        pc.firingTeam = team;
        pc.firingPlayer = playerID;
        pc.firingPlayerName = playerName;
        pc.damage = damage;
        pc.firingGun = gunChoice;
        pc.projectileLifetime = projectileLifetime;
    }
}
