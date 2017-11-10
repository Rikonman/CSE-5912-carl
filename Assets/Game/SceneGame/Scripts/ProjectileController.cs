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
    public int firingTeam;
    public int firingPlayer;

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
        if (collision.gameObject.transform.parent != null)
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

        // have the target take damage
        collisionTarget.TakeDamage(25);
		
		//explode the dead
		/*if(collisionTarget._isDead)
		{
			collision.gameObject.AddComponent<TriangleExplosion>();
			StartCoroutine(collision.gameObject.GetComponent<TriangleExplosion>().SplitMesh(true));
		}*/

    }
}
