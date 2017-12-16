using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GunController : NetworkBehaviour {
    
    public Camera fpsCamera;
    public ParticleSystem flash;
    public GameObject hitEffect;
    public GameObject gun;
    public float force = 60f;
    public float fireDelay = 0f;
    public bool automatic = false;
    public bool shotgun = false;
    public bool sniper = false;
    public bool rockets = false;
    public bool minigun = false;
    public bool larpa = false;
    public bool gauss = false;
    public bool cluster = false;
    

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
    public GunStats[] guns = { new GunStats(-1), new GunStats(-1), new GunStats(0) };
    int selectedGun = 2;
    public GunStats currentGun
    {
        get
        {
            return guns[selectedGun];
        }
    }

    public bool CanBuy
    {
        get
        {
            return guns[1].gunIndex == -1;
        }
    }

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
        currentGun.ResetAmmo(true);
        //gun = transform.GetChild(0).GetChild(0).gameObject;
        team = GetComponent<PlayerTeam>();
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
        pli = GetComponent<PlayerLobbyInfo>();
        playerTarget = GetComponent<Target>();
        pa = GetComponent<playerAnimation>();
        Switch(selectedGun);
        ResetSlots();
    }

    [ClientRpc]
    public void RpcResetAllAmmo(bool refillMag)
    {
        guns[0].ResetAmmo(refillMag);
        guns[1].ResetAmmo(refillMag);
        guns[2].ResetAmmo(refillMag);
    }

    [ClientRpc]
    public void RpcResetAmmo(bool refillMag)
    {
        currentGun.ResetAmmo(refillMag);
    }

    [Command]
    public void CmdResetGuns()
    {
        RpcResetGuns();
    }

    [ClientRpc]
    public void RpcResetGuns()
    {
        Switch(2);
        guns[0] = new GunStats(-1);
        guns[1] = new GunStats(-1);
        guns[2].ResetAmmo(true);
        ResetSlots();
    }

    public void ResetSlots()
    {
        if (isLocalPlayer)
        {
            GameObject weaponDisplay = GameObject.Find("WeaponDisplay");
            WeaponDisplay weaps = weaponDisplay.GetComponent<WeaponDisplay>();
            weaps.ChangeSlot(0, -1);
            weaps.ChangeSlot(1, -1);
            weaps.ChangeSlot(2, 0);
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

        if (minigun && (!Input.GetButton("Fire1") || currentGun.currentAmmoInMag == 0) && minigunShot.isPlaying)
        {
            CmdStopMinigun();
        }
        if (!locked)
        {
            if (((automatic || minigun) && Input.GetButton("Fire1") || !(automatic || minigun) && Input.GetButtonDown("Fire1")) && Time.time >= fireDelay && !playerTarget._isDead)
            {
                fireDelay = Time.time + currentGun.fireRate;
                Debug.Log(fireDelay - Time.time);
                bool ammoInMag = currentGun.currentAmmoInMag > 0;
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
                if (currentGun.currentAmmoInReserve > 0 && currentGun.currentAmmoInMag == 0)
                {
                    reloadDelayer = ReloadDelayer();
                    StartCoroutine(reloadDelayer);
                }
            }

            if (currentGun.currentAmmoInReserve > 0 && currentGun.currentAmmoInMag != currentGun.maxAmmoInMag && Input.GetKeyDown(KeyCode.R))
            {
                reloadDelayer = ReloadDelayer();
                StartCoroutine(reloadDelayer);
            }
        }
    }

    private IEnumerator reloadDelayer;

    public IEnumerator ReloadDelayer()
    {
        locked = true;
        float remainingTime = 2f;

        while (remainingTime > 0)
        {
            yield return null;

            remainingTime -= Time.deltaTime;

        }
        if (currentGun.currentAmmoInReserve >= currentGun.maxAmmoInMag - currentGun.currentAmmoInMag)
        {
            currentGun.currentAmmoInReserve -= (currentGun.maxAmmoInMag - currentGun.currentAmmoInMag);
            currentGun.currentAmmoInMag = currentGun.maxAmmoInMag;

        }
        else if (currentGun.currentAmmoInReserve < currentGun.maxAmmoInMag - currentGun.currentAmmoInMag)
        {
            currentGun.currentAmmoInMag += currentGun.currentAmmoInReserve;
            currentGun.currentAmmoInReserve = 0;
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
        if (currentGun.currentAmmoInMag > 0)
        {
            flash.Play();
            if (currentGun.gunIndex != 5)
            {
                CmdPlayGunshot(currentGun.gunIndex);
            }
            if (currentGun.gunIndex != 0 && currentGun.gunIndex != 1 && currentGun.gunIndex != 3)
            {
                Vector3 exit = barrellExit.position;
                if (currentGun.gunIndex == 6)
                {
                    exit = barrellExit.position + (fpsCamera.transform.forward.normalized * (barrellExit.position - transform.position).magnitude / 2f);
                }
                Vector3 forward = fpsCamera.transform.forward;
                if (currentGun.gunIndex == 5)
                {
                    forward = Quaternion.Euler(new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), Random.Range(-2f, 2f))) * forward;
                }
                CmdSpawnProjectile(team.team, team.playerID, currentGun.damage, currentGun.gunIndex, currentGun.range, pli.playerName, exit,
                    fpsCamera.transform.rotation, forward);
            }
            else
            {
                RaycastHit hit;
                Vector3 forward = fpsCamera.transform.forward;
                if (currentGun.gunIndex == 0 || currentGun.gunIndex == 1)
                {
                    forward = Quaternion.Euler(new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), Random.Range(-2f, 2f))) * forward;
                }
                if (Physics.Raycast(fpsCamera.transform.position, forward, out hit, currentGun.range))
                {
                    Debug.Log(hit.transform.name);

                    Target target = hit.transform.GetComponent<Target>();
                    if (target != null)
                    {
                        if (target.team != null && target.team.team != team.team || target.bid != null || 
                            target.team == null && target.bid == null && 
                            (team.team == 0 && target.gameObject.tag == "BlueSpawnCore" || team.team == 1 && target.gameObject.tag == "RedSpawnCore"))
                        {
                            CmdDoRaycastHit(target.gameObject.GetComponent<NetworkIdentity>().netId, pli.playerName, team.team,
                                currentGun.gunIndex, currentGun.damage, target.bid != null && target.bid.team == team.team);
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
            currentGun.currentAmmoInMag--;
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
        target.DamageTarget(pli.playerName, team.team, currentGun.gunIndex, inDamage, 1f, isFriendlyFire);
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
        if (projectile != null)
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
    public void CmdSwitch(int slotIndex)
    {
        Switch(slotIndex);
        RpcSwitch(slotIndex);
    }

    [ClientRpc]
    void RpcSwitch(int slotIndex)
    {
        Switch(slotIndex);
    }

    public void Switch(int slotIndex)
    {
        if (reloadDelayer != null)
        {
            StopCoroutine(reloadDelayer);
            locked = false;
        }
        gun.transform.GetChild(currentGun.gunIndex).gameObject.SetActive(false);
        selectedGun = slotIndex;
        automatic = currentGun.gunIndex == 1;
        shotgun = currentGun.gunIndex == 2;
        sniper = currentGun.gunIndex == 3;
        rockets = currentGun.gunIndex == 4;
        minigun = currentGun.gunIndex == 5;
        larpa = currentGun.gunIndex == 6;
        gauss = currentGun.gunIndex == 7;
        cluster = currentGun.gunIndex == 8;
        gun.transform.GetChild(currentGun.gunIndex).gameObject.SetActive(true);
        barrellExit = gun.transform.GetChild(currentGun.gunIndex).GetChild(0);

    }
    

    public void Buy(int gunIndex)
    {
        if (guns[0].gunIndex == -1)
        {
            guns[0] = new GunStats(gunIndex);

            if (isLocalPlayer)
            {
                GameObject weaponDisplay = GameObject.Find("WeaponDisplay");
                WeaponDisplay weaps = weaponDisplay.GetComponent<WeaponDisplay>();
                weaps.ChangeSlot(0, gunIndex);
            }
            Switch(0);
        }
        else
        {
            guns[1] = new GunStats(gunIndex);
            if (isLocalPlayer)
            {
                GameObject weaponDisplay = GameObject.Find("WeaponDisplay");
                WeaponDisplay weaps = weaponDisplay.GetComponent<WeaponDisplay>();
                weaps.ChangeSlot(1, gunIndex);
            }
            Switch(1);
        }
    }
}
