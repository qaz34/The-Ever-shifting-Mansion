using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public static class DumbDumbs
{
    [MenuItem("Dumb Designer Tools/Toggle Triggers")]
    static void ToggleTriggers()
    {
        var allObjects = Object.FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj.GetComponent<CameraTrigger>())
            {
                obj.GetComponent<MeshRenderer>().enabled = !obj.GetComponent<MeshRenderer>().enabled;
            }
        }
    }
}

