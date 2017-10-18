using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
public class Inventory : MonoBehaviour
{
    int currentlyEquipWeapon = 0;
    public List<Weapon> weapons;
    public Transform weaponLocation;
    void Start()
    {
        Instantiate(GetComponent<CombatController>().equipWeapon.weaponPrefab, weaponLocation.position, weaponLocation.rotation, weaponLocation);
    }
    void Update()
    {
        InputDevice device = InputManager.ActiveDevice;
        if (device.DPad.Left.WasPressed)
        {
            currentlyEquipWeapon = (currentlyEquipWeapon + weapons.Count - 1) % weapons.Count;
            CombatController combat = GetComponent<CombatController>();
            combat.equipWeapon = weapons[currentlyEquipWeapon];
            //equip wep on player
        }
        if (device.DPad.Right.WasPressed)
        {
            currentlyEquipWeapon = (currentlyEquipWeapon + 1) % weapons.Count;
            CombatController combat = GetComponent<CombatController>();
            combat.equipWeapon = weapons[currentlyEquipWeapon];
            //equip wep on player
        }
    }
}



