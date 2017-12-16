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
            tempExp.StartExplosion();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
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

        collisionTarget.DamageTarget(firingPlayerName, firingTeam, firingGun, damage, 1f, collisionBID != null && collisionBID.team == firingTeam);

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

    
}
