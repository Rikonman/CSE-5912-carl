using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponDisplay : MonoBehaviour
{
    public GameObject[] uiSlots;
    public Sprite pistol;
    public Sprite assault;
    public Sprite shotgun;
    public Sprite sniper;
    public Sprite rockets;
    public Sprite minigun;
    public Sprite larpa;
    public Sprite gauss;
    public Sprite cluster;
    public Sprite spike;

    public void ChangeSlot(int slotNum, int gunChoice)
    {
        if (gunChoice == -1)
        {
            uiSlots[slotNum].SetActive(false);
        }
        else
        {
            uiSlots[slotNum].transform.GetChild(1).GetComponent<Image>().sprite = gunChoice == 0 ? pistol : (gunChoice == 1 ? assault :
                (gunChoice == 2 ? shotgun : (gunChoice == 3 ? sniper : (gunChoice == 4 ? rockets : (gunChoice == 5 ? minigun :
                (gunChoice == 6 ? larpa : (gunChoice == 7 ? gauss : (gunChoice == 7 ? cluster : spike))))))));
            uiSlots[slotNum].SetActive(true);
        }
    }
    
}
