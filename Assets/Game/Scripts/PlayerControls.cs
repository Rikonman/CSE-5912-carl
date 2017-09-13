using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {

    public float lookSensitivity = 5f;
    float xRotation;
    float yRotation;
    float xRotationV;
    float yRotationV;
    float lookSmoothDamp = 0.1f;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {

        xRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;
        yRotation += Input.GetAxis("Mouse X") * lookSensitivity;
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * 3.0f, 0, Input.GetAxis("Vertical") * Time.deltaTime * 3.0f);

    }
}
