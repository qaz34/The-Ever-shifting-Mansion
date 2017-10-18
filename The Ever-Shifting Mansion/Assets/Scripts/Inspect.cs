using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
public class Inspect : MonoBehaviour
{
    public Item item;

    void LookAt()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        Instantiate(item.weaponPrefab, transform.position, new Quaternion(), transform);
    }
    public void BeginLook(Item _item)
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        item = _item;
        GetComponentInParent<Camera>().enabled = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterCont>().enabled = false;
        transform.rotation = new Quaternion();
        Instantiate(item.weaponPrefab, transform.position, new Quaternion(), transform);
    }
    public void LeaveLook()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterCont>().enabled = true;
        GetComponentInParent<Camera>().enabled = false;
    }
    void Update()
    {
        InputDevice device = InputManager.ActiveDevice;
        Vector2 moveDir = new Vector2(device.LeftStick.Y, device.LeftStick.X);
        moveDir *= 500 * Time.deltaTime;
        transform.Rotate(moveDir, Space.World);
    }
}
