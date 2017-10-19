using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WepType
{
    MELEE,
    RANGED,
    SPECIAL
}
public enum AmmoType
{
    PISTOL,
    RIFLE,
    SHOTGUN,
    NADES
}

public class Weapon : Item
{
    public int damage = 10;
    [Tooltip("Rounds per second")]
    public float fireRate = 1;
    public bool holdToFire;
    public WepType type;
    public override void PickUp()
    {
        Inventory inv = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        inv.weapons[(int)type] = this;
        inv.Equip(true);
    }
}

[CreateAssetMenu(fileName = "Melee", menuName = "Weapons/Melee", order = 1)]
public class MeleeWep : Weapon
{
    [Tooltip("Swings Per Second")]
    public float arcRadius = 1;
    public float arcAngle = 10;

    public float knockBackForce = 1;
    public float knockBackAngle = 10;
}

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Ranged", order = 1)]
public class RangedWep : Weapon
{
    public AmmoType ammoType;
    public int ammoCap = 5;
    [Tooltip("Seconds for full reload")]
    public float reloadSpeed = 1;
}

