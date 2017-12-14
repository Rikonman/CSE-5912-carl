using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExplosionController : NetworkBehaviour {
    public ProjectileController pc;
    public SphereCollider sc;
    public CapsuleCollider cc;
    public ParticleSystem smokePS;
    public GameObject explosion;
    public AudioSource explosionSound;
    public int numEmitted;

    // Use this for initialization
    void Start () {
        pc = gameObject.GetComponent<ProjectileController>();
        sc = gameObject.GetComponent<SphereCollider>();
        cc = gameObject.GetComponent<CapsuleCollider>();
        smokePS = gameObject.GetComponentInChildren<ParticleSystem>();
    }

    public void StartExplosion()
    {
        sc.enabled = true;
        CmdStartExplosion();

        StartCoroutine(Delayer());
    }

    [Command]
    public void CmdStartExplosion()
    {
        if (smokePS != null)
        {
            smokePS.Stop(true);
        }
        explosion.SetActive(true);
        explosion.GetComponent<ParticleSystem>().Emit(numEmitted);
        RpcStartExplosion(gameObject.GetComponent<NetworkIdentity>().netId);
    }

    [ClientRpc]
    public void RpcStartExplosion(NetworkInstanceId nid)
    {
        GameObject rocket = ClientScene.FindLocalObject(nid);
        rocket.GetComponent<MeshRenderer>().enabled = false;
        ParticleSystem tempPS = rocket.GetComponent<ParticleSystem>();
        if (tempPS != null)
        {
            tempPS.Stop(true);
        }
        GameObject tempExp = rocket.transform.GetChild(0).gameObject;
        tempExp.SetActive(true);
        tempExp.GetComponent<ParticleSystem>().Emit(numEmitted);
        explosionSound.Play();
    }

    public IEnumerator Delayer()
    {
        float remainingTime = .1f;

        while (remainingTime > 0)
        {
            yield return null;

            remainingTime -= Time.deltaTime;

        }
        sc.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
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
        if (other.gameObject.tag != "Player" && other.gameObject.tag != "Building" && other.gameObject.tag != "RedSpawnCore" && other.gameObject.tag != "BlueSpawnCore" && !hasParent || 
            hasParent && other.gameObject.transform.parent.gameObject.tag != "Player" && other.gameObject.transform.parent.gameObject.tag != "Building" &&
            other.gameObject.transform.parent.gameObject.tag != "RedSpawnCore" && other.gameObject.transform.parent.gameObject.tag != "BlueSpawnCore")
            return;

        PlayerTeam collisionTeam = other.gameObject.GetComponent<PlayerTeam>();
        if (collisionTeam != null && collisionTeam.team == pc.firingTeam ||
            other.gameObject.tag == "RedSpawnCore" && pc.firingTeam == 0 ||
            other.gameObject.tag == "BlueSpawnCore" && pc.firingTeam == 1)
        {
            return;
        }

        BuildIdentifier collisionBID = other.gameObject.GetComponent<BuildIdentifier>();

        if (!isServer)
            return;

        if (collisionTarget == null)
        {
            return;
        }

        pc.DamageTarget(collisionTarget, collisionTeam == null, other.gameObject.tag, hasParent, hasParent ? 3f : 1f, collisionBID != null && collisionBID.team == pc.firingTeam);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
