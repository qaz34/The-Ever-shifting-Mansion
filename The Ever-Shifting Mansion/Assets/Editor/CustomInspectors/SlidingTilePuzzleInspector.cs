using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof (PuzzleBox.SlidingTilePuzzle))]
public class SlidingTilePuzzleInspector : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		PuzzleBox.SlidingTilePuzzle obj = (PuzzleBox.SlidingTilePuzzle)target;

		if (GUILayout.Button("Randomise Layout"))
		{
			obj.RandomTest();
		}

		if (obj.isFocused)
		{
			GUI.color = Color.green;
			if (GUILayout.Button("Enabled"))
			{
				obj.isFocused = false;
			}
			GUI.color = Color.white;
		}
		else
		{
			GUI.color = Color.red;
			if (GUILayout.Button("Disabled"))
			{
				obj.isFocused = true;
			}
			GUI.color = Color.white;
		}
	}
}
