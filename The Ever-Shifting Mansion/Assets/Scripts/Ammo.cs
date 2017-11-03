using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[CreateAssetMenu(fileName = "Item", menuName = "Items/Ammo", order = 1)]
public class Ammo : Item
{
    public AmmoType ammoType;
    public int amount;
    public override void PickUp()
    {
        Inventory inv = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        foreach (var ammo in inv.ammo)
            if (ammo.ammoType == ammoType)
            {
                ammo.amount += amount;
                return;
            }
        inv.ammo.Add(Instantiate(this));
    }
    public override void Interact()
    {
        Inventory inv = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        foreach (var wep in inv.weapons.Where(i => i && i.type != WepType.MELEE))
            if (((RangedWep)wep).ammoType == ammoType)
                ((RangedWep)wep).InstantReload(this);
    }
}