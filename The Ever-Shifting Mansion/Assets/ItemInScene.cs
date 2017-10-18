using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
public class ItemInScene : MonoBehaviour
{
    public Transform itemPos;
    public Item item;
    bool isLooking = false;
    bool playerIsIn = false;
    void Start()
    {
        Spawn(item);
    }
    public void Spawn(Item _item)
    {
        item = _item;
        Instantiate(item.weaponPrefab, itemPos.transform.position, itemPos.transform.rotation, itemPos);
    }
    void Update()
    {
        InputDevice device = InputManager.ActiveDevice;
        if (playerIsIn && device.Action2.WasPressed)
        {
            if (!isLooking)
            {
                Debug.Log("Start");
                GameObject.FindGameObjectWithTag("Inspect").GetComponent<Inspect>().BeginLook(item);
                isLooking = true;
            }
            else if (isLooking)
            {
                Debug.Log("End");
                GameObject.FindGameObjectWithTag("Inspect").GetComponent<Inspect>().LeaveLook();
                isLooking = false;
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerIsIn = false;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerIsIn = true;
        }
    }
}
