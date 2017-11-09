using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using System.Linq;
public class InventoryDisplay : MonoBehaviour
{
    public struct ItemAndContainer
    {
        public GameObject itemInGame;
        public Item itemScript;
    }

    public List<Transform> slots;
    public GameObject inventoryCamera;
    public GameObject ammoPlaceBox;
    public Transform ammoParent;
    public GameObject AmmoNumber;
    List<ItemAndContainer> items = new List<ItemAndContainer>();
    int currentlySelected;
    public float selectSpeed = 1;
    float timeSinceSwitch;
    bool inspecting = false;
    bool inventoryOpen = false;
    void Update()
    {
        InputDevice device = InputManager.ActiveDevice;
        if (device.Action4.WasPressed && !inspecting)
        {
            ToggleInventory();
        }
        if (inventoryOpen)
        {
            if (items.Count == 0)
                return;
            if (device.LeftStick.Left && (Time.time - timeSinceSwitch) > selectSpeed && !inspecting)
            {
                timeSinceSwitch = Time.time;
                items[currentlySelected % items.Count].itemInGame.GetComponent<cakeslice.Outline>().enabled = false;
                currentlySelected = (currentlySelected + items.Count - 1) % items.Count;
                items[currentlySelected].itemInGame.GetComponent<cakeslice.Outline>().enabled = true;
            }
            else if (device.LeftStick.Right && (Time.time - timeSinceSwitch) > selectSpeed && !inspecting)
            {
                items[currentlySelected % items.Count].itemInGame.GetComponent<cakeslice.Outline>().enabled = false;
                currentlySelected = (currentlySelected + 1) % items.Count;
                items[currentlySelected].itemInGame.GetComponent<cakeslice.Outline>().enabled = true;
                timeSinceSwitch = Time.time;
            }
            if (device.LeftStick.Vector.magnitude == 0)
            {
                timeSinceSwitch = 0;
            }
            if (device.Action1.WasPressed && !inspecting)
            {
                GameObject.FindGameObjectWithTag("Inspect").GetComponent<Inspect>().stopLookDelegate += ToggleLook;
                inspecting = true;
                GameObject.FindGameObjectWithTag("Inspect").GetComponent<Inspect>().BeginLook(items[currentlySelected % items.Count].itemScript, null);
            }
        }
    }
    void ToggleLook(bool interact)
    {
        GameObject.FindGameObjectWithTag("Inspect").GetComponent<Inspect>().stopLookDelegate -= ToggleLook;
        if (interact)
            items[currentlySelected].itemScript.Interact();
        inspecting = false;
        TextMesh textMesh = items[currentlySelected].itemInGame.GetComponentInChildren<TextMesh>();
        if (textMesh)
        {
            textMesh.text = ((Ammo)items[currentlySelected].itemScript).amount.ToString();
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
            items = new List<ItemAndContainer>();
            for (int i = 0; i < slots.Count; i++)
            {
                foreach (Transform child in slots[i])
                    Destroy(child.gameObject);
                if (player.GetComponent<Inventory>().weapons[i])
                {
                    var go = Instantiate(player.GetComponent<Inventory>().weapons[i].display, slots[i].position, slots[i].rotation, slots[i]);
                    go.AddComponent<cakeslice.Outline>().enabled = false;
                    items.Add(new ItemAndContainer() { itemInGame = go, itemScript = player.GetComponent<Inventory>().weapons[i] });
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
                        if (Vector3.Distance(currentPos, pos) < ammo.display.transform.localScale.x * 5)
                        {
                            canBe = false;
                            break;
                        }
                    }
                    if (canBe)
                        break;
                }
                takenPositions.Add(currentPos);
                var ammoBox = Instantiate(ammo.display, new Vector3(x, ammoPlaceBox.transform.position.y, y), Quaternion.Euler(0, Random.Range(0, 360), 0), ammoParent);
                var ammoText = Instantiate(AmmoNumber, new Vector3(x, ammoPlaceBox.transform.position.y, y), Quaternion.Euler(90, 0, 0), ammoParent);
                ammoText.GetComponent<TextMesh>().text = ammo.amount.ToString();
                ammoText.transform.parent = ammoBox.transform;
                ammoBox.AddComponent<cakeslice.Outline>().enabled = false;
                items.Add(new ItemAndContainer() { itemInGame = ammoBox, itemScript = ammo });
            }
            if (items.Count > 0)
                items[currentlySelected % items.Count].itemInGame.GetComponent<cakeslice.Outline>().enabled = true;
        }
    }
}
