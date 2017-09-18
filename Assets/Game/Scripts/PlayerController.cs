﻿using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{
    [Header("Movement Variables")]
    [SerializeField]
    private float jumpSensitivity = 1500f;
    [SerializeField]
    private float lookSensitivity = 5f;
    [Header("First Person Camera Position")]
    [SerializeField]
    float fpCameraY = 0.45f;                 // The height off of the ground that the camera should be
    [SerializeField]
    float fpCameraX = 0f;                    // The height off of the ground that the camera should be
    [SerializeField]
    float fpCameraZ = 0.2f;                  // The height off of the ground that the camera should be
    [Header("Third Person Camera Position")]
    [SerializeField]
    float tpCameraDistance = 6f;            // Distance from the player that the camera should be
    [SerializeField]
    float tpCameraY = 7f;                   // The height off of the ground that the camera should be
    [SerializeField]
    bool isFirstPerson = true;
	[Header("UI")]
	[SerializeField]
	GameObject HUDLayout;

    Transform mainCamera;
    Vector3 tpCameraOffset;
    Vector3 fpCameraOffset;

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
    float uiRefreshTimer;

    private Rigidbody rb;
	private GameObject clientHUD; 

    // Use this for initialization
    void Start()
    {
        // if this player is not the local player...
		if (!isLocalPlayer) {
			// then remove this script. By removing this script all the rest of the code will not run.
			Destroy (this);
			return;
		} else {
			clientHUD = Instantiate (HUDLayout);
			clientHUD.name = HUDLayout.name; 
		}
        //crosshair.enabled = true;
        rb = GetComponent<Rigidbody>();

        tpCameraOffset = new Vector3(0f, tpCameraY, -tpCameraDistance);
        fpCameraOffset = new Vector3(fpCameraX, fpCameraY, fpCameraZ);

        mainCamera = Camera.main.transform;
        MoveCamera();
        uiRefreshTimer = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        xRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;
        if (xRotation > 90)
        {
            xRotation = 90;
        }
        else if (xRotation < -90)
        {
            xRotation = -90;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = sprintSpeed;
        }
        else
        {
            speed = walkingSpeed;
        }

        yRotation += Input.GetAxis("Mouse X") * lookSensitivity;
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
        transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * speed, 0, Input.GetAxis("Vertical") * Time.deltaTime * speed);

        Vector3 jumpForce = Vector3.zero;

        if (Input.GetButton("Jump"))
        {
            jumpForce = Vector3.up * jumpSensitivity;
            if (jumpForce != Vector3.zero)
            {
                rb.AddForce(Time.fixedDeltaTime * jumpForce, ForceMode.Acceleration);
            }
        }

        // Update the camera's position/rotation
        MoveCamera();

        uiRefreshTimer += Time.deltaTime;
        if (uiRefreshTimer >= .5f)
        {
            PlayerTeam tempTeam = GetComponent<PlayerTeam>();
            ResourceBank tempBank = tempTeam.baseObject.GetComponent<ResourceBank>();
            UnityEngine.UI.Text textBox = tempTeam.resourceText.GetComponent<UnityEngine.UI.Text>();
            textBox.text = "Team " + (tempTeam.team + 1) + " \nStone: " + tempBank.stone + "\nWood: " + tempBank.wood;

            uiRefreshTimer = 0f;
        }
    }

    void MoveCamera()
    {
        mainCamera.position = transform.position;
        mainCamera.rotation = transform.rotation;
        if (isFirstPerson)
        {
            fpCameraOffset = new Vector3(fpCameraX, fpCameraY, fpCameraZ);
            mainCamera.Translate(fpCameraOffset);
            mainCamera.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        }
        else
        {
            tpCameraOffset = new Vector3(0f, tpCameraY, -tpCameraDistance);
            mainCamera.Translate(tpCameraOffset);
            mainCamera.LookAt(transform);
        }
    }

	void OnDisable()
	{
		Destroy (clientHUD);
	}
}
