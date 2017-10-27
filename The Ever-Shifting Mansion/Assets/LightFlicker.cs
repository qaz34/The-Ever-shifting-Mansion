using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    float xPan = 0;
    public float speed = .05f;
    // Use this for initialization
    void Start()
    {
        xPan = Random.Range(0, 10);
    }

    // Update is called once per frame
    void Update()
    {
        xPan += speed;

        GetComponent<Light>().intensity = Mathf.PerlinNoise(xPan, xPan);
    }
}
