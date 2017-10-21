using Prototype.NetworkLobby;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Target : NetworkBehaviour {

    public float startingHealth = 100f;
    [SyncVar(hook = "OnCurrentHealthChange")]
    public float currentHealth;
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
	MeshFilter tempMesh; 
	MeshFilter mesh; 
    //Renderer rend;
    CapsuleCollider col;
    Rigidbody rb;
    PlayerTeam team;
    public RectTransform healthbar;

    void Start()
    {
        currentHealth = startingHealth;

        isDead = false;
        //rend = transform.GetComponent<Renderer>();
        col = transform.GetComponent<CapsuleCollider>();
        rb = transform.GetComponent<Rigidbody>();
		mesh = transform.GetComponent<MeshFilter>();
		tempMesh = mesh; 
        team = GetComponent<PlayerTeam>();
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
        if (isVulnerable && !isDead)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                Die();
            }
            Debug.Log("Hit: " + currentHealth);
        }
        else
        {
            Debug.Log("This object is invulnerable");
        }
    }

    private void OnCurrentHealthChange(float newHealth)
    {
        currentHealth = newHealth;
        healthbar.sizeDelta = new Vector2(newHealth * 2, healthbar.sizeDelta.y);
    }

    public void Die() {
		tempMesh = mesh; 
        isDead = true;
        //rend.enabled = false;
        col.enabled = false;
        rb.useGravity = false;
    }

    private void Respawn()
    {
		mesh = tempMesh;
        isDead = false;
        team.RpcChangeLocation(LobbyManager.s_Singleton.GetSpawnLocation(team.team));
        //rend.enabled = true;
        col.enabled = true;
        rb.useGravity = true;
        currentHealth = startingHealth;
        isDead = false;
        timer = 0;
    }
}
