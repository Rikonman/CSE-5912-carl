using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GunController : NetworkBehaviour {

    public float damage = 1f;
    public float range = 100f;
    public Camera fpsCamera;
    public ParticleSystem flash;
    public GameObject hitEffect;
    public float force = 60f;
    public float fireRate = .1f;
    public float fireDelay = 0f;
    public bool automatic = false;
    float xRotation;
    float yRotation;
    float lookSensitivity = 5f;

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

    void Start()
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
    void Update () {
        // Only the local player can fire a weapon
        if (!isLocalPlayer)
            return;

        
        if (automatic) {
            if (Input.GetButton("Fire1") && Time.time >= fireDelay)
            {
                fireDelay = Time.time + 1f / fireRate;
                Shoot();
            }
        } else {
            if (Input.GetButtonDown("Fire1") && Time.time >= fireDelay)
            {
                fireDelay = Time.time + fireRate;
                Shoot();
            }
        }

        //GunMove();
	}

    void Shoot() {
        if (currentAmmoInMag > 0)
        {
            flash.Play();
            CmdSpawnProjectile();
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
    void CmdSpawnProjectile()
    {
        GameObject instance = Instantiate(projectilePrefab, barrellExit.position, barrellExit.rotation);
        instance.GetComponent<Rigidbody>().AddForce(barrellExit.forward * range);

        NetworkServer.Spawn(instance);
    }

    /*void GunMove() {
        xRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;
        if (xRotation > 90)
        {
            xRotation = 90;
        }
        else if (xRotation < -90)
        {
            xRotation = -90;
        }

        yRotation += Input.GetAxis("Mouse X") * lookSensitivity;
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }*/
}
