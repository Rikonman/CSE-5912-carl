using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPaddle : MonoBehaviour {

    Vector3 position;
    float moveSpeed = 25f;
    Ball ball;
    public bool movingUp;
    public bool movingDown;

    void Start () {
        ball = GameObject.Find("Ball").GetComponent<Ball>();
        position = transform.position;
	}
	
	void Update () {
        if (!PauseMenu.IsPaused)
        {
            movingUp = movingDown = false;
            if (ball.vel.normalized.x < 0 && ball.transform.position.x < -10)
            {
                if (ball.transform.position.z > transform.position.z+2)
                {
                    MoveUp();
                }
                else if (ball.transform.position.z < transform.position.z-2)
                {
                    MoveDown();
                }
            }
        }
	}

    void MoveUp()
    {
        movingUp = true;
        position = Vector3.Lerp(position, position + new Vector3(0, 0, 1), moveSpeed * Time.deltaTime);
        position = new Vector3(position.x, position.y, Mathf.Clamp(position.z, -25, 25));
        transform.position = position;
    }
    void MoveDown()
    {
        movingDown = true;
        position = Vector3.Lerp(position, position + new Vector3(0, 0, -1), moveSpeed * Time.deltaTime);
        position = new Vector3(position.x, position.y, Mathf.Clamp(position.z, -25, 25));
        transform.position = position;
    }
}
