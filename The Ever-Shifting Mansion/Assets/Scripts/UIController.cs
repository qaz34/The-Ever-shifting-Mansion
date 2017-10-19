using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
public class UIController : MonoBehaviour

{
    GameObject[] pauseObjects;
    public GameObject pauseMenu;

    // Use this for initialization
    void Start()
    {
        Time.timeScale = 1;
        pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");
        HidePaused();
    }

    void Update()
    {
        InputDevice device = InputManager.ActiveDevice;
        if (device.MenuWasPressed && !GameObject.FindGameObjectWithTag("Inspect").GetComponent<Inspect>().looking)
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
                ShowPaused();
                Debug.Log("The gods have stopped time");
            }
            else if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
                HidePaused();
                Debug.Log("Time has been restored");
            }
        }

    }

    public void ShowPaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(true);
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
    }



    public void HidePaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(false);
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }



}
