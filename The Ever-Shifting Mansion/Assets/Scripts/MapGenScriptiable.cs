using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Specialized;
using System.Linq;
[CreateAssetMenu(fileName = "Map Gen", menuName = "Map/Generator", order = 1)]

public class MapGenScriptiable : ScriptableObject
{
    public delegate void unPlace(RoomScriptable room);
    unPlace noFit;
    [System.Serializable]
    public struct RoomWithWeighting
    {
        public RoomScriptable room;
        public float chanceToPlace;
    }
    [System.Serializable]
    public struct WeaponTarget
    {
        public Weapon weapon;
        public float target;
    }
    [System.Serializable]
    public struct SpecialRoom
    {
        public RoomScriptable room;
        public int distanceAfter;
        public bool needToBePlaced;
    }
    public List<RoomScriptable> rooms;
    public List<RoomWithWeighting> useableRooms;
    public List<SpecialRoom> specialRooms = new List<SpecialRoom>();
    public List<WeaponTarget> neededWeapons = new List<WeaponTarget>();
    List<SpecialRoom> placed = new List<SpecialRoom>();
    public RoomScriptable startRoom;
    [HideInInspector]
    public bool[] grid1D;
    [HideInInspector]
    public Vector2 gridSize;
    public int iterations;
    [HideInInspector]
    public DimensionalAnchor grid;
    public int targetEnemies;

    public int targetConsumables;

    [System.Serializable]
    public struct DimensionalAnchor
    {
        public bool[] Grid;
        public int Columns, Rows;

        public bool this[int column, int row]
        {
            get
            {
                return Grid[Columns * row + column];
            }
            set
            {
                Grid[Columns * row + column] = value;
            }
        }
    }
    public void Initilise()
    {
        grid = new DimensionalAnchor() { Grid = grid1D, Columns = (int)gridSize.x, Rows = (int)gridSize.y };
        if (grid1D == null || grid1D.Length == 0)
        {
            grid1D = new bool[(int)gridSize.x * (int)gridSize.y];
            for (int x = 0; x < gridSize.x; x++)
                for (int y = 0; y < gridSize.y; y++)
                    grid[x, y] = false;
        }
    }
    public void NewGrid()
    {
        grid = new DimensionalAnchor() { Grid = grid1D, Columns = (int)gridSize.x, Rows = (int)gridSize.y };
        grid1D = new bool[(int)gridSize.x * (int)gridSize.y];
        for (int x = 0; x < gridSize.x; x++)
            for (int y = 0; y < gridSize.y; y++)
                grid[x, y] = false;

    }
    public Vector2 Size
    {
        get
        {
            return gridSize;
        }
        set
        {
            Vector2 endSize;
            endSize = new Vector2(Mathf.Max(1, value.x), Mathf.Max(1, value.y));
            bool[] newArray = new bool[(int)endSize.x * (int)endSize.y];
            int smallX = (int)Mathf.Min(endSize.x, gridSize.x);
            int smallY = (int)Mathf.Min(endSize.y, gridSize.y);
            for (int x = 0; x < smallX; x++)
            {
                for (int y = 0; y < smallY; y++)
                {
                    if (endSize.x < gridSize.x)
                        newArray[smallX * y + x] = grid1D[(int)gridSize.x * y + x];
                    else
                        newArray[(int)endSize.x * y + x] = grid1D[(int)smallX * y + x];
                }
            }
            grid1D = newArray;
            grid = new DimensionalAnchor() { Grid = grid1D, Rows = (int)endSize.y, Columns = (int)endSize.x };
            gridSize = endSize;
        }

    }

