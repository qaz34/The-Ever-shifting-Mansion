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
    public bool alive = true;
    public int CurrentHealth
    {
        get
        {
            return health;
        }
        set
        {
            if (value < health)
            {
                GetComponent<Animator>().SetTrigger("Flinch");
            }
            health = value;
            if (GameObject.FindGameObjectWithTag("MapGen"))
                if (health <= 0)
                {
                    alive = false;
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
                        GetComponent<Animator>().SetBool("Alive", false);
                        GameObject.FindGameObjectWithTag("MapGen").GetComponent<MainMenuGen>().currentRoom.enemiesInRoom--;
                    }

                    //gameObject.SetActive(false);
                }
                else
                {
                    if (tag == "Player")
                    {
                        GetComponent<Animator>().SetFloat("health", health);
                    }
                }
        }
    }
    public bool isDead = false;
    // Use this for initialization
    void Start()
    {
        health = maxHealth;
        if (tag == "Player")
        {
            GetComponent<Animator>().SetFloat("health", health);
        }
    }

    //    void Update()
    //    {
    //        InputDevice device = InputManager.ActiveDevice;
    //        if (device.DPad.Left.WasPressed)
    //        {
    //            CurrentHealth -= 10;
    //        }
    //    }
}
