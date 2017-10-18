using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    public bool dontDestroy;
    void Start()
    {
        if (dontDestroy)
            DontDestroyOnLoad(gameObject);
    }

}
