using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
public class NoteInspector : MonoBehaviour
{

    void Update()
    {
        InputDevice device = InputManager.ActiveDevice;
        if (device.Action1.WasPressed)
        {
            GameObject.FindGameObjectWithTag("Player").SendMessage("SetEnabled", true);
            Destroy(this);
        }
    }
}
