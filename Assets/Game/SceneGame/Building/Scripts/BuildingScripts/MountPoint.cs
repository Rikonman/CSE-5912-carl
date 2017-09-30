using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MountPoint {
    public Transform parent;
    public List<Vector3> points = new List<Vector3>();
    public List<BuildPoints.MountType> pointType = new List<BuildPoints.MountType>();
}
