using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    float xPan = 0;
    public float speed = .05f;
    public float min, max;
    // Use this for initialization
    void Start()
    {
        xPan = Random.Range(0, 10);
    }

    // Update is called once per frame
    void Update()
    {
        xPan += speed * 100 * Time.deltaTime;
        float intensity = Mathf.Lerp(min, max, Mathf.PerlinNoise(xPan, xPan));
        GetComponent<Light>().intensity = intensity;
    }
}
