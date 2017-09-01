using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{

    public Vector3 vel;
    public TextMesh aiScore;
    public static int aiScoreLoad = 0;
    int aiScoreInt = 0;
    public TextMesh playerScore;
    int playerScoreInt = 0;
    public static int playerScoreLoad = 0;
    float moveSpeed = 35f;
    public static bool load = false;

    void Start()
    {
        int direction = Mathf.RoundToInt(Random.value);
        if (direction == 0)
        {
            direction = -1;
        }
        else
        {
            direction = 1;
        }
        vel = new Vector3(1 * moveSpeed, 0, direction * moveSpeed);
    }

    void Update()
    {

        if (load)
        {
            aiScoreInt = aiScoreLoad;
            playerScoreInt = playerScoreLoad;
            aiScore.text = "" + aiScoreInt;
            playerScore.text = "" + playerScoreInt;
            load = false;
        }

        if (PauseMenu.IsPaused)
        {
            return;
        }
        vel = vel.normalized * moveSpeed;
        transform.position = Vector3.Lerp(transform.position, transform.position + vel, Time.deltaTime);
        if (transform.position.x >= Screen.width / 10)
        {
            aiScoreInt++;
            aiScore.text = ""+aiScoreInt;
            aiScoreLoad = aiScoreInt;
            transform.position = Vector3.zero;
        }
        else if (transform.position.x <= -Screen.width / 10)
        {
            playerScoreInt++;
            playerScore.text = "" + playerScoreInt;
            playerScoreLoad = playerScoreInt;
            transform.position = Vector3.zero;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "LeftPaddle")
        {
            vel = Vector3.Reflect(vel, new Vector3(1, 0, 0));
            if (other.GetComponent<AIPaddle>().movingUp)
            {
                vel.z = Mathf.Abs(vel.z);
            }
            if (other.GetComponent<AIPaddle>().movingDown)
            {
                vel.z = -Mathf.Abs(vel.z);
            }
        }
        if (other.transform.name == "RightPaddle")
        {
            vel = Vector3.Reflect(vel, new Vector3(-1, 0, 0));
            if (other.GetComponent<PlayerPaddle>().movingUp)
            {
                vel.z = Mathf.Abs(vel.z);
            }
            if (other.GetComponent<PlayerPaddle>().movingDown)
            {
                vel.z = -Mathf.Abs(vel.z);
            }
        }
        if (other.transform.name == "TopWall")
        {
            vel = Vector3.Reflect(vel, new Vector3(0, 0, -1));
        }
        if (other.transform.name == "BottomWall")
        {
            vel = Vector3.Reflect(vel, new Vector3(0, 0, 1));
        }
    }
}
