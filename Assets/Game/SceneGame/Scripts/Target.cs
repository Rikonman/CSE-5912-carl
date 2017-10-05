using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Target : NetworkBehaviour {

    [SerializeField]
    public float startingHealth = 100f;

    [SyncVar(hook = "OnCurrentHealthChange")]
    public float currentHealth;
	public bool isVulnerable = true;
    public Vector3 startingPos;
    public int respawnTime = 5;
    public float timer;
    [SyncVar]
    private bool _isDead;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }
    
    public RectTransform healthbar;

    void Start()
    {
        currentHealth = startingHealth;

        startingPos = transform.position;

        isDead = false;

        //disableOnDeath[0] = rend;

        //healthbar.sizeDelta = new Vector2(health * 2, healthbar.sizeDelta.y);
    }

    private void Update()
    {
        if (isDead)
        {
            timer += Time.deltaTime;
        }

        if(timer>=respawnTime){
            RpcRespawn();
        }
    }

    public void TakeDamage(float damage) {

        if (isDead)
            return;

        if (isVulnerable)
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
        isDead = true;

        //Renderer mesh = GetComponent<Renderer>();
        //if (mesh != null)
        //{
        //    mesh.enabled = false;
        //}
    }

    [ClientRpc]
    private void RpcRespawn()
    {
        //Renderer mesh = GetComponent<Renderer>();
        //if (mesh != null)
        //{
        //    mesh.enabled = true;
        //}
        isDead = false;
        timer = 0;

        CmdSetHealth(startingHealth);
        transform.position = startingPos;
    }

    [Command]
    void CmdSetHealth(float newHealth)
    {
        currentHealth = newHealth;
    }

}
