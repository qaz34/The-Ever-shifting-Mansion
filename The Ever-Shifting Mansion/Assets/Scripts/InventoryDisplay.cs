using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
public class InventoryDisplay : MonoBehaviour
{
    public List<Transform> slots;
    public GameObject inventoryCamera;
    void Update()
    {
        InputDevice device = InputManager.ActiveDevice;
        if (device.Action4.WasPressed)
        {
            ToggleInventory();
        }
    }
    public void ToggleInventory()
    {
        inventoryCamera.SetActive(!inventoryCamera.activeSelf);
        var player = GameObject.FindGameObjectWithTag("Player");
        player.SendMessage("SetEnabled", !inventoryCamera.activeSelf);
        player.GetComponent<CharacterCont>().enabled = !inventoryCamera.activeSelf;
        for (int i = 0; i < slots.Count; i++)
        {
            foreach (Transform child in slots[i])
                Destroy(child.gameObject);
            if (player.GetComponent<Inventory>().weapons[i])
            {
                var go = Instantiate(player.GetComponent<Inventory>().weapons[i].weaponPrefab, slots[i].position, slots[i].rotation, slots[i]);
            }
        }
    }
}
