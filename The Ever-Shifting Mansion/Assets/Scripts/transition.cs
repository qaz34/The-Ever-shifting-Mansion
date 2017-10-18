using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class transition : MonoBehaviour
{

    // Use this for initialization
    void OnEnable()
    {
        GameObject.FindGameObjectWithTag("MapGen").SendMessage("Set");
    }

}
