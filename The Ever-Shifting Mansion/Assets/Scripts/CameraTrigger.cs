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
            if (!player.currentCamera || player.amIn.Count == 0)
            {
                player.currentCamera = connectedCamera;
                connectedCamera.gameObject.SetActive(true);
            }
            player.amIn.Add(this);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            player.amIn.Remove(this);
            if (player.amIn.Count > 0)
            {
                if (player.currentCamera == connectedCamera && player.amIn[player.amIn.Count - 1].connectedCamera != player.currentCamera)
                {
                    player.currentCamera = player.amIn[player.amIn.Count - 1].connectedCamera;
                    if (!player.PreviousCamera)
                        player.PreviousCamera = connectedCamera;

                    connectedCamera.gameObject.SetActive(false);
                    player.amIn[player.amIn.Count - 1].connectedCamera.gameObject.SetActive(true);
                }
            }
            if (player.amIn.Count == 0 || player.amIn[player.amIn.Count - 1].connectedCamera != player.currentCamera || player.currentCamera == null)
                connectedCamera.gameObject.SetActive(false);
        }
    }
}
