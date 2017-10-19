using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Item : ScriptableObject
{
    [Tooltip("Mesh")]
    public GameObject weaponDisplay;
    public GameObject weaponInGame;
    public TextAsset description;
    public virtual void PickUp()
    {

    }
}
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
}