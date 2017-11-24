using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
public class Inventory : MonoBehaviour
{
    [HideInInspector]
    public int currentlyEquipWeapon = 0;
    public List<Weapon> weapons;
    public List<Ammo> ammo;
    public List<Consumable> consumables;
    public Transform weaponLocation;
    void Start()
    {
        if (GetComponent<CombatController>().equipWeapon)
            Instantiate(GetComponent<CombatController>().equipWeapon.inGame, weaponLocation.position, weaponLocation.rotation, weaponLocation);
    }
    public void Equip(bool left)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            CombatController combat = GetComponent<CombatController>();
            if (weapons[currentlyEquipWeapon])
            {
                combat.equipWeapon = weapons[currentlyEquipWeapon];
                foreach (Transform trans in weaponLocation)
                    Destroy(trans.gameObject);
                Instantiate(GetComponent<CombatController>().equipWeapon.inGame, weaponLocation.position, weaponLocation.rotation, weaponLocation);
                return;
            }
            else
            {
                if (left)
                    currentlyEquipWeapon = (currentlyEquipWeapon + weapons.Count - 1) % weapons.Count;
                else
                    currentlyEquipWeapon = (currentlyEquipWeapon + 1) % weapons.Count;
            }
        }
        GetComponent<CombatController>().ammoChanged?.Invoke();
        Debug.Log("NO WEAPONS");
    }
    void Update()
    {
        InputDevice device = InputManager.ActiveDevice;
        if (device.DPad.Left.WasPressed)
        {
            currentlyEquipWeapon = (currentlyEquipWeapon + weapons.Count - 1) % weapons.Count;
            Equip(true);
        }
        if (device.DPad.Right.WasPressed)
        {
            currentlyEquipWeapon = (currentlyEquipWeapon + 1) % weapons.Count;
            Equip(false);
        }
    }
}



