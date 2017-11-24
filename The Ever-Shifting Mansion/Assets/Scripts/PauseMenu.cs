using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
public class PauseMenu : MonoBehaviour
{
    bool paused = false;
    public GameObject pauseCanvas;
    void Update()
    {
        if (InputManager.ActiveDevice.MenuWasPressed)
        {
            paused = !paused;
            if (paused)
            {
                Time.timeScale = 0;
                pauseCanvas.SetActive(true);
            }
            else
            {
                Time.timeScale = 1;
                pauseCanvas.SetActive(false);
            }
        }
    }
}
