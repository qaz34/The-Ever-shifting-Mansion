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
	}
}
