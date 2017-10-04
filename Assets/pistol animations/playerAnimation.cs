using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class playerAnimation : NetworkBehaviour {

    [SerializeField]
    private float lookSensitivity = 5f;

    private float inputH;
    private float inputV;
    float xRotation;

    public Animator anim;
    public Transform spine;

    // Use this for initialization
    void Start () {
        if (!isLocalPlayer)
            return;
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
            return;
        xRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;
        if (xRotation > 70)
        {
            xRotation = 70;
        }
        else if (xRotation < -80)
        {
            xRotation = -80;
        }

        if (Input.GetKeyDown("1")) {
            anim.Play("HM_Aim_Revolver_Walk");
        }

        inputH = Input.GetAxis("Horizontal");
        inputV = Input.GetAxis("Vertical");

        anim.SetFloat("inputH", inputH);
        anim.SetFloat("inputV", inputV);

    }
    
    
}
