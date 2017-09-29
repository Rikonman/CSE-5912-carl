using UnityEngine;

public class Billboard : MonoBehaviour {

    private Camera cameraToStayFixedTo;

    void Start()
    {
        cameraToStayFixedTo = GameObject.Find("Camera").GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update () {
        transform.LookAt(cameraToStayFixedTo.transform);
	}
}
