using UnityEngine;
using UnityEngine.Networking;

public class SpawnController : NetworkBehaviour {

	[SerializeField]
	Behaviour[] disabled;

	[SerializeField]
	GameObject HUDLayout; 

	private GameObject clientHUD;

	Camera LobbyCam; 

	public bool localPlayer = false; 

	void Start()
	{
		if (!isLocalPlayer) {
			for (int i = 0; i < disabled.Length; i++) {
				disabled [i].enabled = false; 
			}
		} else {
			LobbyCam = Camera.main;
			if(LobbyCam != null)
				LobbyCam.gameObject.SetActive (false);
			localPlayer = true; 

			clientHUD = Instantiate (HUDLayout); 
			clientHUD.name = HUDLayout.name; 

			clientHUD.transform.parent = GameObject.Find ("_UI").transform;
			//clientHUD.transform.gameobject.SetActive (true);
			MakeClientHUDVisible (clientHUD);
		}
	}
		
	void OnDisable()
	{
		Destroy(clientHUD);
		if (LobbyCam != null)
			LobbyCam.gameObject.SetActive (true);
	}

	void MakeClientHUDVisible(GameObject go)
	{
		go.layer = 2; 

		foreach (Transform child in go.transform) 
		{
			MakeClientHUDVisible (child.gameObject); 
		}
	}

}
