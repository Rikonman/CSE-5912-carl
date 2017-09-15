using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPoints : MonoBehaviour
{
    public enum MountType { Floor1, Wall, Ceiling1, Door1};
    public MountType type;
    string[] fileStrings;
    public MountPoint mounting;
    public Vector3 offset;
    public bool valid;

    void Start()
    {
        TextAsset textFile = (TextAsset)Resources.Load("MountingPoints");
        fileStrings = textFile.text.Split('[');
        AddPoints(type.ToString());
    }

    void Update()
    {

    }

    void AddPoints(string typeString)
    {
        string typeData = "";
        foreach (string str in fileStrings)
        {
            if (str.Length > 0)
            {
                if (str.Substring(0, str.IndexOf('\n') - 2) == type.ToString())
                {
                    typeData = str;
                }
            }
        }
        string[] filePoints = typeData.Split(':');
        for (int i = 1; i < filePoints.Length; i++)
        {
            string[] xyz = filePoints[i].Split(',');
            float x;
            float.TryParse(xyz[0],out x);
            float y;
            float.TryParse(xyz[1], out y);
            float z;
            float.TryParse(xyz[2], out z);
            int type;
            int.TryParse(xyz[3], out type);
            mounting.parent = transform;
            mounting.points.Add(new Vector3(x, y, z));
            mounting.pointType.Add((MountType)type);
        }
    }
}
