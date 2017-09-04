using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CreateFolders : MonoBehaviour {

    bool exists;
    string pong = Environment.SpecialFolder.Desktop + "/Pong/";
    string saves = Environment.SpecialFolder.Desktop + "/Pong/Saves";
    string screenshots = Environment.SpecialFolder.Desktop + "/Pong/Screenshots";

    // Use this for initialization
    void Start () {
        exists = System.IO.Directory.Exists(pong);
        if (!exists)
            System.IO.Directory.CreateDirectory(pong);

        exists = System.IO.Directory.Exists(saves);
        if (!exists)
            System.IO.Directory.CreateDirectory(saves);

        exists = System.IO.Directory.Exists(screenshots);
        if (!exists)
            System.IO.Directory.CreateDirectory(screenshots);
    }
	
}
