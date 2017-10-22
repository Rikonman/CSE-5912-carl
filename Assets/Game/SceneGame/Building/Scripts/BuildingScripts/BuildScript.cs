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

    public int teamID;
    public bool buildMode;
    public Material invalidMaterial;
    public Material validMaterial;
    public List<GameObject> objects = new List<GameObject>();
    public Transform baseParent;
    public Transform camera;

    List<MountPoint> mountPoints = new List<MountPoint>();
    GameObject previewObject;
    BuildPoints previewBuildPoints;
    MeshRenderer meshRend;
    int currentObject;
    Material originalMaterial;
    List<List<bool>> pointUsed = new List<List<bool>>();
    List<Vector3> placedObjects = new List<Vector3>();

    int objectIndex = -1;
    int vertexIndex = -1;
    int finalObjectIndex = -1;
    int finalVertexIndex = -1;

    GameObject BuildMenu;
    GunController gun;

    void Start()
    {
        currentObject = 0;
        if (teamID == 0)
        {
            baseParent = GameObject.Find("Base1Center").transform;
        }
        BuildMenu = GameObject.Find("BuildMenu");
        gun = GetComponent<GunController>();
    }

    void Update()
    {
        if(BuildMenu == null)
        {
            BuildMenu = GameObject.Find("BuildMenu");
        }
        if(gun == null)
        {
            gun = GetComponent<GunController>();
        }
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
        foreach (MountPoint mp in mountPoints)
        {
            for (int j = 0; j < mp.pointType.Count; j++)
            {
                if (mp.pointType[j] == previewBuildPoints.type)
                {
                    objectIndex = mountPoints.IndexOf(mp);
                    vertexIndex = j;
                    Vector3 v3 = mp.points[vertexIndex];
                    if (!pointUsed[objectIndex][vertexIndex])
                    {
                        Vector3 newVec3 = mp.parent.TransformPoint(new Vector3(v3.x / (mp.parent.transform.localScale.x), v3.y / (mp.parent.transform.localScale.y), v3.z / (mp.parent.transform.localScale.z)));
                        Debug.DrawRay(newVec3, Vector3.up, Color.red);
                        if (Vector3.Distance(previewObject.transform.position, newVec3) < 2)
                        {
                            bool near = false;
                            foreach (Vector3 nearV3 in placedObjects)
                            {
                                if (Vector3.Distance(nearV3, newVec3) < 0.1f)
                                {
                                    near = true;
                                }
                            }
                            if (!near)
                            {
                                previewObject.transform.position = newVec3 + previewBuildPoints.offset;
                                previewObject.transform.LookAt(mp.parent);
                                previewObject.transform.localEulerAngles = new Vector3(0, previewObject.transform.localEulerAngles.y - 90, 0);
                                if (previewBuildPoints.type == mp.parent.GetComponent<BuildPoints>().type)
                                {
                                    previewObject.transform.localEulerAngles = mp.parent.transform.localEulerAngles;
                                }
                                if (previewBuildPoints.type == BuildPoints.MountType.Wall
                                    && mp.parent.GetComponent<BuildPoints>().type == BuildPoints.MountType.Door1)
                                {
                                    previewObject.transform.localEulerAngles = mp.parent.transform.localEulerAngles;
                                }
                                if (previewBuildPoints.type == BuildPoints.MountType.Door1
                                   && mp.parent.GetComponent<BuildPoints>().type == BuildPoints.MountType.Wall)
                                {
                                    previewObject.transform.localEulerAngles = mp.parent.transform.localEulerAngles;
                                }
                                if(previewBuildPoints.type == BuildPoints.MountType.Stair1) {
                                    previewObject.transform.localEulerAngles = mp.parent.transform.localEulerAngles + new Vector3(38, 0,0);
                                }
                                SetMaterial(validMaterial);
                                previewBuildPoints.valid = true;

                                finalObjectIndex = mountPoints.IndexOf(mp);
                                finalVertexIndex = mp.points.IndexOf(v3);
                            }
                        }
                    }
                }
            }
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
            pointUsed[finalObjectIndex][finalVertexIndex] = true;
        }
        if (previewBuildPoints.valid)
        {
            previewObject.name = (previewBuildPoints.type.ToString() + " Placed");
            BuildPoints bp = previewBuildPoints;
            mountPoints.Add(bp.mounting);
            List<bool> newBools = new List<bool>();
            foreach (Vector3 v3 in bp.mounting.points)
            {
                bool used = false;
                newBools.Add(used);
            }
            pointUsed.Add(newBools);
            ResetMaterials();
            placedObjects.Add(previewObject.transform.position - previewBuildPoints.offset);
            previewObject.layer = 0;
            CmdSpawnBuildingPart(objects[currentObject].ToString(), currentObject, previewObject.transform.position, previewObject.transform.rotation);
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
    void CmdSpawnBuildingPart(string objectString, int objectID, Vector3 position, Quaternion rotation)
    {
        Debug.Log("Placing " + objectString + "With Object ID:" + currentObject + " at X:" + position.x + "; Y:" + position.y + "; Z:" + position.z);
        GameObject instance = Instantiate(objects[objectID], position, rotation);
        NetworkServer.Spawn(instance);
    }
}
