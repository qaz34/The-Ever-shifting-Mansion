using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
public class InventoryDisplay : MonoBehaviour
{
    public List<Transform> slots;
    public GameObject inventoryCamera;
    public GameObject ammoPlaceBox;
    public Transform ammoParent;
    public GameObject AmmoNumber;
    List<GameObject> items = new List<GameObject>();
    int currentlySelected;
    public float selectSpeed = 1;
    float timeSinceSwitch;
    bool inventoryOpen = false;
    void Update()
    {
        InputDevice device = InputManager.ActiveDevice;

        if (device.Action4.WasPressed)
        {
            ToggleInventory();
        }
        if (inventoryOpen)
        {
            if (device.LeftStick.Left && (Time.time - timeSinceSwitch) > selectSpeed)
            {
                timeSinceSwitch = Time.time;
                items[currentlySelected % items.Count].GetComponent<cakeslice.Outline>().enabled = false;
                currentlySelected = (currentlySelected + items.Count - 1) % items.Count;
                items[currentlySelected].GetComponent<cakeslice.Outline>().enabled = true;
            }
            else if (device.LeftStick.Right && (Time.time - timeSinceSwitch) > selectSpeed)
            {
                items[currentlySelected % items.Count].GetComponent<cakeslice.Outline>().enabled = false;
                currentlySelected = (currentlySelected + 1) % items.Count;
                items[currentlySelected].GetComponent<cakeslice.Outline>().enabled = true;
                timeSinceSwitch = Time.time;
            }
            if (device.LeftStick.Vector.magnitude == 0)
            {
                timeSinceSwitch = 0;
            }
        }
    }
    public void ToggleInventory()
    {
        inventoryCamera.SetActive(!inventoryCamera.activeSelf);
        var player = GameObject.FindGameObjectWithTag("Player");
        player.SendMessage("SetEnabled", !inventoryCamera.activeSelf);
        inventoryOpen = inventoryCamera.activeSelf;
        if (inventoryOpen)
        {
            items = new List<GameObject>();
            for (int i = 0; i < slots.Count; i++)
            {
                foreach (Transform child in slots[i])
                    Destroy(child.gameObject);
                if (player.GetComponent<Inventory>().weapons[i])
                {
                    var go = Instantiate(player.GetComponent<Inventory>().weapons[i].weaponDisplay, slots[i].position, slots[i].rotation, slots[i]);
                    go.AddComponent<cakeslice.Outline>().enabled = false;
                    items.Add(go);
                }
            }
            List<Vector3> takenPositions = new List<Vector3>();

            foreach (Transform child in ammoParent)
                Destroy(child.gameObject);
            foreach (var ammo in player.GetComponent<Inventory>().ammo)
            {
                float x = Random.Range(ammoPlaceBox.transform.position.x - ammoPlaceBox.transform.localScale.x / 2, ammoPlaceBox.transform.position.x + ammoPlaceBox.transform.localScale.x / 2);
                float y = Random.Range(ammoPlaceBox.transform.position.z - ammoPlaceBox.transform.localScale.z / 2, ammoPlaceBox.transform.position.z + ammoPlaceBox.transform.localScale.z / 2);
                var currentPos = new Vector3(x, 0, y);
                int i = 0;
                while (i < 10)
                {
                    i++;
                    x = Random.Range(ammoPlaceBox.transform.position.x - ammoPlaceBox.transform.localScale.x / 2, ammoPlaceBox.transform.position.x + ammoPlaceBox.transform.localScale.x / 2);
                    y = Random.Range(ammoPlaceBox.transform.position.z - ammoPlaceBox.transform.localScale.z / 2, ammoPlaceBox.transform.position.z + ammoPlaceBox.transform.localScale.z / 2);
                    currentPos = new Vector3(x, 0, y);
                    bool canBe = true;
                    foreach (var pos in takenPositions)
                    {
                        if (Vector3.Distance(currentPos, pos) < ammo.weaponDisplay.transform.localScale.x * 5)
                        {
                            canBe = false;
                            break;
                        }
                    }
                    if (canBe)
                        break;
                }
                takenPositions.Add(currentPos);
                var ammoBox = Instantiate(ammo.weaponDisplay, new Vector3(x, 0, y), Quaternion.Euler(0, Random.Range(0, 360), 0), ammoParent);
                var ammoText = Instantiate(AmmoNumber, new Vector3(x, 0, y), Quaternion.Euler(90, 0, 0), ammoParent);
                ammoText.GetComponent<TextMesh>().text = ammo.amount.ToString();
                ammoBox.AddComponent<cakeslice.Outline>().enabled = false;
                items.Add(ammoBox);
            }
            items[currentlySelected % items.Count].GetComponent<cakeslice.Outline>().enabled = true;
        }
    }
}
