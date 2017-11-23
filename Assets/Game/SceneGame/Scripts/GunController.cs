using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GunController : NetworkBehaviour {

    public float damage = 1f;
    public float range = 2000f;
    public Camera fpsCamera;
    public ParticleSystem flash;
    public GameObject hitEffect;
    public GameObject gun;
    public float force = 60f;
    public float fireRate = .1f;
    public float fireDelay = 0f;
    public bool automatic = false;
    public bool shotgun = false;
    public bool sniper = false;
    public bool rockets = false;
    public float speedModifier = 1f;

    //=============================
    // Ammunition stuffs
    //=============================
    public int maxAmmoInMag = 20;
    public int startingReserveAmmo = 50;
    public int currentAmmoInMag;
    public int currentAmmoInReserve;
    public int currentGun=0;
    public int numberOfGuns = 2;

    [SerializeField]
    GameObject projectilePrefab;
    [SerializeField]
    GameObject rocketPrefab;
    [SerializeField]
    Transform barrellExit;
    public AudioSource gunshot;
    public PlayerTeam team;
    public bool locked;

    void Start()
    {
        locked = false;
        ResetAmmo(true);
        //gun = transform.GetChild(0).GetChild(0).gameObject;
        team = GetComponent<PlayerTeam>();
        CmdSwitch(0);
    }

    [ClientRpc]
    public void RpcResetAmmo(bool refillMag)
    {
        ResetAmmo(refillMag);
    }

    public void ResetAmmo(bool refillMag)
    {
        currentAmmoInReserve = startingReserveAmmo;
        if (currentAmmoInReserve >= maxAmmoInMag)
        {
            if (refillMag)
            {
                currentAmmoInMag = maxAmmoInMag;
                currentAmmoInReserve -= maxAmmoInMag;
            }
            
        }
        else
        {
            if (refillMag)
            {
                currentAmmoInMag = currentAmmoInReserve;
                currentAmmoInReserve = 0;
            }
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
                    fireDelay = Time.time + (fireRate/2f);
                    Debug.Log(fireDelay - Time.time);
                    Shoot();
                }
            }
            else
            {
                if (Input.GetButtonDown("Fire1") && Time.time >= fireDelay)
                {
                    fireDelay = Time.time + fireRate;
                    Debug.Log(fireDelay - Time.time);
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
            CmdSpawnProjectile(team.team, team.playerID, damage, currentGun, range, barrellExit.position, fpsCamera.transform.rotation, fpsCamera.transform.forward);
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
    void CmdSpawnProjectile(int team, int playerID, float damage, int gunChoice, float rangeModifier, Vector3 position, Quaternion rotation, Vector3 forward)
    {
        if (gunChoice == 2)
        {
            for (int counter = 0; counter < 8; counter++)
            {
                Vector3 changeVector;
                float val1 = Random.Range(.5f, 2f);
                float val2 = Random.Range(.5f, 2f);
                float variation = Random.Range(200f, 1000f);
                int choice = Random.Range(0, 3);
                if (choice == 0 && forward.z != 0f)
                {
                    float val3 = (-val1 * forward.x - val2 * forward.y) / forward.z;
                    changeVector = new Vector3(val1, val2, val3);
                    changeVector.Normalize();
                }
                else if ((choice == 1 || forward.z == 0f) && forward.y != 0f)
                {
                    float val3 = (-val1 * forward.x - val2 * forward.z) / forward.y;
                    changeVector = new Vector3(val1, val3, val2);
                    changeVector.Normalize();
                }
                else if ((choice == 1 || forward.z == 0f || forward.y == 0f) && forward.x != 0f)
                {
                    float val3 = (-val1 * forward.z - val2 * forward.y) / forward.x;
                    changeVector = new Vector3(val3, val2, val1);
                    changeVector.Normalize();
                }
                else
                {
                    changeVector = forward;
                    changeVector.Normalize();
                }
                GameObject instance = Instantiate(projectilePrefab, position, rotation);
                instance.GetComponent<Rigidbody>().AddForce(forward * rangeModifier + changeVector * variation);

                ProjectileController pc = instance.GetComponent<ProjectileController>();
                pc.firingTeam = team;
                pc.firingPlayer = playerID;
                pc.damage = damage;
                NetworkServer.Spawn(instance);
                RpcUpdateProjectileData(instance.GetComponent<NetworkIdentity>().netId, team, playerID, damage, pc.projectileLifetime);
            }
        }
        else
        {
            GameObject instance = Instantiate(gunChoice == 4 ? rocketPrefab : projectilePrefab, position, rotation);
            instance.GetComponent<Rigidbody>().AddForce(forward * rangeModifier);
            ProjectileController pc = instance.GetComponent<ProjectileController>();
            pc.firingTeam = team;
            pc.firingPlayer = playerID;
            pc.damage = damage;
            if (gunChoice == 3)
            {
                pc.projectileLifetime = 4f;
            }
            NetworkServer.Spawn(instance);
            RpcUpdateProjectileData(instance.GetComponent<NetworkIdentity>().netId, team, playerID, damage, pc.projectileLifetime);
        }
    }

    [ClientRpc]
    public void RpcUpdateProjectileData(NetworkInstanceId nid, int team, int playerID, float damage, float projectileLifetime)
    {
        GameObject projectile = ClientScene.FindLocalObject(nid);
        ProjectileController pc = projectile.GetComponent<ProjectileController>();
        pc.firingTeam = team;
        pc.firingPlayer = playerID;
        pc.damage = damage;
        pc.projectileLifetime = projectileLifetime;
    }

    [Command]
    public void CmdSwitch(int gunIndex)
    {

        Switch(gunIndex);
        RpcSwitch(gunIndex);

    }

    [ClientRpc]
    void RpcSwitch(int gunIndex)
    {

        Switch(gunIndex);

    }

    void Switch(int gunIndex)
    {
        gun.transform.GetChild(currentGun).gameObject.SetActive(false);
        automatic = gunIndex == 1;
        shotgun = gunIndex == 2;
        sniper = gunIndex == 3;
        rockets = gunIndex == 4;
        currentGun = gunIndex;
        gun.transform.GetChild(currentGun).gameObject.SetActive(true);
        barrellExit = gun.transform.GetChild(currentGun).GetChild(0);

        if (shotgun)
        {
            damage = 10;
            maxAmmoInMag = 5;
            startingReserveAmmo = 30;
            fireRate = 1f;
            range = 2000f;
            currentAmmoInMag = maxAmmoInMag;
            currentAmmoInReserve = startingReserveAmmo;
        }
        else if (automatic)
        {
            damage = 10;
            maxAmmoInMag = 50;
            startingReserveAmmo = 150;
            fireRate = .1f;
            range = 2000f;
            currentAmmoInMag = maxAmmoInMag;
            currentAmmoInReserve = startingReserveAmmo;
        }
        else if (sniper)
        {
            damage = 60;
            maxAmmoInMag = 1;
            startingReserveAmmo = 15;
            fireRate = 1f;
            range = 3000f;
            currentAmmoInMag = maxAmmoInMag;
            currentAmmoInReserve = startingReserveAmmo;
        }
        else if (rockets)
        {
            damage = 40;
            maxAmmoInMag = 4;
            startingReserveAmmo = 20;
            fireRate = 1f;
            range = 1000f;
            currentAmmoInMag = maxAmmoInMag;
            currentAmmoInReserve = startingReserveAmmo;
        }
        else
        {
            damage = 20;
            maxAmmoInMag = 20;
            startingReserveAmmo = 50;
            fireRate = .1f;
            range = 2000f;
            currentAmmoInMag = maxAmmoInMag;
            currentAmmoInReserve = startingReserveAmmo;
        }
    }
}
