using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using InControl;
public class Door : MonoBehaviour
{
    [HideInInspector, SerializeField]
    public Object connectedScene;
    AsyncOperation op;
    bool loading = false;
    bool wasPressed = false;
    bool started = false;

    public VideoClip video;
    public GameObject canvas;
    public void LoadScene()
    {
        StartCoroutine(SceneLoading());
        StartCoroutine(VideoDone());
    }
    IEnumerator SceneLoading()
    {
        op = SceneManager.LoadSceneAsync(connectedScene.name);
        op.allowSceneActivation = false;
        yield return op;
    }
    IEnumerator VideoDone()
    {
        canvas.SetActive(true);
        yield return new WaitForSeconds((float)video.length);
        SwitchScene();
    }
    public void SwitchScene()
    {
        op.allowSceneActivation = true;
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
