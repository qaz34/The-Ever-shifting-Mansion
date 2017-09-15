using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    void Update()
    {

    }
}