    void RandomEnemies()
    {
        int enemiesSpawned = 0;
        var roomsAvaliable = new List<RoomScriptable>(rooms);
        while (enemiesSpawned < targetEnemies && roomsAvaliable.Count > 0)
        {
            var room = roomsAvaliable[Random.Range(0, roomsAvaliable.Count)];
            if (room.enemiesInRoom < room.maxEnemies)
            {
                enemiesSpawned++;
                room.enemiesInRoom++;
            }
            else
                roomsAvaliable.Remove(room);
        }
    }
    void DeleteFromList(RoomScriptable room)
    {
        foreach (var roomS in placed)
        {
            if (roomS.room == room)
            {
                placed.Remove(roomS);
                break;
            }
        }
        noFit -= DeleteFromList;
    }
    Item GetItemWeighted(RoomScriptable room)
    {
        //var items = room.spawnableItems[Random.Range(0, room.spawnableItems.Count)];
        if (room.spawnableItems.Count > 0)
        {
            float percentile = 0;
            float full = 0;
            float percent = Random.Range(0, 100);
            foreach (var item in room.spawnableItems)
                full += item.weight;
            if (full == 0)
            {
                return room.spawnableItems[Random.Range(0, room.spawnableItems.Count)].item;
            }
            foreach (var item in room.spawnableItems)
            {
                percentile += item.weight;
                if ((percentile / full) * 100 > percent)
                {
                    return item.item;
                }
            }
            return room.spawnableItems[Random.Range(0, room.spawnableItems.Count)].item;
        }
        return null;
    }
    Item GetWepWeighted(RoomScriptable room)
    {
        if (room.spawnableWeps.Count > 0)
        {
            float percentile = 0;
            float full = 0;
            float percent = Random.Range(0, 100);
            foreach (var item in room.spawnableWeps)
                full += item.weight;
            if (full == 0)
            {
                return room.spawnableWeps[Random.Range(0, room.spawnableWeps.Count)].item;
            }
            foreach (var item in room.spawnableWeps)
            {
                percentile += item.weight;
                if ((percentile / full) * 100 > percent)
                {
                    return item.item;
                }
            }
            // return room.spawnableWeps[Random.Range(0, room.spawnableWeps.Count)].item;
        }
        return null;
    }

