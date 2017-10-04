using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAnimation : MonoBehaviour {

    [SerializeField]
    private float lookSensitivity = 5f;

    private float inputH;
    private float inputV;
    float xRotation;

    public Animator anim;
    public Transform spine;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

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
