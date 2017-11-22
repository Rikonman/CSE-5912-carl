using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class BuyManager : NetworkBehaviour {

    public bool buyMode;
    public GameObject BuildMenu;
    public GameObject BuyMenu;
    public bool locked;
    public PlayerTeam team;
    public GunController gun;
    public BuildScript buildScript;
    // Use this for initialization
    void Start ()
    {
        team = gameObject.GetComponent<PlayerTeam>();
        gun = gameObject.GetComponent<GunController>();
        buildScript = gameObject.GetComponent<BuildScript>();
        buyMode = false;
        locked = false;
        if (isLocalPlayer)
        {
            BuildMenu = GameObject.Find("BuildMenu");
            BuyMenu = GameObject.Find("BuyMenu");
            StartCoroutine(Delayer());
        }
    }

    public IEnumerator Delayer()
    {
        float remainingTime = 1f;

        while (remainingTime > 0)
        {
            yield return null;

            remainingTime -= Time.deltaTime;

        }

        BuildMenu.SetActive(false);
        BuyMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!locked)
        {

            if (isLocalPlayer)
            {

                if (Input.GetKeyDown(KeyCode.V))
                {
                    buyMode = !buyMode;
                }

                if (buyMode)
                {
                    gun.enabled = !buyMode;
                    buildScript.buildMode = false;
                    BuyMenu.SetActive(true);
                    BuildMenu.SetActive(false);
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        ResourceBank resources = team.baseObject.GetComponent<ResourceBank>();
                        if (resources.metal >= 50 && gun.currentGun == 0)
                        {
                            gun.CmdSwitch(1);
                            resources.Add("Metal", -50);
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        ResourceBank resources = team.baseObject.GetComponent<ResourceBank>();
                        if (resources.metal >= 50 && gun.currentGun == 0)
                        {
                            gun.CmdSwitch(2);
                            resources.Add("Metal", -50);
                        }
                    }
                }
                else
                {
                    if (BuyMenu != null)
                    {
                        BuyMenu.SetActive(false);
                    }

                }
            }
        }
    }
}
