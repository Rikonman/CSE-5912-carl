using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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
    Rigidbody rb;
    public RectTransform healthbar;

    void Start()
    {
        health = startingHealth;

        startingPos = GameObject.Find("SpawnPoint").transform.position;

        isDead = false;
        rend = transform.GetComponent<Renderer>();
        col = transform.GetComponent<CapsuleCollider>();
        rb = transform.GetComponent<Rigidbody>();
        //healthbar.sizeDelta = new Vector2(health * 2, healthbar.sizeDelta.y);
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
            Debug.Log("Hit: " + health);
            healthbar.sizeDelta = new Vector2(health * 2, healthbar.sizeDelta.y);
        } else {
			Debug.Log ("This object is invulnerable"); 
		}
    }

    private void Die() {
        isDead = true;
        rend.enabled = false;
        col.enabled = false;
        rb.useGravity = false;
    }

    private void Respawn()
    {
        isDead = false;
        transform.position = startingPos;
        rend.enabled = true;
        col.enabled = true;
        rb.useGravity = true;
        health = startingHealth;
        isDead = false;
        timer = 0;
    }
}
