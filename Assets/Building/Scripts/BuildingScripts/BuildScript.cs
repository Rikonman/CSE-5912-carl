using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildScript : MonoBehaviour
{
//TODO: Re-add preview material
//Fix placement rotations to be type independent
//Clean up code (Getcomponents) and overall structure
    public bool buildMode;
    public Material invalidMaterial;
    public Material validMaterial;
    public List<GameObject> objects = new List<GameObject>();
    public Transform baseParent;


    List<MountPoint> mountPoints = new List<MountPoint>();
    GameObject previewObject;
    MeshRenderer meshRend;
    int currentObject;
    Material originalMaterial;    
    List<List<bool>> pointUsed = new List<List<bool>>();
    List<Vector3> placedObjects = new List<Vector3>();

    int objectIndex = -1;
    int vertexIndex = -1;
    int finalObjectIndex = -1;
    int finalVertexIndex = -1;


    void Start()
    {
        currentObject = 0;
    }

    void Update()
    {
        if (buildMode)
        {
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
            if (previewObject != null)
            {
                PositionPreview();

                if (Input.GetMouseButtonDown(0))
                {
                    PlaceObject();

                }
            }
        }
    }

    void PositionPreview()
    {
        Ray previewRay = new Ray(transform.position, transform.forward);
        RaycastHit previewHit;
        //meshRend.material = invalidMaterial;
        previewObject.transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y - 90, 0);
        previewObject.GetComponent<BuildPoints>().valid = false;
        if (Physics.Raycast(previewRay, out previewHit, 5f))
        {
            previewObject.transform.position = previewHit.point;

            if (currentObject == 0 && previewHit.collider.gameObject.tag == "Ground")
            {
                //meshRend.material = validMaterial;
                previewObject.GetComponent<BuildPoints>().valid = true;
            }

            SnapPreview();
        }
        else
        {
            //Snap and check
            previewObject.transform.position = transform.position + previewObject.GetComponent<BuildPoints>().offset + previewRay.direction * 5f;
            SnapPreview();
        }



    }
    void SnapPreview()
    {
        foreach (MountPoint mp in mountPoints)
        {
            for (int j = 0; j <  mp.pointType.Count; j++)
            {
                if (mp.pointType[j] == previewObject.GetComponent<BuildPoints>().type)
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
                            foreach(Vector3 nearV3 in placedObjects)
                            {
                                if(Vector3.Distance(nearV3,newVec3) < 0.1f)
                                {
                                    near = true;
                                }
                            }
                            if (!near)
                            {
                                previewObject.transform.position = newVec3 + previewObject.GetComponent<BuildPoints>().offset;
                                previewObject.transform.LookAt(mp.parent);
                                previewObject.transform.localEulerAngles = new Vector3(0, previewObject.transform.localEulerAngles.y - 90, 0);
                                if (previewObject.GetComponent<BuildPoints>().type == mp.parent.GetComponent<BuildPoints>().type)
                                {
                                    previewObject.transform.localEulerAngles = mp.parent.transform.localEulerAngles;
                                }
                                if (previewObject.GetComponent<BuildPoints>().type == BuildPoints.MountType.Wall 
                                    && mp.parent.GetComponent<BuildPoints>().type == BuildPoints.MountType.Door1)
                                {
                                    previewObject.transform.localEulerAngles = mp.parent.transform.localEulerAngles;
                                }
                                if (previewObject.GetComponent<BuildPoints>().type == BuildPoints.MountType.Door1
                                   && mp.parent.GetComponent<BuildPoints>().type == BuildPoints.MountType.Wall)
                                {
                                    previewObject.transform.localEulerAngles = mp.parent.transform.localEulerAngles;
                                }
                                //meshRend.material = validMaterial;
                                previewObject.GetComponent<BuildPoints>().valid = true;

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
        //meshRend = previewObject.GetComponent<MeshRenderer>();
        //originalMaterial = meshRend.material;
        //meshRend.material = invalidMaterial;
        previewObject.layer = 2;
    }

    void PlaceObject()
    {
        if (finalVertexIndex != -1 && finalObjectIndex != -1)
        {
            pointUsed[finalObjectIndex][finalVertexIndex] = true;
        }
        if (previewObject.GetComponent<BuildPoints>().valid)
        {
            previewObject.name = (previewObject.GetComponent<BuildPoints>().type.ToString() + " Placed");
            BuildPoints bp = previewObject.GetComponent<BuildPoints>();
            mountPoints.Add(bp.mounting);
            List<bool> newBools = new List<bool>();
            foreach (Vector3 v3 in bp.mounting.points)
            {
                bool used = false;
                newBools.Add(used);
            }
            pointUsed.Add(newBools);
            //meshRend.material = originalMaterial;
            placedObjects.Add(previewObject.transform.position - previewObject.GetComponent<BuildPoints>().offset);
            previewObject.layer = 0;
            previewObject = null;
            //meshRend = null;
        }
    }
}
