using UnityEngine;
using UnityEngine.Networking;

public class PlayerLobbyInfo : NetworkBehaviour {

    [SyncVar]
    public Color playerColor;
    [SyncVar]
    public string playerName;

    MeshRenderer[] rends;

	// Use this for initialization
	void Start () {
        rends = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].material.color = playerColor;
        }
        Debug.Log("Player Color: " + playerColor.ToString());
        Debug.Log("Player Name: " + playerName);
	}
	
	public void HidePlayer()
    {
        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].material.color = Color.clear;
        }
    }
}
