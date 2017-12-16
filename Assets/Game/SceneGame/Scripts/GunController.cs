﻿using System.Collections;
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
    public bool minigun = false;
    public bool larpa = false;
    public bool gauss = false;
    public bool cluster = false;
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
    GameObject larpaPrefab;
    [SerializeField]
    GameObject gaussPrefab;
    [SerializeField]
    GameObject clusterPrefab;
    [SerializeField]
    Transform barrellExit;
    public PlayerTeam team;
    public bool locked;
    Rigidbody rb;
    CapsuleCollider cc;
    public PlayerLobbyInfo pli;
    public Target playerTarget;
    playerAnimation pa;
    public GameObject mainCamera;

    public AudioSource pistolShot;
    public AudioSource assaultOneShot;
    public AudioSource assaultTwoShot;
    public AudioSource assaultThreeShot;
    public AudioSource shotgunShot;
    public AudioSource sniperShot;
    public AudioSource rocketShot;
    public AudioSource minigunShot;
    public AudioSource larpaShot;
    public AudioSource gaussShot;
    public AudioSource clusterShot;
    public int assaultCounter;
    void Start()
    {
        assaultCounter = 0;
        locked = false;
        ResetAmmo(true);
        //gun = transform.GetChild(0).GetChild(0).gameObject;
        team = GetComponent<PlayerTeam>();
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
        pli = GetComponent<PlayerLobbyInfo>();
        playerTarget = GetComponent<Target>();
        pa = GetComponent<playerAnimation>();
        CmdSwitch(0);
    }

    [Command]
    public void CmdResetAmmo(bool refillMag)
    {
        ResetAmmo(refillMag);
        RpcResetAmmo(refillMag);
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

        if (minigun && (!Input.GetButton("Fire1") || currentAmmoInMag == 0) && minigunShot.isPlaying)
        {
            CmdStopMinigun();
        }
        if (!locked)
        {
            if (((automatic || minigun) && Input.GetButton("Fire1") || !(automatic || minigun) && Input.GetButtonDown("Fire1")) && Time.time >= fireDelay && !playerTarget._isDead)
            {
                fireDelay = Time.time + fireRate;
                Debug.Log(fireDelay - Time.time);
                bool ammoInMag = currentAmmoInMag > 0;
                Shoot();
                if (minigun && ammoInMag)
                {
                    rb.AddForce(-fpsCamera.transform.forward * 35, ForceMode.Impulse);
                    if (!minigunShot.isPlaying)
                    {
                        CmdStartMinigun();
                    }
                }
                if (gauss && ammoInMag)
                {
                    rb.AddForce(-fpsCamera.transform.forward * 300, ForceMode.Impulse);
                }
                if (currentAmmoInReserve > 0 && currentAmmoInMag == 0)
                {
                    StartCoroutine(ReloadDelayer());
                }
            }

            if (currentAmmoInReserve > 0 && currentAmmoInMag != maxAmmoInMag && Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(ReloadDelayer());
            }
        }
    }

    public IEnumerator ReloadDelayer()
    {
        locked = true;
        float remainingTime = 2f;

        while (remainingTime > 0)
        {
            yield return null;

            remainingTime -= Time.deltaTime;

        }
        if (currentAmmoInReserve >= maxAmmoInMag - currentAmmoInMag)
        {
            currentAmmoInReserve -= (maxAmmoInMag - currentAmmoInMag);
            currentAmmoInMag = maxAmmoInMag;

        }
        else if (currentAmmoInReserve < maxAmmoInMag - currentAmmoInMag)
        {
            currentAmmoInMag += currentAmmoInReserve;
            currentAmmoInReserve = 0;
        }
        locked = false;
    }

    [Command]
    public void CmdStartMinigun()
    {
        minigunShot.Play();
        RpcStartMinigun();
    }

    [ClientRpc]
    public void RpcStartMinigun()
    {
        minigunShot.Play();
    }

    
    [Command]
    public void CmdStopMinigun()
    {
        minigunShot.Stop();
        RpcStopMinigun();
    }

    [ClientRpc]
    public void RpcStopMinigun()
    {
        minigunShot.Stop();
    }

    [Command]
    public void CmdPlayGunshot(int gunChoice)
    {
        PlayGunshot(gunChoice);
        RpcPlayGunshot(gunChoice);
    }

    [ClientRpc]
    public void RpcPlayGunshot(int gunChoice)
    {
        PlayGunshot(gunChoice);
    }

    public void PlayGunshot(int gunChoice)
    {
        if (gunChoice == 0)
        {
            pistolShot.Play();
        }
        else if (gunChoice == 1)
        {
            if (assaultCounter == 0)
            {
                assaultOneShot.Play();
                assaultCounter++;
            }
            else if (assaultCounter == 1)
            {
                assaultTwoShot.Play();
                assaultCounter++;
            }
            else
            {
                assaultThreeShot.Play();
                assaultCounter = 0;
            }
        }
        else if (gunChoice == 2)
        {
            shotgunShot.Play();
        }
        else if (gunChoice == 3)
        {
            sniperShot.Play();
        }
        else if (gunChoice == 4)
        {
            rocketShot.Play();
        }
        else if (gunChoice == 6)
        {
            larpaShot.Play();
        }
        else if (gunChoice == 7)
        {
            gaussShot.Play();
        }
        else if (gunChoice == 8)
        {
            clusterShot.Play();
        }
    }

    void Shoot() {
        if (currentAmmoInMag > 0)
        {
            flash.Play();
            if (currentGun != 5)
            {
                CmdPlayGunshot(currentGun);
            }
            if (currentGun != 0 && currentGun != 1 && currentGun != 3)
            {
                Vector3 exit = barrellExit.position;
                if (currentGun == 6)
                {
                    exit = barrellExit.position + (fpsCamera.transform.forward.normalized * (barrellExit.position - transform.position).magnitude / 2f);
                }
                Vector3 forward = fpsCamera.transform.forward;
                if (currentGun == 5)
                {
                    forward = Quaternion.Euler(new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), Random.Range(-2f, 2f))) * forward;
                }
                CmdSpawnProjectile(team.team, team.playerID, damage, currentGun, range, pli.playerName, exit,
                    fpsCamera.transform.rotation, forward);
            }
            else
            {
                RaycastHit hit;
                Vector3 forward = fpsCamera.transform.forward;
                if (currentGun == 0 || currentGun == 1)
                {
                    forward = Quaternion.Euler(new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), Random.Range(-2f, 2f))) * forward;
                }
                if (Physics.Raycast(fpsCamera.transform.position, forward, out hit, range))
                {
                    Debug.Log(hit.transform.name);

                    Target target = hit.transform.GetComponent<Target>();
                    if (target != null)
                    {
                        if (target.team != null && target.team.team != team.team || target.bid != null || 
                            target.team == null && target.bid == null && 
                            (team.team == 0 && target.gameObject.tag == "BlueSpawnCore" || team.team == 1 && target.gameObject.tag == "RedSpawnCore"))
                        {
                            CmdDoRaycastHit(target.gameObject.GetComponent<NetworkIdentity>().netId, pli.playerName, team.team, currentGun, damage, target.bid != null && target.bid.team == team.team);
                            if (hit.rigidbody != null)
                            {
                                hit.rigidbody.AddForce(-hit.normal * force);
                            }
                        }
                    }
                    StartCoroutine(FireDelayer(.1f));
                    rb.AddForce(-fpsCamera.transform.forward * 5, ForceMode.Impulse);
                    GameObject tempHitEffect = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(tempHitEffect, 0.3f);
                }

            }
            currentAmmoInMag--;
        }
        else
        {
            // Play empty mag sound here
        }
        
    }

    public IEnumerator FireDelayer(float rate)
    {
        float remainingTime = rate;

        while (remainingTime > 0)
        {
            yield return null;

            remainingTime -= Time.deltaTime;
            pa.MoveXRotation(remainingTime > .025f ? 3f / rate * Time.deltaTime : -3f / rate * Time.deltaTime);
        }
    }

    [Command]
    public void CmdDoRaycastHit(NetworkInstanceId netid, string playerName, int teamID, int gunChoice, float inDamage, bool isFriendlyFire)
    {
        GameObject hitObject = ClientScene.FindLocalObject(netid);
        Target target = hitObject.GetComponent<Target>();
        target.DamageTarget(pli.playerName, team.team, currentGun, inDamage, 1f, isFriendlyFire);
    }

    // This command is called from the localPlayer and run on the server. Note that Commands must begin with 'Cmd'
    [Command]
    void CmdSpawnProjectile(int team, int playerID, float damage, int gunChoice, float rangeModifier, string playerName, Vector3 position, Quaternion rotation, Vector3 forward)
    {
        if (gunChoice == 2)
        {
            for (int counter = 0; counter < 8; counter++)
            {
                Vector3 newForward = Quaternion.Euler(new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f), Random.Range(-20f, 20f))) * forward;

                GameObject instance = Instantiate(projectilePrefab, position + newForward.normalized / 2f, rotation);
                instance.GetComponent<Rigidbody>().AddForce(newForward * rangeModifier/* + changeVector * variation*/);

                ProjectileController pc = instance.GetComponent<ProjectileController>();
                pc.firingTeam = team;
                pc.firingPlayer = playerID;
                pc.firingPlayerName = playerName;
                pc.damage = damage;
                pc.firingGun = gunChoice;
                NetworkServer.Spawn(instance);
                RpcUpdateProjectileData(instance.GetComponent<NetworkIdentity>().netId, team, playerID, damage, gunChoice, pc.projectileLifetime, playerName);
            }
        }
        else
        {
            Quaternion trueRot = rotation;
            if (gunChoice == 8)
            {
                trueRot = trueRot * Quaternion.Euler(new Vector3(0f, -90f, 0f));
            }
            GameObject instance = Instantiate(gunChoice == 4 ? rocketPrefab : (gunChoice == 6 ? larpaPrefab : 
                (gunChoice == 7 ? gaussPrefab : (gunChoice == 8 ? clusterPrefab : projectilePrefab))), position, trueRot);
            instance.GetComponent<Rigidbody>().AddForce(forward * rangeModifier);
            ProjectileController pc = instance.GetComponent<ProjectileController>();
            pc.firingTeam = team;
            pc.firingPlayer = playerID;
            pc.firingPlayerName = playerName;
            pc.damage = damage;
            pc.firingGun = gunChoice;
            if (gunChoice == 6)
            {
                pc.projectileLifetime = 5f;
            }
            else if (gunChoice == 4)
            {
                pc.projectileLifetime = 4f;
            }
            else if (gunChoice == 7)
            {
                pc.projectileLifetime = 1.6f;
            }
            NetworkServer.Spawn(instance);
            RpcUpdateProjectileData(instance.GetComponent<NetworkIdentity>().netId, team, playerID, damage, gunChoice, pc.projectileLifetime, playerName);
        }
    }

    [ClientRpc]
    public void RpcUpdateProjectileData(NetworkInstanceId nid, int team, int playerID, float damage, int gunChoice, float projectileLifetime, string playerName)
    {
        GameObject projectile = ClientScene.FindLocalObject(nid);
        if  (projectile != null)
        {
            ProjectileController pc = projectile.GetComponent<ProjectileController>();
            pc.firingTeam = team;
            pc.firingPlayer = playerID;
            pc.firingPlayerName = playerName;
            pc.damage = damage;
            pc.firingGun = gunChoice;
            pc.projectileLifetime = projectileLifetime;
        }
        
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
        minigun = gunIndex == 5;
        larpa = gunIndex == 6;
        gauss = gunIndex == 7;
        cluster = gunIndex == 8;
        currentGun = gunIndex;
        gun.transform.GetChild(currentGun).gameObject.SetActive(true);
        barrellExit = gun.transform.GetChild(currentGun).GetChild(0);

        if (shotgun)
        {
            damage = 15;
            maxAmmoInMag = 5;
            startingReserveAmmo = 30;
            fireRate = 1f;
            range = 3000f;
            currentAmmoInMag = maxAmmoInMag;
            currentAmmoInReserve = startingReserveAmmo;
        }
        else if (automatic)
        {
            damage = 10;
            maxAmmoInMag = 30;
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
            damage = 60;
            maxAmmoInMag = 4;
            startingReserveAmmo = 20;
            fireRate = 1f;
            range = 2000f;
            currentAmmoInMag = maxAmmoInMag;
            currentAmmoInReserve = startingReserveAmmo;
        }
        else if (minigun)
        {
            damage = 10;
            maxAmmoInMag = 60;
            startingReserveAmmo = 180;
            fireRate = .05f;
            range = 8000f;
            currentAmmoInMag = maxAmmoInMag;
            currentAmmoInReserve = startingReserveAmmo;
        }
        else if (larpa)
        {
            damage = 30;
            maxAmmoInMag = 2;
            startingReserveAmmo = 10;
            fireRate = 1f;
            range = 600f;
            currentAmmoInMag = maxAmmoInMag;
            currentAmmoInReserve = startingReserveAmmo;
        }
        else if (gauss)
        {
            damage = 60;
            maxAmmoInMag = 1;
            startingReserveAmmo = 15;
            fireRate = 1f;
            range = 8000f;
            currentAmmoInMag = maxAmmoInMag;
            currentAmmoInReserve = startingReserveAmmo;
        }
        else if (cluster)
        {
            damage = 30;
            maxAmmoInMag = 3;
            startingReserveAmmo = 15;
            fireRate = 1f;
            range = 1200f;
            currentAmmoInMag = maxAmmoInMag;
            currentAmmoInReserve = startingReserveAmmo;
        }
        else
        {
            damage = 20;
            maxAmmoInMag = 20;
            startingReserveAmmo = 50;
            fireRate = .5f;
            range = 2000f;
            currentAmmoInMag = maxAmmoInMag;
            currentAmmoInReserve = startingReserveAmmo;
        }
    }
}
