using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUpdate : MonoBehaviour
{

    public MapScriptiable map;
    // Use this for initialization
    void Start()
    {
        map.house.SetPixels(new Color[128 * 128]);
        map.house.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        map.ShowMap();
    }
}
