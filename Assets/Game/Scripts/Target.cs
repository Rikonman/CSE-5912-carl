using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {

    public float health = 50f;
	public bool isVulnerable = true; 

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
