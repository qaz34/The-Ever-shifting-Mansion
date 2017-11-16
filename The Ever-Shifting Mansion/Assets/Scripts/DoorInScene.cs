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

    bool insideTrigger = false;
    public void LoadScene()
    {
        GameObject.FindGameObjectWithTag("MapGen").GetComponent<MainMenuGen>().Load(connectedRoom);
    }

    private void Update()
    {
        InputDevice device = InputManager.ActiveDevice;
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterCont>().enabled)
            if (insideTrigger && device.Action1.WasPressed)
            {
                if (!loading)
                {
                    loading = true;
                    LoadScene();
                }
            }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            insideTrigger = true;
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            insideTrigger = false;
    }
}
