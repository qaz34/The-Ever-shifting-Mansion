using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLoad : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject.FindGameObjectWithTag("MapGen").SendMessage("StartLoad");
    }
	

}
