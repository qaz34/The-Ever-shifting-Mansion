using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
public class UIController : MonoBehaviour
{
    GameObject[] pauseObjects;
    public GameObject gameOverPanel;
    public GameObject PauseMenu;

    // Use this for initialization
    void Start()
    {
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
