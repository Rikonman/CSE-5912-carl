using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EManager : NetworkBehaviour {
    public float eTime;
    public int eTarget;
    public float repairTime;
    public float hpPerRepair;
    public Transform cameraTrans;
    public PlayerTeam team;
    public GameObject lookTextBox;
    public Text lookText;
    // Use this for initialization
    void Start () {
        eTime = 0;
        eTarget = -1;
        team = gameObject.GetComponent<PlayerTeam>();
        if (isLocalPlayer)
        {
            StartCoroutine(TextDelayer());
        }
        
    }

    public IEnumerator TextDelayer()
    {
        float remainingTime = 1f;

        while (remainingTime > 0)
        {
            yield return null;

            remainingTime -= Time.deltaTime;

        }

        lookTextBox = GameObject.Find("LookText");
        lookText = lookTextBox.GetComponent<Text>();

    }

    void FixedUpdate () {
        if (isLocalPlayer)
        {
            Ray previewRay = new Ray(cameraTrans.position, cameraTrans.forward);
            RaycastHit previewHit;
            if (Physics.Raycast(previewRay, out previewHit, 3f))
            {
                    Transform hitTrans = previewHit.transform;
                    GameObject hitGameObject = hitTrans.parent != null ? hitTrans.parent.gameObject : hitTrans.gameObject;
                Target hitTarget = hitGameObject.GetComponent<Target>();
                if (hitTarget != null)
                {
                    if (hitTarget.currentHealth < hitTarget.startingHealth)
                    {
                        lookText.text = "Hold 'E' to repair (" + string.Format("{0:P0}", hitTarget.currentHealth / hitTarget.startingHealth) + ")";
                        lookText.enabled = true;
                    }
                    else
                    {
                        lookText.enabled = false;
                    }
                }
                if (Input.GetKey(KeyCode.E))
                {
                    BuildIdentifier bid = hitGameObject.GetComponent<BuildIdentifier>();

                    if (bid != null && bid.team == team.team)
                    {
                        if (eTarget != bid.id)
                        {
                            eTime = 0;
                            eTarget = bid.id;
                        }
                        else
                        {
                            eTime += Time.deltaTime;
                            if (eTime > repairTime)
                            {
                                eTime = 0;
                                CmdRepair(hitGameObject.GetComponent<NetworkIdentity>().netId);
                            }
                        }
                    }
                    else
                    {
                        eTime = 0;
                        eTarget = -1;
                    }
                }
                else
                {
                    eTime = 0;
                    eTarget = -1;
                }
            }
            else
            {
                eTime = 0;
                eTarget = -1;
                if (lookText != null)
                {
                    lookText.enabled = false;
                }
                
            }
        }
        
	}

    [Command]
    public void CmdRepair(NetworkInstanceId nid)
    {
        GameObject hitGameObject = ClientScene.FindLocalObject(nid);
        Target target = hitGameObject.GetComponent<Target>();
        target.RepairBuilding(hpPerRepair);
    }
}
