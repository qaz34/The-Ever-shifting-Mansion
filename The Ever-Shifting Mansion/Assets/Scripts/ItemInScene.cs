using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
public class ItemInScene : MonoBehaviour
{
    public Transform itemPos;
    public Item item;
    [HideInInspector]
    public bool isLooking = false;
    bool playerIsIn = false;
    void Start()
    {
        Spawn(item);
    }
    public void Spawn(Item _item)
    {
        item = _item;
        Instantiate(item.weaponDisplay, itemPos.transform.position, itemPos.transform.rotation, itemPos);
    }
    void Update()
    {
        InputDevice device = InputManager.ActiveDevice;
        if (playerIsIn && device.Action1.WasPressed && !isLooking)
        {
            GameObject.FindGameObjectWithTag("Inspect").GetComponent<Inspect>().BeginLook(item, this);
            isLooking = true;
            GameObject.FindGameObjectWithTag("Inspect").GetComponent<Inspect>().stopLookDelegate += StopLooking;
        }
    }
    void StopLooking(bool interact)
    {
        isLooking = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterCont>().SetEnabled(true);
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
