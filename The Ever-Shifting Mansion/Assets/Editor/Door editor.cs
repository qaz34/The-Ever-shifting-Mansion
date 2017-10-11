using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(DoorInScene))]
public class DoorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DoorInScene door = (DoorInScene)target;
        //SceneAsset scene = (SceneAsset)EditorGUILayout.ObjectField("scene object", (door.connectedScene == null) ? new SceneAsset() : door.connectedScene, typeof(SceneAsset), true);
        //door.connectedScene = scene;
        DrawDefaultInspector();
    }
}
