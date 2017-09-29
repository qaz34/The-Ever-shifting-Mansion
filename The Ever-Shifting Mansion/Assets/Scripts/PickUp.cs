using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    void Update()
    {
        if(gameObject.tag == "weapon")
        {
            //currentWeapon = this;
        }
        if(gameObject.tag == "consumable")
        {
            //add 1 of this
        }

    }

}
