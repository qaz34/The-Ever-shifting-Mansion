using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
public class UIController : MonoBehaviour

{
    GameObject[] pauseObjects;
    public GameObject gameOverPanel;
    public GameObject inventory;
    public GameObject PauseMenu;
    public Camera connectedCamera;
    private CharacterCont player;


    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterCont>();
        player.currentCamera = connectedCamera;
        Time.timeScale = 1;
        pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");
        hidePaused();
    }

    void Update()
    {
        InputDevice device = InputManager.ActiveDevice;
        //uses the p and escape keys to pause and unpause the game
        if (device.MenuWasPressed)
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
                showPaused();
                Debug.Log("The gods have stopped time");
            }
            else if (Time.timeScale == 0)
            {
                hidePaused();
                Debug.Log("Time has been restored");
            }
        }
        if (device.Action4 || Input.GetKeyDown(KeyCode.Tab))
        {
            showInventory();
        }
    }

    public void showPaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(true);
            PauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void showInventory()
    {
        
    }

    public void hidePaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(false);
            PauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }



}
