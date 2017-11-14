using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UIManager : NetworkBehaviour {

    private Text txtAmmoInMag;
    private Text txtAmmoInReserve;
    private Text txtPlayerHealth;

    private GunController gunController;
    private Target playerTarget;
    private RectTransform healthbar;

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
        playerTarget = GetComponent<Target>();
        playerTarget.onHealthChanged += HealthChanged;
        txtPlayerHealth = GameObject.Find("UIPHBText").GetComponent<Text>();
        healthbar = GameObject.Find("UIPHBValue").GetComponent<RectTransform>();
        txtPlayerHealth.text = string.Format("{0:N0}", playerTarget.currentHealth) + " | " + string.Format("{0:N0}", playerTarget.startingHealth);
    }

    private void HealthChanged(float prevVal, float newVal)
    {
        Debug.Log("UIManager Health Changed");
        if (healthbar != null)
        {
            Debug.Log("Healthbar UI Changed. Was: " + string.Format("{0:N2}", prevVal) + "; Now: " + string.Format("{0:N2}", prevVal));
            healthbar.sizeDelta = new Vector2(newVal * 5, healthbar.sizeDelta.y);
            txtPlayerHealth.text = string.Format("{0:N0}", playerTarget.currentHealth) + " | " + string.Format("{0:N0}", playerTarget.startingHealth);
        }
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
        //if (playerTarget.currentHealth.ToString() != txtPlayerHealth.text.Substring(txtPlayerHealth.text.IndexOf(":") + 2, txtPlayerHealth.text.IndexOf("/") - (txtPlayerHealth.text.IndexOf(":") + 2)))
        //{
        //    txtPlayerHealth.text = txtPlayerHealth.text.Substring(0, txtPlayerHealth.text.IndexOf(":")) + ": " + playerTarget.currentHealth.ToString() + "/" + playerTarget.startingHealth.ToString();
        //}
    }
}
