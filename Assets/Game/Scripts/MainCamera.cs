using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {

    public AudioSource sfxAudioSource;

    public static bool Is3D { get; set; }

    public delegate void GameStartAnimCompleted();
    public static event GameStartAnimCompleted gameStartAnimCompleted;

    public delegate void TwoToThreeAnimStarted();
    public static event TwoToThreeAnimStarted twoToThreeAnimStarted;

    public delegate void TwoToThreeAnimCompleted();
    public static event TwoToThreeAnimCompleted twoToThreeAnimCompleted;

    public delegate void ThreeToTwoAnimStarted();
    public static event ThreeToTwoAnimStarted threeToTwoAnimStarted;

    public delegate void ThreeToTwoAnimCompleted();
    public static event ThreeToTwoAnimCompleted threeToTwoAnimCompleted;

    private Animator cameraAnimator;

    void Start()
    {
        cameraAnimator = GetComponent<Animator>();
        sfxAudioSource.volume = GameSettings.SfxVolume;
    }

    private void Update()
    {
        if (PauseMenu.IsGameReady)
        {
            cameraAnimator.SetBool("is3Dto2D", Input.GetKeyDown(KeyCode.Alpha2));
            cameraAnimator.SetBool("is2Dto3D", Input.GetKeyDown(KeyCode.Alpha3));
        }
        else
        {
            cameraAnimator.SetBool("is3Dto2D", false);
            cameraAnimator.SetBool("is2Dto3D", false);
        }
    }

    void CamGameStartAnimCompleted()
    {
        Is3D = false;
        NewGameCountdown test = GameObject.Find("_UI").transform.GetChild(1).gameObject.GetComponent<NewGameCountdown>();
        test.StartInitialCountdown();
        if (gameStartAnimCompleted != null)
            gameStartAnimCompleted();

    }

    void CamTwoToThreeAnimStarted()
    {
        if (!PauseMenu.IsPaused)
            PauseMenu.IsGameReady = false;
        PauseMenu.IsPaused = true;
        if (twoToThreeAnimStarted != null)
            twoToThreeAnimStarted();
    }

    void CamTwoToThreeAnimCompleted()
    {
        if (!PauseMenu.IsGameReady)
            PauseMenu.IsPaused = false;
        PauseMenu.IsGameReady = true;
        Is3D = true;
        if (twoToThreeAnimCompleted != null)
            twoToThreeAnimCompleted();
    }

    void CamThreeToTwoAnimStarted()
    {
        if (!PauseMenu.IsPaused)
            PauseMenu.IsGameReady = false;
        PauseMenu.IsPaused = true;
        if (threeToTwoAnimStarted != null)
            threeToTwoAnimStarted();
    }

    void CamThreeToTwoAnimCompleted()
    {
        if (!PauseMenu.IsGameReady)
            PauseMenu.IsPaused = false;
        PauseMenu.IsGameReady = true;
        Is3D = false;
        if (threeToTwoAnimCompleted != null)
            threeToTwoAnimCompleted();
    }
}
