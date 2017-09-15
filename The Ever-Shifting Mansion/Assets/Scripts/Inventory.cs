using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject slot1;
    public GameObject slot2;
    public GameObject slot3;


    void start()
    {
        slot1 = GameObject.Find("WeaponSlot1");
        slot2 = GameObject.Find("WeaponSlot2");
        slot3 = GameObject.Find("WeaponSlot3");
    }


}
