using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public enum Type
{
    WEAPON,
    AMMO,
    CONSUMABLE,
    NOTE,
    MAP
}


public class Item : ScriptableObject
{
    [Tooltip("Mesh")]
    public GameObject display;
    public GameObject inGame;
    public TextAsset description;
    public Type typeOf;
    public virtual void PickUp() { }
    public virtual void Interact() { }
}
