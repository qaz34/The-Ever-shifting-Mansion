using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using InControl;
using UnityEngine.UI;
public class MainMenuGen : MonoBehaviour
{
    public MapGenScriptiable mapGen;
    RoomScriptable roomLoading;
    public RoomScriptable currentRoom;
    AsyncOperation op;
    public bool transitionTime = false;
    public GameObject playerPrefab;
    public bool moving = true;
    GameObject player;
    public GameObject pauseMenu;

    public void Start()
    {
    }
    void Update()
    {
        if (op != null && op.progress > .8f && moving && InputManager.ActiveDevice.Action1.WasPressed)
        {
            Set();
        }
    }


    public void NewGame()
    {
        if (!GameObject.FindGameObjectWithTag("Player"))
        {
            StartCoroutine(ShowControls());
            StartCoroutine(loadMap());
        }
    }
    public void Load(RoomScriptable room)
    {
        roomLoading = room;
        SceneManager.LoadScene("DoorTransitionScene");
        moving = true;
    }
    void StartLoad()
    {
        StartCoroutine(SceneLoading(roomLoading));
    }
    IEnumerator SceneLoading(RoomScriptable room)
    {
        op = SceneManager.LoadSceneAsync(room.connectedSceneName);
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
        moving = false;
        foreach (var door in roomLoading.doors.Where(i => i.connectedScene != null))
        {
            GameObject go = Instantiate(roomLoading.doorObject);
            go.transform.position = new Vector3(door.posOnGrid.x + .5f, 0, door.posOnGrid.y + .5f);
            if (door.connectedScene == currentRoom)
            {
                spawnPos = go.transform.position;
                spawnPos.y = 0;
                player.transform.eulerAngles = Vector3.zero;
                player.transform.Rotate(transform.up, -90 * (int)door.direction);
            }
            go.transform.Translate(new Vector3(door.Direction(false).x / 2, 0, door.Direction(false).y / 2));
            go.transform.Rotate(transform.up, 90 * (int)door.direction);
            go.GetComponentInChildren<DoorInScene>().connectedRoom = door.connectedScene;
        }
        currentRoom = roomLoading;
        currentRoom.explored = true;
        player.GetComponent<CharacterCont>().amIn = new List<CameraTrigger>();
        player.transform.position = spawnPos;

        transitionTime = false;
        // SpawnEnemies();
        var props = GameObject.FindGameObjectsWithTag("PropSpawn").ToList();
        var itemSpawns = GameObject.FindGameObjectsWithTag("ItemSpawn").ToList();
        var itemSpawnsNoDestroy = new List<GameObject>();
        Random.InitState(currentRoom.seed);

        foreach (var item in currentRoom.spawnList)
        {
            if (itemSpawns.Count == 0)
                break;
            var itemSpawn = itemSpawns[Random.Range(0, itemSpawns.Count)];

            if (!currentRoom.grabbedList.Contains(item.Key))
            {
                itemSpawn.GetComponent<ItemInScene>().item = item.Value;
                itemSpawn.GetComponent<ItemInScene>().itemIndex = item.Key;
                itemSpawn.GetComponent<ItemInScene>().Spawn();
            }
            itemSpawns.Remove(itemSpawn);
            itemSpawnsNoDestroy.Add(itemSpawn);
        }
        foreach (var prop in props)
        {
            prop.GetComponent<ItemContainer>()?.RollRandom(itemSpawnsNoDestroy);
        }
        SpawnEnemies();

        Random.InitState(System.DateTime.Now.Second);

        //SpawnItems();
    }
    public void SpawnEnemies()
    {
        List<Spawner> spawner = GameObject.FindGameObjectWithTag("EnemySpawn")?.GetComponentsInChildren<Spawner>().ToList();
        if (spawner != null)
            for (int i = 0; i < roomLoading.enemiesInRoom; i++)
            {
                GameObject go = Instantiate(spawner[i].enemy);
                go.transform.position = spawner[i].transform.position;
                go.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
            }
    }
    IEnumerator ShowControls()
    {
        var go = Instantiate(pauseMenu);
        yield return new WaitForSeconds(1);
        while (true)
        {
            if (InputManager.ActiveDevice.AnyButton.IsPressed)
            {
                break;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
        Load(mapGen.rooms[0]);
        go.GetComponent<PauseMenu>().pauseCanvas.SetActive(false);
    }
    IEnumerator loadMap()
    {
        yield return null;
        MapGenScriptiable gen = Instantiate(mapGen);
        //Instantiate new copies of the rooms into the gen
        gen.GenMap();
        mapGen = gen;
        DontDestroyOnLoad(mapGen);
        currentRoom = gen.rooms[0];
        player = Instantiate(playerPrefab);
        player.transform.position = new Vector3(-100, -100, -100);
        DontDestroyOnLoad(player);
    }
}
