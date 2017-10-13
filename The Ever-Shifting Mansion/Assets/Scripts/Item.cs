using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    [Tooltip("Mesh")]
    public GameObject weaponPrefab;
}
[CreateAssetMenu(fileName = "Item", menuName = "Items/Ammo", order = 1)]
public class Ammo : Item
{
    public AmmoType type;
    public int amount;
}