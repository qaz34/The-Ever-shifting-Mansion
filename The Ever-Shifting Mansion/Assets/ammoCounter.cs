﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ammoCounter : MonoBehaviour
{
    void Start()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<CombatController>().ammoChanged += AmmoChange;
    }
    void Update()
    {

    }
    public void AmmoChange()
    {
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<CombatController>().equipWeapon.type != WepType.MELEE)
        {
            RangedWep wep = (RangedWep)GameObject.FindGameObjectWithTag("Player").GetComponent<CombatController>().equipWeapon;
            GetComponentInChildren<Text>().text = wep.name.Remove(wep.name.Length - 8, 7) + ": " + wep.left.ToString() + "/" + wep.ammoCap.ToString();
        }
        else
        {
            MeleeWep wep = (MeleeWep)GameObject.FindGameObjectWithTag("Player").GetComponent<CombatController>().equipWeapon;
            GetComponentInChildren<Text>().text = wep.name.Remove(wep.name.Length - 8, 7);
        }
    }
}
