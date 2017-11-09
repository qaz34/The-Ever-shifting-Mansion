using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.UI;
public class Inspect : MonoBehaviour
{
    public Item item;
    [HideInInspector]
    public bool justEntered;
    public Text inspectText;
    public float rotateSpeed;
    [HideInInspector]
    public ItemInScene thingInspecting;
    public Space rotateSpace;
    [HideInInspector]
    public bool looking = false;
    public delegate void StopLook(bool interact);
    public StopLook stopLookDelegate;

    void LookAt()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        Instantiate(item.display, transform.position, new Quaternion(), transform);
    }
    public void BeginLook(Item _item, ItemInScene itemInScene)
    {
        thingInspecting = itemInScene;
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        item = _item;
        GetComponentInParent<Camera>().enabled = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterCont>().SetEnabled(false);
        transform.rotation = new Quaternion();
        transform.Rotate(Vector3.up, 45);
        Instantiate(item.display, transform.position, new Quaternion(), transform);
        justEntered = true;
        inspectText.text = item.description.text;
        looking = true;
    }
    public void LeaveLook()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        //GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterCont>().SetEnabled(false);
        GetComponentInParent<Camera>().enabled = false;
        looking = false;
    }
    void Update()
    {
        if (looking)
        {
            InputDevice device = InputManager.ActiveDevice;
            Vector3 moveDir = new Vector3(device.LeftStick.Y, 0, device.LeftStick.X);
            moveDir *= rotateSpeed * 100 * Time.deltaTime;
            transform.Rotate(moveDir, rotateSpace);
            if (device.Action1.WasPressed && !justEntered)
            {
                if (thingInspecting)
                {
                    thingInspecting.isLooking = false;
                    item.PickUp();
                    GameObject.FindGameObjectWithTag("MapGen").GetComponent<MainMenuGen>().currentRoom.grabbedList.Add(thingInspecting.itemIndex);
                    Destroy(thingInspecting.gameObject);
                }
                LeaveLook();
                stopLookDelegate?.Invoke(true);
            }
            else if (device.Action2.WasPressed || device.MenuWasPressed && !justEntered)
            {
                if (thingInspecting)
                    thingInspecting.isLooking = false;
                LeaveLook();
                stopLookDelegate?.Invoke(false);
            }
            justEntered = false;
        }
    }
}