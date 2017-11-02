using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            if (health <= 0)
            {
                health = 0;
                isDead = true;
                if (tag == "player")
                {

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