    void RandomConsumables()
    {
        int consumablesSpawned = 0;
        var roomsAvaliable = new List<RoomScriptable>(rooms);
        while (consumablesSpawned < targetConsumables && roomsAvaliable.Count > 0)
        {
            var room = roomsAvaliable[Random.Range(0, roomsAvaliable.Count)];
            if (room.spawnList.Count < room.maxItems && room.spawnableItems.Count > 0)
            {
                var item = Instantiate(GetItemWeighted(room));
                room.spawnList.Add(room.spawnList.Count, item);
            }
            else
                roomsAvaliable.Remove(room);
        }
    }
    bool RandomWeapons()
    {
        foreach (var wep in neededWeapons)
        {
            if (wep.weapon && wep.target != 0)
            {
                var avaliableRooms = new List<RoomScriptable>(rooms);
                var correctRooms = new List<RoomScriptable>();
                foreach (var room in avaliableRooms)
                    foreach (var sWep in room.spawnableWeps)
                        if (sWep.item == wep.weapon)
                        {
                            correctRooms.Add(room);
                            break;
                        }

                for (int i = 0; i < wep.target; i++)
                {
                    if (correctRooms.Count == 0)
                    {
                        return false;
                    }
                    var room = correctRooms[Random.Range(0, correctRooms.Count)];
                    if (room.spawnList.Count < room.maxItems)
                    {
                        var item = Instantiate(wep.weapon);
                        room.spawnList.Add(room.spawnList.Count, item);
                    }
                    else
                        correctRooms.Remove(room);
                }
            }
        }
        return true;
    }
    struct DoorRoom
    {
        public RoomScriptable room;
        public RoomScriptable.Door door;
    }
    void RandomMap()
    {
        rooms = new List<RoomScriptable>();
        placed = new List<SpecialRoom>();
        List<DoorRoom> doors = new List<DoorRoom>();
        Vector2 startPos = new Vector2(Mathf.Ceil(gridSize.x / 2) - Mathf.Floor(startRoom.Size.x / 2), 0);
        RoomScriptable room = Instantiate(startRoom);
        if (room.Size.x * 2 >= gridSize.x || room.Size.y * 2 >= gridSize.y)
        {
            Debug.Log("Make Grid Bigger");
            return;
        }
        rooms.Add(room);
        room.posOnGrid = startPos;
        //place room
        SetMap(this, room, startPos);
        //store doors
        foreach (var door in room.doors)
            doors.Add(new DoorRoom { room = room, door = door });

        //loop

        while (true)
        {
            //pop first door on list
            var door = doors[0];
            doors.RemoveAt(0);
            if (door.room.distanceFromStart < iterations)
            {
                RoomScriptable roomBase = RollDoor(this, door.room.distanceFromStart + 1);
                //place room
                room = Instantiate(roomBase);
                room.RotateTo((RoomScriptable.Rotated)Random.Range(0, 4));
                bool fit = false;
                for (int it = 0; it < 10; it++)
                {
                    foreach (var connectedDoor in room.doors)
                    {
                        if ((door.door.Direction() + connectedDoor.Direction()).magnitude == 0)
                        {
                            Vector2 pos = door.room.posOnGrid + door.door.GridPos - connectedDoor.GridPos + door.door.Direction();
                            if (CanFit(this, room, pos))
                            {
                                door.door.connectedScene = room;
                                connectedDoor.connectedScene = door.room;
                                room.posOnGrid = pos;
                                rooms.Add(room);
                                foreach (var doorInRoom in room.doors)
                                    doors.Add(new DoorRoom { room = room, door = doorInRoom });
                                room.distanceFromStart = door.room.distanceFromStart + 1;
                                fit = true;
                                SetMap(this, room, pos);
                                break;
                            }
                        }
                    }
                    if (!fit)
                        room.RotateTo((RoomScriptable.Rotated)Random.Range(0, 4));
                    else
                        break;
                }
                if (!fit)
                    noFit?.Invoke(roomBase);
            }
            if (doors.Count == 0)
            {
                break;
            }
        }
    }
    bool CheckMap()
    {
        if (iterations < 4)
            return true;
        foreach (var room in specialRooms.Where(i => !placed.Contains(i)))
        {
            if (room.needToBePlaced == true)
                return false;
        }
        return true;
    }
    public void ConnectDoors()
    {
        foreach (var roomA in rooms)
        {
            foreach (var doorA in roomA.doors.Where(i => !i.connectedScene))
            {
                foreach (var roomB in rooms.Where(i => i != roomA))
                {
                    if (Vector2.Distance(roomB.posOnGrid, roomA.posOnGrid) < Mathf.Max(roomA.Size.magnitude, roomB.Size.magnitude))
                    {
                        foreach (var doorB in roomB.doors.Where(i => !i.connectedScene))
                        {
                            if ((doorA.Direction() + doorB.Direction()).magnitude == 0)
                            {
                                Vector2 posA = roomA.posOnGrid + doorA.GridPos;
                                Vector2 posB = roomB.posOnGrid + doorB.GridPos;
                                if (Vector2.Distance(posA, posB) == 1)
                                {
                                    bool timeTobbbbbbbBreak = false;
                                    foreach (var doorsCheck in roomB.doors.Where(i => i != doorB && i.connectedScene == roomA))
                                        timeTobbbbbbbBreak = true;
                                    if (timeTobbbbbbbBreak)
                                        break;
                                    doorA.connectedScene = roomB;
                                    doorB.connectedScene = roomA;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    public void GiveSeed()
    {
        foreach (var room in rooms)
        {
            room.seed = Random.Range(0, int.MaxValue);
        }
    }
    public void GenMap()
    {
        int i = 0;
        while (true && i < 10)
        {
            i++;
            NewGrid();
            RandomMap();
            if (i < 10)
            {
                Debug.Log("Failed");
                if (!CheckMap())
                    continue;
                if (!RandomWeapons())
                    continue;
            }
            RandomEnemies();
            RandomConsumables();
            ConnectDoors();
            GiveSeed();
            break;
        }
    }
    RoomScriptable RollDoor(MapGenScriptiable gen, int distanceFromStart)
    {
        float percentile = 0;
        float full = 0;

        float percent = Random.Range(0, 100);
        if (percent > 80)
        {
            var rooms = new List<SpecialRoom>();
            foreach (var room in gen.specialRooms.Where(i => !placed.Contains(i)))
            {
                if (distanceFromStart >= room.distanceAfter)
                {
                    rooms.Add(room);
                }
            }
            foreach (var room in rooms)
            {
                percentile++;
                if ((percentile / rooms.Count - gen.placed.Count) * 100 > percent)
                {
                    placed.Add(room);
                    noFit += DeleteFromList;
                    return room.room;
                }
            }
        }


        percent = Random.Range(0, 100);
        foreach (var room in gen.useableRooms)
            full += room.chanceToPlace;
        foreach (var room in gen.useableRooms)
        {
            percentile += room.chanceToPlace;
            if ((percentile / full) * 100 > percent)
            {
                return room.room;
            }
        }
        return gen.useableRooms[Random.Range(0, gen.useableRooms.Count)].room;
    }
    //start is the client rooms 0,0 coord in global
    void SetMap(MapGenScriptiable gen, RoomScriptable room, Vector2 start)
    {
        for (int x = (int)start.x; x < start.x + room.Size.x; x++)
            for (int y = (int)start.y; y < start.y + room.Size.y; y++)
                if (!gen.grid[x, y])
                    gen.grid[x, y] = room.roomGrid[x - (int)start.x, y - (int)start.y];

    }
    //start is the client rooms 0,0 coord in global
    bool CanFit(MapGenScriptiable gen, RoomScriptable room, Vector2 start)
    {
        for (int x = (int)start.x; x < start.x + room.Size.x; x++)
            for (int y = (int)start.y; y < start.y + room.Size.y; y++)
            {
                if (x >= gen.Size.x || x < 0 || y >= gen.Size.y || y < 0)
                    return false;
                if (gen.grid[x, y] == true && room.roomGrid[x - (int)start.x, y - (int)start.y] == true)
                    return false;
            }


        return true;

    }
}
