using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip pongClip;
    public AudioClip wallClip;
    public AudioClip playerScoreClip;
    public AudioClip enemyScoreClip;
    public Vector3 vel;
    public TextMesh aiScore;
    public TextMesh aiScore2;
    public static int aiScoreLoad = 0;
    int aiScoreInt = 0;
    public TextMesh playerScore;
    public TextMesh playerScore2;
    int playerScoreInt = 0;
    public static int playerScoreLoad = 0;
    float moveSpeed = 35f;
    public static bool load = false;
    public static int waitTime = 2;
    float currentTime;
    bool pauseBall = false;
    NewGameCountdown newGameCountDownRef;

    void Start()
    {
        ResetVelocity();
        currentTime = 3f;
        newGameCountDownRef = GameObject.Find("_UI").transform.GetChild(1).gameObject.GetComponent<NewGameCountdown>();
    }

    void ResetVelocity()
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
        int side = Mathf.RoundToInt(Random.value);
        vel = new Vector3(1 * moveSpeed * (side == 0 ? -1 : 1), 0, direction * moveSpeed);
    }

    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.C))
            transform.position = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.B))
            pauseBall = !pauseBall;


            if (load)
        {
            aiScoreInt = aiScoreLoad;
            playerScoreInt = playerScoreLoad;
            aiScore2.text = "" + aiScoreInt;
            playerScore2.text = "" + playerScoreInt;
            aiScore.text = "" + aiScoreInt;
            playerScore.text = "" + playerScoreInt;
            load = false;
        }
        
        if (PauseMenu.IsPaused||pauseBall)
        {
            return;
        }
        if (currentTime < waitTime)
        {
            currentTime += Time.deltaTime;
        }
        vel = vel.normalized * moveSpeed;
        if (currentTime >= waitTime)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + vel, Time.deltaTime);
            GetComponent<Rigidbody>().AddForceAtPosition(vel,transform.position + new Vector3(0,1,0));
        }
        
        if (transform.position.x >= Screen.width / 10 || transform.position.x > 0 && (transform.position.z < -40 || transform.position.z > 40))
        {
            audioSource.PlayOneShot(enemyScoreClip);
            aiScoreInt++;
            aiScore.text = "" + aiScoreInt;
            aiScore2.text = aiScore.text;
            aiScoreLoad = aiScoreInt;
            ResetVelocity();
            transform.position = Vector3.zero;
            currentTime = 0f;
            newGameCountDownRef.DoShortCountdown(waitTime);
        }
        else if (transform.position.x <= -Screen.width / 10 || transform.position.x < 0 && (transform.position.z < -40 || transform.position.z > 40))
        {
            audioSource.PlayOneShot(playerScoreClip);
            playerScoreInt++;
            playerScore.text = "" + playerScoreInt;
            playerScore2.text = playerScore.text;
            playerScoreLoad = playerScoreInt;
            ResetVelocity();
            transform.position = Vector3.zero;
            currentTime = 0f;
            newGameCountDownRef.DoShortCountdown(waitTime);
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "LeftPaddle")
        {
            audioSource.PlayOneShot(pongClip);
            vel = Vector3.Reflect(vel, new Vector3(1, 0, 0));
            if (other.GetComponent<AIPaddle>().movingUp)
            {
                vel.z = Mathf.Abs(vel.z);
            }
            if (other.GetComponent<AIPaddle>().movingDown)
            {
                vel.z = -Mathf.Abs(vel.z);
            }
            Vector3 paddleVel = new Vector3(other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position).x - other.transform.position.x,
                0, other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position).z - other.transform.position.z);
            paddleVel.Normalize();
            float velMagnitude = vel.magnitude;
            Vector3 tempVel = vel;
            tempVel.Normalize();
            Vector3 newVel = paddleVel + tempVel;
            newVel.Normalize();
            newVel = newVel * velMagnitude;
            vel = newVel;
        }
        if (other.transform.name == "RightPaddle")
        {
            audioSource.PlayOneShot(pongClip);
            vel = Vector3.Reflect(vel, new Vector3(-1, 0, 0));
            if (other.GetComponent<PlayerPaddle>().movingUp)
            {
                vel.z = Mathf.Abs(vel.z);
            }
            if (other.GetComponent<PlayerPaddle>().movingDown)
            {
                vel.z = -Mathf.Abs(vel.z);
            }
            Vector3 paddleVel = new Vector3(other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position).x - other.transform.position.x,
                0, other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position).z - other.transform.position.z);
            paddleVel.Normalize();
            float velMagnitude = vel.magnitude;
            Vector3 tempVel = vel;
            tempVel.Normalize();
            Vector3 newVel = paddleVel + tempVel;
            newVel.Normalize();
            newVel = newVel * velMagnitude;
            vel = newVel;
        }
        if (other.transform.name == "TopWall")
        {
            audioSource.PlayOneShot(wallClip);
            vel = Vector3.Reflect(vel, new Vector3(0, 0, -1));
        }
        if (other.transform.name == "BottomWall")
        {
            audioSource.PlayOneShot(wallClip);
            vel = Vector3.Reflect(vel, new Vector3(0, 0, 1));
        }
    }
}
