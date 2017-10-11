using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuGen : MonoBehaviour
{
    public MapGenScriptiable mapGen;
    RoomScriptable roomLoading;
    AsyncOperation op;
    public void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void NewGame()
    {
        MapGenScriptiable gen = Instantiate(mapGen);
        gen.GenMap();
        Load(gen.rooms[0]);
    }
    public void Load(RoomScriptable room)
    {
        roomLoading = room;
        StartCoroutine(SceneLoading(room));
    }
    IEnumerator SceneLoading(RoomScriptable room)
    {
        op = SceneManager.LoadSceneAsync(room.connectedScene.name);
        op.allowSceneActivation = true;
        yield return op;
        foreach (var door in room.doors)
        {
            if (door.connectedScene != null)
            {
                GameObject go = Instantiate(room.doorObject);
                go.transform.position = new Vector3(door.posOnGrid.x + .5f, room.doorObject.transform.localScale.y / 2, door.posOnGrid.y + .5f);
                go.transform.Translate(new Vector3(door.Direction(false).x / 2, 0, door.Direction(false).y / 2));
                go.transform.Rotate(transform.up, 90 * (int)door.direction);
                go.GetComponentInChildren<DoorInScene>().connectedRoom = door.connectedScene;
            }
        }
    }
}
