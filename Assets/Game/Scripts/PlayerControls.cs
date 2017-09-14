using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] 
public class PlayerControls : MonoBehaviour {

	[SerializeField]
	private float jumpSensitivity = 1000f;
	private float lookSensitivity = 5f;

    float xRotation;
    float yRotation;
    float xRotationV;
    float yRotationV;
    float lookSmoothDamp = 0.1f;

	private Rigidbody rb; 

    // Use this for initialization
    void Start () {
		rb = GetComponent<Rigidbody> (); 
    }
	
	// Update is called once per frame
	void Update () {

        xRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;
        yRotation += Input.GetAxis("Mouse X") * lookSensitivity;
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * 3.0f, 0, Input.GetAxis("Vertical") * Time.deltaTime * 3.0f);

		Vector3 jumpForce = Vector3.zero; 

		if (Input.GetButton ("Jump")) {
			jumpForce = Vector3.up * jumpSensitivity; 
			if (jumpForce != Vector3.zero)
			{
				rb.AddForce (Time.fixedDeltaTime * jumpForce, ForceMode.Acceleration); 
			}
		}
    }
}
