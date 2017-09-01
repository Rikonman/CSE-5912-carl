using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {

    public delegate void GameStartAnimCompleted();
    public static event GameStartAnimCompleted gameStartAnimCompleted;

    void CamGameStartAnimCompleted()
    {
        gameStartAnimCompleted();
    }
}
