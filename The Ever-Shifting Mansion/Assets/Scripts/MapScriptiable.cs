using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[CreateAssetMenu(fileName = "Map", menuName = "Items/Map", order = 1)]
public class MapScriptiable : Item
{
    public Texture2D house;
    Color[] pixels = new Color[128 * 128];
    [Range(0, 1)]
    public float alphaOfRooms = 0.01f;
    //public float currentRoomAlpha = 1;
    public GameObject canvas;
    GameObject canvasMade;
    bool showing = false;
    public override void Interact()
    {
        if (showing)
        {
            showing = false;
            Destroy(canvasMade);
        }
        else
        {
            showing = true;
            ShowMap();
            canvasMade = Instantiate(canvas);
        }
    }
    public void ShowMap()
    {
        if (!GameObject.FindGameObjectWithTag("MapGen").GetComponent<MainMenuGen>().currentRoom)
        {
            house.SetPixels(pixels);
            house.Apply();
            return;
        }
        // var go = Instantiate(canvas);
        //draw map on canvas
        var map = GameObject.FindGameObjectWithTag("MapGen").GetComponent<MainMenuGen>().mapGen;

        foreach (var room in map.rooms.Where(i => i.explored))
        {
            Debug.Log(1);
            Random.InitState(room.seed);
            Color color = new Color(Random.Range(0f, .9f), Random.Range(0f, .9f), Random.Range(0f, .9f), alphaOfRooms);
            if (room == GameObject.FindGameObjectWithTag("MapGen").GetComponent<MainMenuGen>().currentRoom)
            {
                color.a = 1;
            }
            for (int x = (int)room.posOnGrid.x; x < room.posOnGrid.x + room.Size.x; x++)
            {
                for (int y = (int)room.posOnGrid.y; y < room.posOnGrid.y + room.Size.y; y++)
                {
                    if (room.roomGrid[x - (int)room.posOnGrid.x, y - (int)room.posOnGrid.y])
                    {
                        pixels[x + y * 128] = color;
                    }
                }
            }
            foreach (var door in room.doors.Where(i => i.connectedScene))
            {
                Vector2 pos = room.posOnGrid + door.GridPos;
                pixels[(int)pos.x + (int)pos.y * 128] = new Color(1, 0, 0, color.a);
            }
        }
        Random.InitState(System.DateTime.Now.Millisecond);
        house.SetPixels(pixels);
        house.Apply();
    }
}
