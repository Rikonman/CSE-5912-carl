using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

[RequireComponent(typeof(Rigidbody))] 
public class PlayerControls : MonoBehaviour {

	[SerializeField]
	private float jumpSensitivity = 1500f;

	[SerializeField]
	private float lookSensitivity = 5f;

	//[SerializeField]
	//private Image crosshair; 

    float xRotation;
    float yRotation;
    float xRotationV;
    float yRotationV;
    float lookSmoothDamp = 0.1f;
    float walkingSpeed = 6f;
    float speed;
    float sprintSpeed = 12f;

	private Rigidbody rb; 

    // Use this for initialization
    void Start () {
		//crosshair.enabled = true;
		rb = GetComponent<Rigidbody> (); 
    }

	void OnCreate()
	{
		
	}
	
	// Update is called once per frame
	void Update () {

        xRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;
        if (xRotation > 90)
        {
            xRotation = 90;
        }
        else if (xRotation < -90) {
            xRotation = -90;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = sprintSpeed;
        }
        else {
            speed = walkingSpeed;
        }

        yRotation += Input.GetAxis("Mouse X") * lookSensitivity;
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * speed, 0, Input.GetAxis("Vertical") * Time.deltaTime * speed);

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
