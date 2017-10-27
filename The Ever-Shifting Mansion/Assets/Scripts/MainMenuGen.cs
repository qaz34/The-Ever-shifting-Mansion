using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
public class MainMenuGen : MonoBehaviour
{
    public MapGenScriptiable mapGen;
    RoomScriptable roomLoading;
    public RoomScriptable currentRoom;
    AsyncOperation op;
    public bool transitionTime = false;
    public GameObject playerPrefab;
    GameObject player;
    public void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void NewGame()
    {
        MapGenScriptiable gen = Instantiate(mapGen);
        //instansiate new copies of the rooms into the gen
        gen.GenMap();
        mapGen = gen;
        DontDestroyOnLoad(mapGen);
        currentRoom = gen.rooms[0];
        player = Instantiate(playerPrefab);
        player.transform.position = new Vector3(-100, -100, -100);
        DontDestroyOnLoad(player);
        Load(gen.rooms[0]);
    }
    public void Load(RoomScriptable room)
    {
        roomLoading = room;
        SceneManager.LoadScene("DoorTransitionScene");

    }
    void StartLoad()
    {
        StartCoroutine(SceneLoading(roomLoading));
    }
    IEnumerator SceneLoading(RoomScriptable room)
    {
        op = SceneManager.LoadSceneAsync(room.connectedScene.name);
        op.allowSceneActivation = false;
        yield return op;
    }
    public void Set()
    {
        op.allowSceneActivation = true;
        SceneManager.activeSceneChanged += Loaded;
    }
    public void Loaded(Scene sc, Scene sc2)
    {
        SceneManager.activeSceneChanged -= Loaded;
        Vector3 spawnPos = new Vector3(roomLoading.size.x / 2, 0, 2);
        foreach (var door in roomLoading.doors.Where(i => i.connectedScene != null))
        {
            GameObject go = Instantiate(roomLoading.doorObject);
            go.transform.position = new Vector3(door.posOnGrid.x + .5f, roomLoading.doorObject.transform.localScale.y / 2, door.posOnGrid.y + .5f);
            if (door.connectedScene == currentRoom)
            {
                spawnPos = go.transform.position;
            }
            go.transform.Translate(new Vector3(door.Direction(false).x / 2, 0, door.Direction(false).y / 2));
            go.transform.Rotate(transform.up, 90 * (int)door.direction);
            go.GetComponentInChildren<DoorInScene>().connectedRoom = door.connectedScene;
        }

        currentRoom = roomLoading;
        player.transform.position = spawnPos;

        transitionTime = false;
        SpawnEnemies();

        Random.InitState(currentRoom.seed);
        //load seed
        var props = GameObject.FindGameObjectsWithTag("PropSpawn").ToList();
        foreach (var prop in props)
        {
            prop.GetComponent<ItemContainer>()?.RollRandom();
        }

        props = GameObject.FindGameObjectsWithTag("ItemSpawn").ToList();
        foreach (var item in currentRoom.spawnList)
        {
            var go = props[Random.Range(0, props.Count)];
            go.GetComponent<ItemInScene>().Spawn(item);

            //item.GetComponent<ItemInScene>()?.Spawn();
        }


        Random.InitState(System.DateTime.Now.Second);

        SpawnItems();
    }
    public void SpawnEnemies()
    {
        List<Spawner> spawner = GameObject.FindGameObjectWithTag("EnemySpawn")?.GetComponentsInChildren<Spawner>().ToList();
        if (spawner != null)
            for (int i = 0; i < roomLoading.enemiesInRoom; i++)
            {
                GameObject go = Instantiate(spawner[i].enemy);
                go.transform.position = spawner[i].transform.position;
                spawner.RemoveAt(i);
            }
    }
    public void SpawnItems()
    {
        List<Spawner> spawner = GameObject.FindGameObjectWithTag("ItemSpawn")?.GetComponentsInChildren<Spawner>().ToList();
        if (spawner != null)
            for (int i = 0; i < roomLoading.enemiesInRoom; i++)
            {
                GameObject go = Instantiate(spawner[i].enemy);
                go.transform.position = spawner[i].transform.position;
                spawner.RemoveAt(i);
            }
    }
}
