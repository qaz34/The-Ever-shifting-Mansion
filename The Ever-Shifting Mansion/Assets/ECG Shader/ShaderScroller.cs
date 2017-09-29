using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderScroller : MonoBehaviour {

    Material mat;
    public float speedUp = 1;

	// Use this for initialization
	void Start () {
        mat = GetComponent<MeshRenderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
        mat.SetFloat("_Timer", speedUp * Time.time);
	}
}
