using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
    AudioSource audioSource;
    public int damage = 10;
    [Tooltip("Rounds per second")]
    public float fireRate = 1;
    public bool holdToFire;
    public WepType type;
    public AudioClip soundEffect;
    protected float lastFired;
    public override void PickUp()
    {
        Inventory inv = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        inv.weapons[(int)type] = this;
        inv.Equip(true);
    }
    public virtual bool Fire(Transform position) { return false; }



}


public class CoroutineStarter : MonoBehaviour
{
    public static CoroutineStarter instance;

    void Start()
    {
        instance = this;
    }
}




