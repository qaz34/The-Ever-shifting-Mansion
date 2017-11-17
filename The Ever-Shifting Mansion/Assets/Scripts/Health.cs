using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;
using InControl;
public class Health : MonoBehaviour
{
    int health;
    public int maxHealth;
    public int CurrentHealth
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
            if (GameObject.FindGameObjectWithTag("MapGen"))
                if (health <= 0)
                {
                    health = 0;
                    isDead = true;
                    if (tag == "Player")
                    {
                        var objs = FindObjectsOfType<GameObject>();
                        var keep = new List<GameObject>();
                        foreach (var obj in objs)
                        {
                            Destroy(obj);
                        }
                        SceneManager.LoadScene("GameOver");
                    }
                    else
                    {

                        GameObject.FindGameObjectWithTag("MapGen").GetComponent<MainMenuGen>().currentRoom.enemiesInRoom--;
                    }
                    gameObject.SetActive(false);
                }
        }
    }
    public bool isDead = false;
    // Use this for initialization
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    InputDevice device = InputManager.ActiveDevice;
    //    if (device.DPad.Left.WasPressed)
    //    {
    //        CurrentHealth -= 10;
    //    }
    //}
}
