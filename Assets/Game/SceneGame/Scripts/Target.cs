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
    public PlayerTeam team;
    public BuildIdentifier bi;
    public RectTransform healthbar;
    public EmperorController emperorScript;

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
        bi = GetComponent<BuildIdentifier>();

        try
        {
            emperorScript = GameObject.Find("Emperor").GetComponent<EmperorController>();
        }
        catch
        {
            StartCoroutine(EmperorDelayer());
        }
        //StartCoroutine(EmperorDelayer());
        //healthbar.sizeDelta = new Vector2(health * 2, healthbar.sizeDelta.y);
    }

    public IEnumerator EmperorDelayer()
    {
        float remainingTime = 1f;

        while (remainingTime > 0)
        {
            yield return null;

            remainingTime -= Time.deltaTime;

        }
        emperorScript = GameObject.Find("Emperor").GetComponent<EmperorController>();
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

    public bool TakeDamage(float damage) {
        if (isVulnerable && !isDead)
        {
            float priorHealth = currentHealth;
            currentHealth -= damage;
            if (team != null)
            {
                if (priorHealth > startingHealth * 3 / 4 && currentHealth <= startingHealth * 3 / 4)
                {
                    emperorScript.RpcAddEntertainment(1);
                }
                else if (priorHealth > startingHealth * 2 / 4 && currentHealth <= startingHealth * 2 / 4)
                {
                    emperorScript.RpcAddEntertainment(1);
                }
                else if (priorHealth > startingHealth / 4 && currentHealth <= startingHealth / 4)
                {
                    emperorScript.RpcAddEntertainment(1);
                }
            }
            if (currentHealth <= 0)
            {
                Die();
                return true;
            }
            Debug.Log("Hit: " + currentHealth);
        }
        else
        {
            Debug.Log("This object is invulnerable");
        }
        return false;
    }

    private void OnCurrentHealthChange(float newHealth)
    {
        currentHealth = newHealth;
        if (healthbar != null)
        {
            healthbar.sizeDelta = new Vector2(newHealth * 2, healthbar.sizeDelta.y);
        }
            
    }

    public void Die()
    {
        if (team != null)
        {
            emperorScript.RpcAddEntertainment(5);
            if (team.team == 0)
            {
                emperorScript.RpcAddBlueFavor(5);
            }
            else
            {
                emperorScript.RpcAddRedFavor(5);
            }
        }
        else
        {
            emperorScript.RpcAddEntertainment(3);
            if (bi.team == 0)
            {
                emperorScript.RpcAddBlueFavor(3);
            }
            else
            {
                emperorScript.RpcAddRedFavor(3);
            }
        }
        
		tempMesh = mesh;
        isDead = true;
        if (team != null)
        {
            col.enabled = false;
            rb.useGravity = false;

        }
        else
        {
            Destroy(gameObject);
            GameObject tempBase;
            if (bi.team == 0)
            {
                tempBase = GameObject.Find("Base1Center");
            }
            else
            {
                tempBase = GameObject.Find("Base2Center");
            }
            BaseBuildings tempBuilding = tempBase.GetComponent<BaseBuildings>();
            tempBuilding.CmdDestroyMountPoint(bi.parentMountPoint, bi.parentMountBool, bi.id, bi.team);
        }
        //rend.enabled = false;
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
