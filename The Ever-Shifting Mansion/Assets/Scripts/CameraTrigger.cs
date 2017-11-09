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
            player.amIn.Add(this);
            if (!player.currentCamera)
            {
                player.currentCamera = connectedCamera;
                connectedCamera.gameObject.SetActive(true);
            }

            //connectedCamera.gameObject.SetActive(true);


            //if (!player.PreviousCamera)
            //{
            //    player.PreviousCamera = player.currentCamera;
            //}

            //if (player.currentlyInCamera)
            //{
            //    player.currentlyInCamera.GetComponent<AudioListener>().enabled = false;
            //    player.PreviousCamera.depth = 0;
            //}

            //player.currentCamera = connectedCamera;
            //player.currentCamera.depth = 10;
            //player.currentCamera.GetComponent<AudioListener>().enabled = true;
            //player.currentlyInCamera = connectedCamera;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            player.amIn.Remove(this);

            player.currentCamera = player.amIn[player.amIn.Count - 1].connectedCamera;
            if (!player.PreviousCamera)
                player.PreviousCamera = connectedCamera;

            connectedCamera.gameObject.SetActive(false);
            player.amIn[player.amIn.Count - 1].connectedCamera.gameObject.SetActive(true);
            //if (player.currentlyInCamera == connectedCamera)
            //{
            //    player.PreviousCamera = 
            //}           
            //connectedCamera.gameObject.SetActive(false);

        }
    }
}
