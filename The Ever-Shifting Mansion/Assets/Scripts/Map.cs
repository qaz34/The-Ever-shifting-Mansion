using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using InControl;
public class Map : MonoBehaviour
{
    private void Update()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var mapGen = GameObject.FindGameObjectWithTag("MapGen").GetComponent<MainMenuGen>();
        Vector2 posRotated = new Vector2(player.transform.position.x, player.transform.position.z);
        switch (mapGen.currentRoom.rotation)
        {
            case RoomScriptable.Rotated.ZERO:
                break;
            case RoomScriptable.Rotated.DEG90:
                posRotated = new Vector2(posRotated.y, mapGen.currentRoom.size.x - 1 - posRotated.x);
                break;

            case RoomScriptable.Rotated.DEG180:
                posRotated = new Vector2(mapGen.currentRoom.size.x - 1 - posRotated.x, mapGen.currentRoom.size.y - 1 - posRotated.y);
                break;

            case RoomScriptable.Rotated.DEG270:
                posRotated = new Vector2(mapGen.currentRoom.size.y - 1 - posRotated.y, posRotated.x);
                break;
        }
        var posUnit = (mapGen.currentRoom.posOnGrid + posRotated) / 128;

        ((RectTransform)transform).anchoredPosition = Vector2.Scale(posUnit, ((RectTransform)transform.parent).sizeDelta);
        transform.eulerAngles = new Vector3(0, 0, (-player.transform.eulerAngles.y - ((int)mapGen.currentRoom.rotation * 90)));
    }
}
