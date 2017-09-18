using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager {

    [Header("Scene Camera Properties")]
    [SerializeField] Transform sceneCamera;
    [SerializeField] float cameraRotationRadius = 100f;
    [SerializeField] float cameraRotationSpeed = 10f;
    [SerializeField] bool canRotate;

    float rotation;

    public override void OnStartClient(NetworkClient client)
    {
        base.OnStartClient(client);
        canRotate = false;
        // Turn on the gameplay's HUD
        GameObject.Find("_UI").transform.GetChild(1).gameObject.SetActive(true);
        // Hide and lock the cursor to the game window
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = (false);
    }

    public override void OnStartHost()
    {
        base.OnStartHost();
        canRotate = false;
        // Turn on the gameplay's HUD
        GameObject.Find("_UI").transform.GetChild(1).gameObject.SetActive(true);
        // Hide and lock the cursor to the game window
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = (false);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        canRotate = true;
        // Turn off the gameplay's HUD
        GameObject.Find("_UI").transform.GetChild(1).gameObject.SetActive(false);
        // Show and unlock the cursor from the game window
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = (true);
    }

    public override void OnStopHost()
    {
        base.OnStopHost();
        canRotate = true;
        // Turn off the gameplay's HUD
        GameObject.Find("_UI").transform.GetChild(1).gameObject.SetActive(false);
        // Show and unlock the cursor from the game window
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = (true);
    }

    void Update()
    {
        if (!canRotate)
            return;

        rotation += cameraRotationSpeed * Time.deltaTime;
        if (rotation >= 360f)
            rotation -= 360f;

        sceneCamera.position = Vector3.zero;
        sceneCamera.rotation = Quaternion.Euler(0f, rotation, 0f);
        sceneCamera.Translate(0f, cameraRotationRadius*0.75f, -cameraRotationRadius);
        sceneCamera.LookAt(Vector3.zero);
    }
}
