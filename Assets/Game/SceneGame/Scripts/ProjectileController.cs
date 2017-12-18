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
    public bool onlyHurtPlayer;
    public bool persistent;
    public bool positionLocked;
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
        positionLocked = false;
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
        if (!persistent)
        {
            // if the projectile has been alive too long
            age += Time.deltaTime;
            if (age > projectileLifetime)
            {
                if (isServer)
                {
                    CmdHideProj();
                }

                // destroy it on the network
                Destroy(gameObject, 2f);
                age = -2f;
            }
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Projectile" && !persistent)
        {
            return;
        }
        else if (other.gameObject.tag == "Projectile" && other.gameObject.GetComponent<ProjectileController>().persistent)
        {
            return;
        }
        else if (other.gameObject.tag == "Projectile" && other.gameObject.GetComponent<ProjectileController>().isLive)
        {
            if (isServer)
            {
                CmdHideProj();
            }
            return;
        }
        else if ((other.gameObject.tag == "Ground" || other.gameObject.tag == "Building") && persistent && !positionLocked)
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            Ray lowerRay = new Ray(transform.position, -Vector3.up);
            RaycastHit lowerRayhit;

            if (Physics.Raycast(lowerRay, out lowerRayhit, 0.3f))
            {
                if (lowerRayhit.transform.gameObject.tag == "Ground" || lowerRayhit.transform.gameObject.tag == "Building")
                {
                    rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
                    positionLocked = true;
                }
            }
        }
        // if the projectile isn't live, leave (we only want to hit one thing and not go through objects)
        if (!isLive)
            return;

        PlayerTeam collisionTeam = other.gameObject.GetComponent<PlayerTeam>();
        
        // if the projectile was fired by your team, leave
        if (collisionTeam != null && collisionTeam.team == firingTeam ||
            other.gameObject.tag == "RedSpawnCore" && firingTeam == 0 ||
            other.gameObject.tag == "BlueSpawnCore" && firingTeam == 1 ||
            onlyHurtPlayer && collisionTeam == null)
        {
            return;
        }
        BuildIdentifier collisionBID = other.gameObject.GetComponent<BuildIdentifier>();


        if (!isBouncy)
        {
            if (isServer)
            {

                CmdHideProj();
            }

        }
        
        // show the explosion particle effect
        GameObject tempHitEffect = Instantiate(hitEffect, gameObject.transform.position, Quaternion.LookRotation(gameObject.transform.forward, Vector3.up));
        Destroy(tempHitEffect, 0.3f);

        if (!isServer)
            return;
        Target collisionTarget;
        bool hasParent = other.gameObject.transform.parent != null;
        if (hasParent)
        {
            collisionTarget = other.gameObject.transform.parent.GetComponent<Target>();
        }
        else
        {
            collisionTarget = other.gameObject.GetComponent<Target>();
        }
        // if the projectile isn't lethal or it hit something that isn't a target, leave

        if (!canKill || collisionTarget == null)
            return;

        collisionTarget.DamageTarget(firingPlayerName, firingTeam, firingGun, damage, 1f, collisionBID != null && collisionBID.team == firingTeam);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Projectile" && !persistent)
        {
            return;
        }
        else if (collision.gameObject.tag == "Projectile" && collision.gameObject.GetComponent<ProjectileController>().persistent)
        {
            return;
        }
        else if (collision.gameObject.tag == "Projectile" && collision.gameObject.GetComponent<ProjectileController>().isLive)
        {
            if (isServer)
            {
                CmdHideProj();
            }
            return;
        }
        else if ((collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Building") && persistent && !positionLocked)
        {
            Ray lowerRay = new Ray(transform.position, -Vector3.up);
            RaycastHit lowerRayhit;

            if (Physics.Raycast(lowerRay, out lowerRayhit, 0.3f))
            {
                if (lowerRayhit.transform.gameObject.tag == "Ground" || lowerRayhit.transform.gameObject.tag == "Building")
                {
                    rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
                    positionLocked = true;
                }
            }
        }
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
            collision.gameObject.tag == "BlueSpawnCore" && firingTeam == 1 ||
            onlyHurtPlayer && collisionTeam == null)
        {
            return;
        }
        BuildIdentifier collisionBID = collision.gameObject.GetComponent<BuildIdentifier>();

        
        if (!isBouncy)
        {
            if (isServer)
            {
                
                CmdHideProj();
            }

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

    [Command]
    public void CmdHideProj()
    {
        // the projectile is going to explode and is no longer live
        isLive = false;
        // hide the projectile body
        GetComponent<MeshRenderer>().enabled = false;
        RpcHideProj();
        if (persistent)
        {
            Destroy(gameObject);
        }
    }

    [ClientRpc]
    public void RpcHideProj()
    {
        // the projectile is going to explode and is no longer live
        isLive = false;
        // hide the projectile body
        GetComponent<MeshRenderer>().enabled = false;
        if (persistent)
        {
            Destroy(gameObject);
        }
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
