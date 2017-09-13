using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public float damage = 1f;
    public float range = 100f;
    public Camera fpsCamera;
    public ParticleSystem flash;
    public GameObject hitEffect;
    public float force = 60f;
    public float fireRate = .1f;
    public float fireDelay = 0f;
    public bool automatic = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
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
	}

    void Shoot() {
        flash.Play();
        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range)) {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if (target != null) {
                target.TakeDamage(damage);
            }

            if(hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * force);
            }

            GameObject tempHitEffect = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(tempHitEffect, 0.3f);
        }
    }
}
