using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
public class Inventory : MonoBehaviour
{
    int currentlyEquipWeapon = 0;
    public List<Weapon> weapons;


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



