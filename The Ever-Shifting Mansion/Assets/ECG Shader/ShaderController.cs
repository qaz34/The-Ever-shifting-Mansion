using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShaderController : MonoBehaviour
{

    Material mat;
    public float speedUp = 0;
    public Texture[] textures;

    [Serializable]
    public struct ECGSetting
    {
        public Texture texture;
        public AudioClip sound;
        public Color color;
        public float speed;
        public float threshold;
    }

    public ECGSetting[] settings;
    int currentSettingsIndex = -1;
    // pctHealth should be between 0 and 1
    int GetCurrentSettings(float pctHealth)
    {
        // error handling
        if (settings.Length == 0)
        {
            Debug.Log("No SETTINGS!!!");
            return -1;
        }

        int index = 0;
        while (index < settings.Length - 1 && pctHealth >= settings[index].threshold)
            index++;

        return index;
    }

    // Use this for initialization
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        mat.SetFloat("_Timer", speedUp * Time.time);
        int index = GetCurrentSettings((float)GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().CurrentHealth / (float)GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().maxHealth);
        if (currentSettingsIndex != index)
        {
            currentSettingsIndex = index;
            mat.color = settings[currentSettingsIndex].color;
            mat.mainTexture = settings[currentSettingsIndex].texture;
            speedUp = settings[currentSettingsIndex].speed;
        }
    }
}
