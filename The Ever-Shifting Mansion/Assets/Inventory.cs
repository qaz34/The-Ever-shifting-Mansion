using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
public class Inventory : MonoBehaviour
{
    public GameObject equippedWeapon1;
    public GameObject equippedWeapon2;
    public GameObject equippedWeapon3;
    public GameObject weapon1;
    public GameObject weapon2;
    public GameObject weapon3;
    private Transform slot1;
    private Transform slot2;
    private Transform slot3;

    public List<Weapon> weapons;
    int index;

    void Update()
    {
        InputDevice device = InputManager.ActiveDevice;
        GameObject go = Instantiate(equippedWeapon1, slot1.transform.position, slot1.transform.localRotation);        
        if (device.DPad.Left.WasPressed)
        {
            index--;
            if (index < 0)
                index = 2;
            CombatController chara = new CombatController();
            chara.equipWeapon = weapons[index];
            equippedWeapon1 = weapon2;
            print("Space was pressed!");
        }
        if (device.DPad.Right.WasPressed)
        {
            index--;
            index %= 3;
            CombatController chara = new CombatController();
            chara.equipWeapon = weapons[index];
            equippedWeapon1 = weapon2;
            print("Space was pressed!");
        }
    }


    void Start()
    {
        //CombatController chara = new CombatController();
        //equippedWeapon1 = chara.equipWeapon.weaponPrefab;
        equippedWeapon1 = weapon1;




        slot1 = GameObject.Find("WeaponSlot1").transform;
        slot2 = GameObject.Find("WeaponSlot2").transform;
        slot3 = GameObject.Find("WeaponSlot3").transform;

        //  GameObject go2 = Instantiate(weapon2, slot2.transform.position, slot2.transform.localRotation);
        //   GameObject go3 = Instantiate(weapon3, slot3.transform.position, slot3.transform.localRotation);
    }

}



