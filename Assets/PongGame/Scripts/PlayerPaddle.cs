using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPaddle : MonoBehaviour
{

    Vector3 position;
    float moveSpeed = 25f;
    public GameObject AIPaddle;
    public bool movingUp;
    public bool movingDown;

    void OnEnable()
    {
        KeyboardInput.KeyUp += MoveUp;
        KeyboardInput.KeyDown += MoveDown;
        AIPaddle.transform.position = new Vector3(-Screen.width / 10f, 0, 0);
        transform.position = new Vector3(Screen.width / 10f, 0, 0);
        position = transform.position;
    }

    private void Update()
    {
        movingUp = movingDown = false;
    }

    void MoveUp()
    {
        if (!PauseMenu.isPaused)
        {
            movingUp = true;
            position = Vector3.Lerp(position, position + new Vector3(0, 0, 1), moveSpeed * Time.deltaTime);
            position = new Vector3(position.x, position.y, Mathf.Clamp(position.z, -25, 25));
            transform.position = position;
        }
    }
    void MoveDown()
    {
        if (!PauseMenu.isPaused)
        {
            movingDown = true;
            position = Vector3.Lerp(position, position + new Vector3(0, 0, -1), moveSpeed * Time.deltaTime);
            position = new Vector3(position.x, position.y, Mathf.Clamp(position.z, -25, 25));
            transform.position = position;
        }
    }
}
