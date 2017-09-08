using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public Camera connectedCamera;
    private CharacterCont player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterCont>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            connectedCamera.gameObject.SetActive(true);
            player.PreviousCamera = player.currentCamera;
            if (player.PreviousCamera)
            {
                player.PreviousCamera.GetComponent<AudioListener>().enabled = false;
                player.PreviousCamera.depth = 0;
            }
            player.currentCamera = connectedCamera;
            player.currentCamera.depth = 10;

            player.currentCamera.GetComponent<AudioListener>().enabled = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (connectedCamera == player.currentCamera)
            {
                player.currentCamera = player.PreviousCamera;
                player.PreviousCamera = connectedCamera;
            }
            connectedCamera.gameObject.SetActive(false);
        }
    }
}
