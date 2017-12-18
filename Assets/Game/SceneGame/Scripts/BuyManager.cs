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

                if (Input.GetKeyDown(KeyCode.V) && gun.CanBuy)
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
                        if (resources.metal >= 50)
                        {
                            gun.Buy(1);
                            resources.Add("Metal", -50);
                            buyMode = false;
                            BuyMenu.SetActive(false);
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        ResourceBank resources = team.baseObject.GetComponent<ResourceBank>();
                        if (resources.metal >= 50)
                        {
                            gun.Buy(2);
                            resources.Add("Metal", -50);
                            buyMode = false;
                            BuyMenu.SetActive(false);
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha3))
                    {
                        ResourceBank resources = team.baseObject.GetComponent<ResourceBank>();
                        if (resources.metal >= 50)
                        {
                            gun.Buy(3);
                            resources.Add("Metal", -50);
                            buyMode = false;
                            BuyMenu.SetActive(false);
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha4))
                    {
                        ResourceBank resources = team.baseObject.GetComponent<ResourceBank>();
                        if (resources.metal >= 100)
                        {
                            gun.Buy(4);
                            resources.Add("Metal", -100);
                            buyMode = false;
                            BuyMenu.SetActive(false);
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha5))
                    {
                        ResourceBank resources = team.baseObject.GetComponent<ResourceBank>();
                        if (resources.metal >= 100)
                        {
                            gun.Buy(5);
                            resources.Add("Metal", -100);
                            buyMode = false;
                            BuyMenu.SetActive(false);
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha6))
                    {
                        ResourceBank resources = team.baseObject.GetComponent<ResourceBank>();
                        if (resources.metal >= 200)
                        {
                            gun.Buy(6);
                            resources.Add("Metal", -200);
                            buyMode = false;
                            BuyMenu.SetActive(false);
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha7))
                    {
                        ResourceBank resources = team.baseObject.GetComponent<ResourceBank>();
                        if (resources.metal >= 150)
                        {
                            gun.Buy(7);
                            resources.Add("Metal", -150);
                            buyMode = false;
                            BuyMenu.SetActive(false);
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha8))
                    {
                        ResourceBank resources = team.baseObject.GetComponent<ResourceBank>();
                        if (resources.metal >= 200)
                        {
                            gun.Buy(8);
                            resources.Add("Metal", -200);
                            buyMode = false;
                            BuyMenu.SetActive(false);
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha9))
                    {
                        ResourceBank resources = team.baseObject.GetComponent<ResourceBank>();
                        if (resources.metal >= 100)
                        {
                            gun.Buy(9);
                            resources.Add("Metal", -100);
                            buyMode = false;
                            BuyMenu.SetActive(false);
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha0))
                    {
                        ResourceBank resources = team.baseObject.GetComponent<ResourceBank>();
                        if (resources.metal >= 150)
                        {
                            gun.Buy(10);
                            resources.Add("Metal", -150);
                            buyMode = false;
                            BuyMenu.SetActive(false);
                        }
                    }
                }
                else
                {
                    if (BuyMenu != null)
                    {
                        BuyMenu.SetActive(false);
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        if (gun.guns[0].gunIndex != -1)
                        {
                            gun.Switch(0);
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        if (gun.guns[1].gunIndex != -1)
                        {
                            gun.Switch(1);
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha3))
                    {
                        if (gun.guns[2].gunIndex != -1)
                        {
                            gun.Switch(2);
                        }
                    }
                }
            }
        }
    }
}
