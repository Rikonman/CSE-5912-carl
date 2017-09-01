using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : MonoBehaviour {

    public delegate void KeyboardUp();
    public static event KeyboardUp KeyUp;
    
    public delegate void KeyboardDown();
    public static event KeyboardDown KeyDown;

    public delegate void KeyboardEscape();
    public static event KeyboardEscape Paused;

    bool Started;
	
	void Update () {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            KeyUp();
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            KeyDown();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Paused();
        }
    }
}
