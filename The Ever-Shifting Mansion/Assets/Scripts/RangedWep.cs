using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Ranged", order = 1)]
public class RangedWep : Weapon
{
    public AmmoType ammoType;
    public int ammoCap = 5;
    [HideInInspector]
    public int left = 1000;
    [Tooltip("Seconds for full reload")]
    public float reloadSpeed = 1;
    [HideInInspector]
    public bool reloading = false;
    private void OnEnable()
    {
        left = ammoCap;
    }
    public override void Interact()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().currentlyEquipWeapon = (int)type;
        GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().Equip(true);
    }
    public bool Reload(Ammo ammo)
    {
        if (ammo.amount > 0)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().StartCoroutine(Reloading(ammo));
            return true;
        }
        return false;
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
    public override bool Fire(Transform position)
    {
        if (lastFired > Time.time)
            lastFired = 0;
        if (left > 0 && !reloading)
        {
            if (fireRate < Time.time - lastFired)
            {
                lastFired = Time.time;
                //fire animation, particals
                left--;
                RaycastHit hit;
                if (Physics.Raycast(position.position, position.forward, out hit))
                {
                    Health targetHealth = hit.transform.GetComponent<Health>();
                    if (targetHealth)
                    {
                        targetHealth.CurrentHealth -= damage;
                        Debug.Log("Gottem");
                    }
                }
                return true;
            }
        }
        else
        {
            if (!reloading)
                foreach (Ammo ammo in GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().ammo.Where(i => i.ammoType == ammoType))
                {
                    if (ammo.amount > 0)
                    {
                        GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>().SetTrigger("Reload");
                      
                        Reload(ammo);
                    }
                }
        }
        return false;
    }
    IEnumerator Reloading(Ammo ammo)
    {
        reloading = true;
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
