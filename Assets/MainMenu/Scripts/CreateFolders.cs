using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CreateFolders : MonoBehaviour {

    bool exists;
    public static string filepath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    string pong = System.IO.Path.Combine(filepath, "Pong/");
    string saves = System.IO.Path.Combine(filepath, "Pong/Saves");
    string screenshots = System.IO.Path.Combine(filepath, "Pong/Screenshots");
    string videos = System.IO.Path.Combine(filepath, "Pong/Videos");

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

        exists = System.IO.Directory.Exists(videos);
        if (!exists)
            System.IO.Directory.CreateDirectory(videos);
    }
	
}
