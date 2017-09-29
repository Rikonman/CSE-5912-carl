using UnityEngine;

public class Billboard : MonoBehaviour {

    public Camera cameraToStayFixedTo;

    void Start()
    {

    }

    // Update is called once per frame
    void Update () {
        transform.LookAt(cameraToStayFixedTo.transform);
	}
}
