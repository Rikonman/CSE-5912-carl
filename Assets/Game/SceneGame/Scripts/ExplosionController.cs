using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExplosionController : NetworkBehaviour {
    public ProjectileController pc;
    public SphereCollider sc;
    public CapsuleCollider cc;
    // Use this for initialization
    void Start () {
        pc = gameObject.GetComponent<ProjectileController>();
        sc = gameObject.GetComponentInChildren<SphereCollider>();
        cc = gameObject.GetComponentInChildren<CapsuleCollider>();
	}

    public void StartExplosion()
    {
        sc.enabled = true;
        StartCoroutine(Delayer());
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
        if (other.gameObject.tag != "Player" && other.gameObject.tag != "Building" && !hasParent || hasParent && 
            other.gameObject.transform.parent.gameObject.tag != "Player" && other.gameObject.transform.parent.gameObject.tag != "Building")
            return;

        PlayerTeam collisionTeam = other.gameObject.GetComponent<PlayerTeam>();
        if (collisionTeam != null && collisionTeam.team == pc.firingTeam ||
            other.gameObject.tag == "RedSpawnCore" && pc.firingTeam == 0 ||
            other.gameObject.tag == "BlueSpawnCore" && pc.firingTeam == 1)
        {
            return;
        }

        if (!isServer)
            return;

        if (collisionTarget == null)
        {
            return;
        }

        pc.DamageTarget(collisionTarget, collisionTeam == null, other.gameObject.tag, hasParent, hasParent ? 3f : 1f);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
