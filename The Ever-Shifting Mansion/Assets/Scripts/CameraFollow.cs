using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public bool follow = false;
    private void LateUpdate()
    {
        if (!follow)
            return;
        var player = GameObject.FindGameObjectWithTag("Player");
        transform.forward = (player.transform.position + Vector3.up) - transform.position;
    }
}
