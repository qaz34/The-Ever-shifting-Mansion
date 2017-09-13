using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Ranged", order = 1)]
public class Weapon : ScriptableObject
{
    public int damage = 10;
    public int ammoCap = 5;
    [Tooltip("Rounds per second")]
    public float fireRate = 1;
    [Tooltip("Seconds for full reload")]
    public float reloadSpeed = 1;
    public bool holdToFire;
 
}
