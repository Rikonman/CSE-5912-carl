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
        PauseMenu.ExitGame += Exit;
        AIPaddle.transform.position = new Vector3(-40, 1, 0);
        transform.position = new Vector3(40, 1, 0);
        position = transform.position;
    }

    private void Update()
    {
        movingUp = movingDown = false;
    }

    void Exit()
    {
        KeyboardInput.KeyUp -= MoveUp;
        KeyboardInput.KeyDown -= MoveDown;
        PauseMenu.ExitGame -= Exit;
    }

    void MoveUp()
    {
        if (!PauseMenu.IsPaused)
        {
            movingUp = true;
            position = Vector3.Lerp(position, position + new Vector3(0, 0, 1), moveSpeed * Time.deltaTime);
            position = new Vector3(position.x, position.y, Mathf.Clamp(position.z, -25, 25));
            transform.position = position;
        }
    }
    void MoveDown()
    {
        if (!PauseMenu.IsPaused)
        {
            movingDown = true;
            position = Vector3.Lerp(position, position + new Vector3(0, 0, -1), moveSpeed * Time.deltaTime);
            position = new Vector3(position.x, position.y, Mathf.Clamp(position.z, -25, 25));
            transform.position = position;
        }
    }
}
