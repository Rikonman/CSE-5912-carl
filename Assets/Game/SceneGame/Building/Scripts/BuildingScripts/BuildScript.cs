using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BuildScript : NetworkBehaviour
{
    //TODO: Re-add preview material
    //Fix placement rotations to be type independent
    //Clean up code (Getcomponents) and overall structure
    //Snapped and move away and click causes mount point to be removed
    public List<Material> materials = new List<Material>();
    
    public bool buildMode;
    public Material invalidMaterial;
    public Material validMaterial;
    public List<GameObject> objects = new List<GameObject>();
    public Transform baseParent;
    public BaseBuildings baseBuildings;
    public Transform camera;

    GameObject previewObject;
    BuildPoints previewBuildPoints;
    MeshRenderer meshRend;
    int currentObject;
    Material originalMaterial;
    
    int finalObjectIndex = -1;
    int finalVertexIndex = -1;

    GameObject BuildMenu;
    GunController gun;
    PlayerTeam team;

    void Start()
    {
        currentObject = 0;
        StartCoroutine(LoadDelayer());
    }

    public IEnumerator LoadDelayer()
    {
        float remainingTime = 1f;

        while (remainingTime > 0)
        {
            yield return null;

            remainingTime -= Time.deltaTime;

        }
        team = GetComponent<PlayerTeam>();
        BuildMenu = GameObject.Find("BuildMenu");
        gun = GetComponent<GunController>();
        baseParent = team.baseObject.transform;
        baseBuildings = team.baseObject.GetComponent<BaseBuildings>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            buildMode = !buildMode;
        }

        if (isLocalPlayer && buildMode)
        {
            gun.enabled = !buildMode;
            BuildMenu.SetActive(buildMode);
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetpreviewObjectObject(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetpreviewObjectObject(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetpreviewObjectObject(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SetpreviewObjectObject(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SetpreviewObjectObject(4);
            }
            if (previewObject != null)
            {
                PositionPreview();

                if (Input.GetMouseButtonDown(0))
                {
                    PlaceObject();
                }
            }
        }else
        {
            if (BuildMenu != null)
            {
                BuildMenu.SetActive(buildMode);
                gun.enabled = !buildMode;
            }
            if(previewObject != null)
            {
                Destroy(previewObject);
            }
        }
    }

    void PositionPreview()
    {
        Ray previewRay = new Ray(camera.position, camera.forward);
        RaycastHit previewHit;
        SetMaterial(invalidMaterial);
        previewObject.transform.eulerAngles = new Vector3(0, camera.eulerAngles.y - 90, 0);
        previewBuildPoints.valid = false;
        if (Physics.Raycast(previewRay, out previewHit, 5f))
        {
            previewObject.transform.position = previewHit.point;

            if (currentObject == 0 && previewHit.collider.gameObject.tag == "Ground")
            {
                SetMaterial(validMaterial);
                previewBuildPoints.valid = true;
            }

            SnapPreview();
        }
        else
        {
            //Snap and check
            previewObject.transform.position = camera.position + previewBuildPoints.offset + previewRay.direction * 5f;
            SnapPreview();
        }



    }
    void SnapPreview()
    {
        int mpCounter = 0;
        foreach (MountPoint mp in baseBuildings.mountPoints)
        {
            GameObject[] tempObjs = GameObject.FindGameObjectsWithTag("Building");
            GameObject tempObj = null;
            foreach (GameObject currentObj in tempObjs)
            {
                BuildIdentifier tempBuild = currentObj.GetComponent<BuildIdentifier>();
                if (tempBuild.id == mp.objectID && tempBuild.team == mp.team)
                {
                    tempObj = currentObj;
                    break;
                }
            }
            for (int j = 0; j < mp.pointType.Length; j++)
            {

                if (mp.pointType[j] == previewBuildPoints.type)
                {
                    Vector3 v3 = mp.points[j];
                    if (!((bool)baseBuildings.pointUsed[mpCounter].boolList[j]))
                    {
                        Vector3 newVec3 = tempObj.transform.TransformPoint(new Vector3(v3.x / (tempObj.transform.localScale.x), v3.y / (tempObj.transform.localScale.y), v3.z / (tempObj.transform.localScale.z)));
                        //Matrix4x4 m = Matrix4x4.TRS(mp.parentPosition, mp.parentRotation, mp.parentScale);
                        //Vector3 newVec3 = m.MultiplyPoint3x4(new Vector3(v3.x / mp.parentScale.x, v3.y / mp.parentScale.y, v3.z / mp.parentScale.z));
                        Debug.DrawRay(newVec3, Vector3.up, Color.red);
                        if (Vector3.Distance(previewObject.transform.position, newVec3) < 2)
                        {
                            bool near = false;
                            foreach (Vector3 nearV3 in baseBuildings.placedObjects)
                            {
                                if (Vector3.Distance(nearV3, newVec3) < 0.1f)
                                {
                                    near = true;
                                }
                            }
                            if (!near)
                            {
                                previewObject.transform.position = newVec3 + previewBuildPoints.offset;
                                previewObject.transform.LookAt(tempObj.transform);
                                previewObject.transform.localEulerAngles = new Vector3(0, previewObject.transform.localEulerAngles.y - 90, 0);
                                if (previewBuildPoints.type == mp.parentMountType)
                                {
                                    previewObject.transform.localEulerAngles = tempObj.transform.localEulerAngles;
                                }
                                if (previewBuildPoints.type == BuildPoints.MountType.Wall
                                    && mp.parentMountType == BuildPoints.MountType.Door1)
                                {
                                    previewObject.transform.localEulerAngles = tempObj.transform.localEulerAngles;
                                }
                                if (previewBuildPoints.type == BuildPoints.MountType.Door1
                                   && mp.parentMountType == BuildPoints.MountType.Wall)
                                {
                                    previewObject.transform.localEulerAngles = tempObj.transform.localEulerAngles;
                                }
                                if(previewBuildPoints.type == BuildPoints.MountType.Stair1) {
                                    previewObject.transform.localEulerAngles = tempObj.transform.localEulerAngles + new Vector3(38, 0, 0);
                                }
                                SetMaterial(validMaterial);
                                previewBuildPoints.valid = true;

                                finalObjectIndex = mpCounter;
                                finalVertexIndex = j;
                            }
                        }
                    }
                }
            }
            mpCounter++;
        }
    }

    void SetpreviewObjectObject(int id)
    {

        if (previewObject != null)
        {
            Destroy(previewObject);
        }
        currentObject = id;
        previewObject = Instantiate(objects[currentObject], baseParent);
        meshRend = previewObject.GetComponent<MeshRenderer>();
        SaveMaterials();
        SetMaterial(invalidMaterial);
        previewObject.layer = 2;
        previewBuildPoints = previewObject.GetComponent<BuildPoints>();
        previewObject.transform.localEulerAngles = new Vector3(0, 0, 0);
    }
    
    void PlaceObject()
    {
        if (finalVertexIndex != -1 && finalObjectIndex != -1)
        {
            baseBuildings.pointUsed[finalObjectIndex].boolList[finalVertexIndex] = true;
        }
        if (previewBuildPoints.valid)
        {
            previewObject.name = (previewBuildPoints.type.ToString() + " Placed");
            BuildPoints bp = previewBuildPoints;
            int mountIndex = baseBuildings.mountPoints.Count;
            bp.mounting.SetTeam(team.team);
            bp.mounting.SetObjectID(mountIndex);
            CmdAddMountPoint(bp.mounting, team.team);
            SyncStructBool tempBools;
            tempBools.boolList = new bool[0];
            foreach (Vector3 v3 in bp.mounting.points)
            {
                bool used = false;
                tempBools.Add(used);
            }
            CmdAddPointUsed(tempBools, team.team);
            ResetMaterials();
            CmdAddPlacedObject(previewObject.transform.position - previewBuildPoints.offset, team.team);
            previewObject.layer = 0;
            CmdSpawnBuildingPart(objects[currentObject].ToString(), currentObject, previewObject.transform.position, previewObject.transform.rotation, mountIndex, team.team);

            previewObject = null;
            meshRend = null;
        }
    }

    void SaveMaterials()
    {
        materials.Clear();
        if (meshRend != null)
        materials.Add(meshRend.material);
        for (int i = 0; i < previewObject.transform.childCount; i++)
        {
            MeshRenderer rend = previewObject.transform.GetChild(i).GetComponent<MeshRenderer>();
            if (rend != null)
                materials.Add(rend.material);
        }

    }
    void ResetMaterials()
    {
        int index = 0;
        if (meshRend != null)
        {
            meshRend.material = materials[index];
        }
        else
        {
            index--;
        }
        for (int i = 0; i < previewObject.transform.childCount; i++)
        {
            MeshRenderer rend = previewObject.transform.GetChild(i).GetComponent<MeshRenderer>();
            if (rend != null)
            {
                rend.material = materials[index + 1];
                index++;
            }
        }
        materials.Clear();
    }
    void SetMaterial(Material mat)
    {
        if(meshRend != null)
        meshRend.material = mat;
        for (int i = 0; i < previewObject.transform.childCount; i++)
        {
            MeshRenderer rend = previewObject.transform.GetChild(i).GetComponent<MeshRenderer>();
            if (rend != null)
                rend.material = mat;
        }
    }

    [Command]
    void CmdAddPointUsed(SyncStructBool point, int team)
    {
        GameObject baseObj;
        if (team == 0)
        {
            baseObj = GameObject.Find("Base1Center");
        }
        else
        {
            baseObj = GameObject.Find("Base2Center");
        }
        baseObj.GetComponent<BaseBuildings>().AddPointUsed(point);
    }

    [Command]
    void CmdAddMountPoint(MountPoint mp, int team)
    {
        GameObject baseObj;
        if (team == 0)
        {
            baseObj = GameObject.Find("Base1Center");
        }
        else
        {
            baseObj = GameObject.Find("Base2Center");
        }
        baseObj.GetComponent<BaseBuildings>().AddMountPoint(mp);
    }

    [Command]
    void CmdAddPlacedObject(Vector3 vec, int team)
    {
        GameObject baseObj;
        if (team == 0)
        {
            baseObj = GameObject.Find("Base1Center");
        }
        else
        {
            baseObj = GameObject.Find("Base2Center");
        }
        baseObj.GetComponent<BaseBuildings>().AddPlacedObject(vec);
    }

    [Command]
    void CmdSpawnBuildingPart(string objectString, int objectID, Vector3 position, Quaternion rotation, int objectIndex, int inTeam)
    {
        Debug.Log("Placing " + objectString + "With Object ID:" + currentObject + " at X:" + position.x + "; Y:" + position.y + "; Z:" + position.z);
        GameObject tempObj = objects[objectID];
        BuildIdentifier tempID = tempObj.GetComponent<BuildIdentifier>();
        tempID.team = inTeam;
        tempID.id = objectIndex;
        GameObject instance = Instantiate(tempObj, position, rotation);
        NetworkServer.Spawn(instance);
    }
    
    
}
