using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {

    public float startingHealth = 50f;
    public float health;
	public bool isVulnerable = true;

    void Start()
    {
        health = startingHealth;
    }

    public void TakeDamage(float damage) {

		if (isVulnerable) {
			health -= damage;

			if (health <= 0) {
				Destroy (gameObject);
			}
		} else {
			Debug.Log ("This object is invulnerable"); 
		}
    }
}
