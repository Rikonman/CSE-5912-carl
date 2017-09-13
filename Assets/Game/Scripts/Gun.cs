using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public float damage = 1f;
    public float range = 100f;
    public Camera fpsCamera;
    public ParticleSystem flash;
    public GameObject hitEffect;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1")) {
            Shoot();
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

            GameObject tempHitEffect = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(tempHitEffect, 0.3f);
        }
    }
}
