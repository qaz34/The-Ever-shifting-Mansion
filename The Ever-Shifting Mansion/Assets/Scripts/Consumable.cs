using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
[CreateAssetMenu(fileName = "Item", menuName = "Items/Ammo", order = 1)]
public class Consumable : Item
{
    public int amount;
    public override void PickUp()
    {
        Inventory inv = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        inv.consumables.Add(Instantiate(this));
    }
    public override void Interact()
    {
        Health health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        health.CurrentHealth += amount;
        GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().consumables.Remove(this);
    }
}