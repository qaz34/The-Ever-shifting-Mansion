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
    public virtual void Fire(Transform position)
    {
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
    public override void Fire(Transform position)
    {
        var hits = Physics.OverlapSphere(position.position, arcRadius, 1 << LayerMask.NameToLayer("Target"));
        foreach (var hit in hits)
        {
            if (Vector3.Angle(position.forward, hit.transform.position - position.position) < arcAngle)
            {
                hit.transform.GetComponent<Health>().CurrentHealth -= damage;
                Vector3 transformedVector = hit.transform.position - position.position;
                transformedVector.y = 0;
                transformedVector.Normalize();
                transformedVector.y = Mathf.Tan(Mathf.Deg2Rad * knockBackAngle);
                hit.transform.GetComponent<HuskAI>().KnockBack(transformedVector * knockBackForce);

                Debug.Log("WAM");
            }
        }
    }
}
public class CoroutineStarter : MonoBehaviour
{
    public static CoroutineStarter instance;

    void Start()
    {
        instance = this;
    }
}

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Ranged", order = 1)]
public class RangedWep : Weapon
{
    public AmmoType ammoType;
    public int ammoCap = 5;
    [HideInInspector]
    public int left = 5;
    [Tooltip("Seconds for full reload")]
    public float reloadSpeed = 1;
    [HideInInspector]
    public bool reloading = false;

    public override void Interact()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().currentlyEquipWeapon = (int)type;
        GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().Equip(true);
    }
    public void Reload(Ammo ammo)
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().StartCoroutine(Reloading(ammo));
    }
    public void InstantReload(Ammo ammo)
    {
        if (ammo.amount >= ammoCap - left)
        {
            ammo.amount -= ammoCap - left;
            left = ammoCap;
        }
        else
        {
            left = left + ammo.amount;
            ammo.amount = 0;
        }
    }
    public override void Fire(Transform position)
    {
        if (left > 0)
        {
            //fire animation, particals
            left--;
            RaycastHit hit;
            if (Physics.Raycast(position.position, position.forward, out hit))
            {
                Health targetHealth = hit.transform.GetComponent<Health>();
                if (targetHealth)
                    targetHealth.CurrentHealth -= damage;
            }
        }
        else
        {
            if (!reloading)
                foreach (Ammo ammo in GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().ammo.Where(i => i.ammoType == ammoType))
                {
                    if (ammo.amount > 0)
                    {
                        reloading = true;
                        Reload(ammo);
                    }
                }
        }
    }

    IEnumerator Reloading(Ammo ammo)
    {
        yield return new WaitForSeconds(reloadSpeed);
        if (ammo.amount >= ammoCap - left)
        {
            ammo.amount -= ammoCap - left;
            left = ammoCap;
        }
        else
        {
            left = left + ammo.amount;
            ammo.amount = 0;
        }
        reloading = false;
    }
}

