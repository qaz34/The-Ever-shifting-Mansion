using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Door))]
public class DoorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        Door door = (Door)target;
        SceneAsset scene = (SceneAsset)EditorGUILayout.ObjectField("scene object", door.connectedScene, typeof(SceneAsset), true);
        door.connectedScene = scene;
        DrawDefaultInspector();
    }
}
