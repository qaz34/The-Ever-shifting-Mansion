using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPaperSelector : MonoBehaviour
{
    public List<Material> materials;
    void Start()
    {
        if (GameObject.FindGameObjectWithTag("MapGen"))
        {
            Random.InitState(GameObject.FindGameObjectWithTag("MapGen").GetComponent<MainMenuGen>().currentRoom.seed);
        }
        var toSwitch = GetComponentsInChildren<MeshRenderer>();
        Material mat = materials[Random.Range(0, materials.Count)];
        foreach (var to in toSwitch)
        {
            to.material = mat;
        }
        Random.InitState(System.DateTime.Now.Millisecond);
    }
}
