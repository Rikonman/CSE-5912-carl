using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GunController : NetworkBehaviour {

    public float damage = 1f;
    public float range = 1000f;
    public Camera fpsCamera;
    public ParticleSystem flash;
    public GameObject hitEffect;
    public GameObject gun;
    public float force = 60f;
    public float fireRate = .1f;
    public float fireDelay = 0f;
    public bool automatic = false;

    //=============================
    // Ammunition stuffs
    //=============================
    public int maxAmmoInMag = 20;
    public int startingReserveAmmo = 50;
    public int currentAmmoInMag;
    public int currentAmmoInReserve;

    [SerializeField]
    GameObject projectilePrefab;
    [SerializeField]
    Transform barrellExit;
    public AudioSource gunshot;
    public PlayerTeam team;
    public bool locked;

    void Start()
    {
        locked = false;
        ResetAmmo();
        //gun = transform.GetChild(0).GetChild(0).gameObject;
        team = GetComponent<PlayerTeam>();
    }

    [ClientRpc]
    public void RpcResetAmmo()
    {
        ResetAmmo();
    }

    public void ResetAmmo()
    {
        currentAmmoInReserve = startingReserveAmmo;
        if (currentAmmoInReserve >= maxAmmoInMag)
        {
            currentAmmoInMag = maxAmmoInMag;
            currentAmmoInReserve -= maxAmmoInMag;
        }
        else
        {
            currentAmmoInMag = currentAmmoInReserve;
            currentAmmoInReserve = 0;
        }

    }

    // the reset method lets us run slow code (like "Find") in the editor where performance
    // won't impact the players at runtime
    void Reset()
    {
        barrellExit = transform.Find("BarrellExit");
    }

    // Update is called once per frame
    void Update()
    {
        // Only the local player can fire a weapon

        if (!isLocalPlayer)
            return;

        if (!locked)
        {
            if (automatic)
            {
                if (Input.GetButton("Fire1") && Time.time >= fireDelay)
                {
                    fireDelay = Time.time + 1f / fireRate;
                    Shoot();
                }
            }
            else
            {
                if (Input.GetButtonDown("Fire1") && Time.time >= fireDelay)
                {
                    fireDelay = Time.time + fireRate;
                    Shoot();
                }
            }

            if (currentAmmoInReserve >= maxAmmoInMag - currentAmmoInMag && Input.GetKeyDown(KeyCode.R))
            {
                currentAmmoInReserve -= (maxAmmoInMag - currentAmmoInMag);
                currentAmmoInMag = maxAmmoInMag;

            }
            else if (currentAmmoInReserve < maxAmmoInMag - currentAmmoInMag && Input.GetKeyDown(KeyCode.R))
            {
                currentAmmoInMag += currentAmmoInReserve;
                currentAmmoInReserve = 0;
            }
        }
    }

    void Shoot() {
        if (currentAmmoInMag > 0)
        {
            flash.Play();
            gunshot.Play();
            gunshot.loop = false;
            CmdSpawnProjectile(team.team, team.playerID, barrellExit.position, barrellExit.rotation, barrellExit.forward);
            currentAmmoInMag--;
        }
        else
        {
            // Play empty mag sound here
        }
        
        /*RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * force);
            }

            GameObject tempHitEffect = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(tempHitEffect, 0.3f);
        }*/
    }

    // This command is called from the localPlayer and run on the server. Note that Commands must begin with 'Cmd'
    [Command]
    void CmdSpawnProjectile(int team, int playerID, Vector3 position, Quaternion rotation, Vector3 forward)
    {
        GameObject instance = Instantiate(projectilePrefab, position, rotation);
        instance.GetComponent<Rigidbody>().AddForce(forward * range);
        ProjectileController pc = instance.GetComponent<ProjectileController>();
        pc.firingTeam = team;
        pc.firingPlayer = playerID;
        NetworkServer.Spawn(instance);
    }
}
