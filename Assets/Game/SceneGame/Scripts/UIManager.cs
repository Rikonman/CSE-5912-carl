using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UIManager : NetworkBehaviour {

    private Text txtAmmoInMag;
    private Text txtAmmoInReserve;
    private Text txtPlayerHealth;

    private GunController gunController;
    private Target playerTarget;

	// Use this for initialization
	void Start () {
        // if this player is not the local player...
        if (!isLocalPlayer)
        {
            // then remove this script. By removing this script all the rest of the code will not run.
            Destroy(this);
            return;
        }
        gunController = GetComponent<GunController>();
        txtAmmoInMag = GameObject.Find("AmmoInMag").GetComponent<Text>();
        txtAmmoInReserve = GameObject.Find("AmmoInReserve").GetComponent<Text>();
        txtPlayerHealth = GameObject.Find("PlayerHealth").GetComponent<Text>();
        playerTarget = GetComponent<Target>();
    }

    // Update is called once per frame
    void Update () {
        // Only update the text if there is a reason to update it.
        if (gunController.currentAmmoInMag.ToString() != txtAmmoInMag.text.Substring(txtAmmoInMag.text.IndexOf(":") + 2))
        {
            txtAmmoInMag.text = txtAmmoInMag.text.Substring(0, txtAmmoInMag.text.IndexOf(":")) + ": " + gunController.currentAmmoInMag.ToString();
        }
        // Only update the text if there is a reason to update it.
        if (gunController.currentAmmoInReserve.ToString() != txtAmmoInReserve.text.Substring(txtAmmoInReserve.text.IndexOf(":") + 2))
        {
            txtAmmoInReserve.text = txtAmmoInReserve.text.Substring(0, txtAmmoInReserve.text.IndexOf(":")) + ": " + gunController.currentAmmoInReserve.ToString();
        }
        // Only update the text if there is a reason to update it.
        if (playerTarget.currentHealth.ToString() != txtPlayerHealth.text.Substring(txtPlayerHealth.text.IndexOf(":") + 2, txtPlayerHealth.text.IndexOf("/") - (txtPlayerHealth.text.IndexOf(":") + 2)))
        {
            txtPlayerHealth.text = txtPlayerHealth.text.Substring(0, txtPlayerHealth.text.IndexOf(":")) + ": " + playerTarget.currentHealth.ToString() + "/" + playerTarget.startingHealth.ToString();
        }
    }
}
