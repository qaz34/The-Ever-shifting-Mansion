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
    public GameObject mapInScene;
    public GameObject minimapPref;
    GameObject minimap;
    public MapScriptiable mapObject;
    MapScriptiable currentMap;
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
            if (!GameObject.FindGameObjectWithTag("MapGen").GetComponent<MainMenuGen>().moving)
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
                if (items[currentlySelected].itemScript.typeOf == Type.MAP)
                {
                    if (!minimap)
                    {
                        minimap = Instantiate(minimapPref);
                        GameObject.FindGameObjectWithTag("Player").GetComponent<CombatController>().ammoChanged?.Invoke();
                    }
                    else
                    {
                        Destroy(minimap);
                        minimap = null;
                    }
                }
                else
                {
                    GameObject.FindGameObjectWithTag("Inspect").GetComponent<Inspect>().stopLookDelegate += ToggleLook;
                    inspecting = true;
                    GameObject.FindGameObjectWithTag("Inspect").GetComponent<Inspect>().BeginLook(items[currentlySelected % items.Count].itemScript, null);
                }
            }
        }
        else if (device.RightBumper.WasPressed)
        {
            if (!GameObject.FindGameObjectWithTag("MapGen").GetComponent<MainMenuGen>().moving)
            {
                if (!currentMap)
                    currentMap = Instantiate(mapObject);
                inspecting = !inspecting;
                currentMap.Interact();
            }
        }
        if (GameObject.FindGameObjectWithTag("MapGen").GetComponent<MainMenuGen>().moving)
        {
            Destroy(currentMap);
            inspecting = false;
        }
    }
    void ToggleLook(bool interact)
    {
        GameObject.FindGameObjectWithTag("Inspect").GetComponent<Inspect>().stopLookDelegate -= ToggleLook;
        if (interact)
            items[currentlySelected].itemScript.Interact();
        inspecting = false;
        if (items[currentlySelected].itemScript.typeOf == Type.CONSUMABLE)
        {
            if (!GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().consumables.Contains((Consumable)items[currentlySelected].itemScript))
            {
                Destroy(items[currentlySelected].itemInGame);
                items.RemoveAt(currentlySelected);
                if (items.Count != 0)
                {
                    currentlySelected = (currentlySelected + items.Count - 1) % items.Count;
                    items[currentlySelected].itemInGame.GetComponent<cakeslice.Outline>().enabled = true;
                }
            }
        }
        else
        {
            if (items[currentlySelected].itemScript.typeOf == Type.WEAPON && ((Weapon)items[currentlySelected].itemScript).type != WepType.MELEE)
            {
                TextMesh textMesh = items[currentlySelected].itemInGame.GetComponentInChildren<TextMesh>();
                if (textMesh)
                {
                    textMesh.text = ((RangedWep)items[currentlySelected].itemScript).left.ToString();
                }
            }
            else
            {
                TextMesh textMesh = items[currentlySelected].itemInGame.GetComponentInChildren<TextMesh>();
                if (textMesh)
                {
                    textMesh.text = ((Ammo)items[currentlySelected].itemScript).amount.ToString();
                }
            }
        }
    }
    public void ToggleInventory()
    {
        mapInScene.GetComponent<cakeslice.Outline>().enabled = false;
        inventoryCamera.SetActive(!inventoryCamera.activeSelf);
        var player = GameObject.FindGameObjectWithTag("Player");
        player.SendMessage("SetEnabled", !inventoryCamera.activeSelf);
        inventoryOpen = inventoryCamera.activeSelf;
        if (inventoryOpen)
        {
            items = new List<ItemAndContainer>
            {
                new ItemAndContainer() { itemInGame = mapInScene, itemScript = Instantiate(mapObject) }
            };
            for (int i = 0; i < slots.Count; i++)
            {
                foreach (Transform child in slots[i])
                    Destroy(child.gameObject);
                if (player.GetComponent<Inventory>().weapons[i])
                {
                    var go = Instantiate(player.GetComponent<Inventory>().weapons[i].display, slots[i].position, slots[i].rotation, slots[i]);
                    go.AddComponent<cakeslice.Outline>().enabled = false;
                    if (player.GetComponent<Inventory>().weapons[i].type != WepType.MELEE)
                    {
                        var ammoText = Instantiate(AmmoNumber, new Vector3(go.transform.position.x, go.transform.position.y, go.transform.position.z), Quaternion.Euler(90, 0, 0), ammoParent);
                        ammoText.GetComponent<TextMesh>().text = ((RangedWep)player.GetComponent<Inventory>().weapons[i]).left.ToString();
                        ammoText.transform.parent = go.transform;
                    }
                    items.Add(new ItemAndContainer() { itemInGame = go, itemScript = player.GetComponent<Inventory>().weapons[i] });
                }
            }
            List<Vector3> takenPositions = new List<Vector3>();

            foreach (Transform child in ammoParent)
                Destroy(child.gameObject);
            foreach (var ammo in player.GetComponent<Inventory>().ammo)
            {
                float x = 0;
                float y = 0;
                Vector3 currentPos = new Vector3(0, 0, 0);
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
            foreach (var consummable in player.GetComponent<Inventory>().consumables)
            {

                float x = 0;
                float y = 0;
                Vector3 currentPos = new Vector3(0, 0, 0);
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
                        if (Vector3.Distance(currentPos, pos) < consummable.display.transform.localScale.x * 5)
                        {
                            canBe = false;
                            break;
                        }
                    }
                    if (canBe)
                        break;
                }
                takenPositions.Add(currentPos);
                var ammoBox = Instantiate(consummable.display, new Vector3(x, ammoPlaceBox.transform.position.y, y), Quaternion.Euler(0, Random.Range(0, 360), 90), ammoParent);
                ammoBox.AddComponent<cakeslice.Outline>().enabled = false;
                items.Add(new ItemAndContainer() { itemInGame = ammoBox, itemScript = consummable });
            }

            if (items.Count > 0)
                items[currentlySelected % items.Count].itemInGame.GetComponent<cakeslice.Outline>().enabled = true;
        }
    }
}
