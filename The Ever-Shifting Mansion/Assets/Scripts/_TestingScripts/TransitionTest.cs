using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionTest : MonoBehaviour
{
	[HideInInspector]
	public bool complete = false;

	public void Complete()
	{
		Debug.Log("Suck on my programming skills Cameron");
	}

	void Update()
	{
		if (complete) { Debug.Log("Who's the better programmer? That's right, Connor is!"); gameObject.SetActive(false); }
	}
}
