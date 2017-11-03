using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.UI;
[RequireComponent(typeof(Collider))]
public class ItemInScene : MonoBehaviour
{
    public Transform itemPos;
    public Item item;
    [HideInInspector]
    public bool isLooking = false;
    bool playerIsIn = false;
    void Start()
    {
        if (item)
            Spawn(item);
    }
    public void Spawn(Item _item)
    {
        item = _item;
        Instantiate(item.display, itemPos.transform.position, itemPos.transform.rotation, itemPos);
    }
    void Update()
    {
        InputDevice device = InputManager.ActiveDevice;
        if (playerIsIn && device.Action1.WasPressed && !isLooking)
        {
            if (item.typeOf != Type.NOTE)
            {
                GameObject.FindGameObjectWithTag("Inspect").GetComponent<Inspect>().BeginLook(item, this);
                isLooking = true;
                GameObject.FindGameObjectWithTag("Inspect").GetComponent<Inspect>().stopLookDelegate += StopLooking;
            }
            else
            {
                GameObject go = Instantiate(((StoryNote)item).noteCanvas);
                Instantiate(item.display, go.transform);
                go.GetComponentInChildren<Text>().text = item.description.text;
                GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterCont>().SetEnabled(false);
            }
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
