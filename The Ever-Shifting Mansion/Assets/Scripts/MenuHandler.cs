using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{

    public void VolumeControls(Slider slider)
    {
        slider.maxValue = 1;
        slider.minValue = 0;
        AudioListener.volume = slider.value;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
