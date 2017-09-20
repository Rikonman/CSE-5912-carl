using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Target : NetworkBehaviour {

    public float startingHealth = 100f;
    [SyncVar]
    public float health;
	public bool isVulnerable = true;
    public Vector3 startingPos;
    public int respawnTime = 5;
    public float timer;
    [SyncVar]
    private bool isDead;
    public bool _isDead
    {
        get { return isDead; }
        protected set { isDead = value; }
    }
    Renderer rend;
    CapsuleCollider col;

    void Start()
    {
        health = startingHealth;

        startingPos = GameObject.Find("SpawnPoint").transform.position;

        isDead = false;
        rend = transform.GetComponent<Renderer>();
        col = transform.GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        if (isDead)
        {
            timer += Time.deltaTime;
        }

        if(timer>=respawnTime){
            Respawn();
        }
    }

    public void TakeDamage(float damage) {

		if (isVulnerable && !isDead) {
			health -= damage;

			if (health <= 0) {
                Die();
            }
		} else {
			Debug.Log ("This object is invulnerable"); 
		}
    }

    private void Die() {
        isDead = true;
        rend.enabled = false;
        col.enabled = false;    
    }

    private void Respawn()
    {
        isDead = false;
        transform.position = startingPos;
        rend.enabled = true;
        col.enabled = true;
        health = startingHealth;
        isDead = false;
        timer = 0;
    }
}
