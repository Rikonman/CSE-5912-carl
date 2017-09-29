using UnityEngine;
using UnityEngine.Networking;

public class ProjectileController : NetworkBehaviour {

    [SerializeField]
    float projectileLifetime = 2f;
    [SerializeField]
    bool canKill = false;
    public GameObject hitEffect;

    bool isLive = true;
    float age;
    MeshRenderer projectileRenderer;

	// Use this for initialization
	void Start () {
        projectileRenderer = GetComponent<MeshRenderer>();
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

        // the projectile is going to explode and is no longer live
        isLive = false;

        // hide the projectile body
        projectileRenderer.enabled = false;
        // show the explosion particle effect
        GameObject tempHitEffect = Instantiate(hitEffect, gameObject.transform.position, Quaternion.LookRotation(gameObject.transform.forward, Vector3.up));
        Destroy(tempHitEffect, 0.3f);

        if (!isServer)
            return;

        // if the projectile isn't lethal or it hit something that isn't a target, leave
        Target collisionTarget;
        if (!canKill || (collisionTarget = collision.gameObject.GetComponent<Target>()) == null)
            return;

        // have the target take damage
        collisionTarget.TakeDamage(25);
    }
}
