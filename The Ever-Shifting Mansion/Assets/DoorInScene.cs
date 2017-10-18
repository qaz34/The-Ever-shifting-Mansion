using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using InControl;
public class DoorInScene : MonoBehaviour
{
    [HideInInspector]
    public RoomScriptable connectedRoom;
    AsyncOperation op;
    bool loading = false;
    bool wasPressed = false;
    bool started = false;

    public void LoadScene()
    {
        GameObject.FindGameObjectWithTag("MapGen").GetComponent<MainMenuGen>().Load(connectedRoom);
    }
    private void OnTriggerStay(Collider other)
    {
        InputDevice device = InputManager.ActiveDevice;
        if (device.Action1.IsPressed && !started)
        {
            wasPressed = true;
        }
        else
        {
            started = false;
            wasPressed = false;
        }
        if (other.tag == "Player" && device.Action1.WasPressed && wasPressed)
        {
            wasPressed = false;
            started = true;
            if (!loading)
            {
                loading = true;
                LoadScene();
            }
        }

    }
}
