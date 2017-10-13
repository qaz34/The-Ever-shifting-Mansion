using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
public class Inspect : MonoBehaviour
{
    public Camera viewCamera;
    public Item item;

    void LookAt()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        Instantiate(item.weaponPrefab, transform.position, new Quaternion(), transform);
    }
    void Update()
    {
        LookAt();
        InputDevice device = InputManager.ActiveDevice;
        Vector2 moveDir = new Vector2(device.LeftStick.Y, device.LeftStick.X);
        moveDir *= 500 * Time.deltaTime;
        transform.Rotate(moveDir, Space.World);
    }
}
