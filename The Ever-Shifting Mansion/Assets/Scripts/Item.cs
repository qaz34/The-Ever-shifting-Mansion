using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public enum Type
{
    WEAPON,
    AMMO,
    CONSUMABLE
}


public class Item : ScriptableObject
{

    [Tooltip("Mesh")]
    public GameObject weaponDisplay;
    public GameObject weaponInGame;
    public TextAsset description;
    public Type typeOf;
    public virtual void PickUp() { }
    public virtual void Interact() { }
}
