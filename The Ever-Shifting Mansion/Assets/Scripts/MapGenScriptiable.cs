using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Specialized;
[CreateAssetMenu(fileName = "Map Gen", menuName = "Map/Generator", order = 1)]

public class MapGenScriptiable : ScriptableObject
{
    [System.Serializable]
    public struct RoomWithWeighting
    {
        public RoomScriptable room;
        public float chanceToPlace;
    }
    [System.Serializable]
    public struct RoomTile
    {
        public RoomScriptable room;
        public bool door;
        public bool placed;
    }
    public List<RoomWithWeighting> useableRooms;
    public RoomScriptable startRoom;
    [HideInInspector]
    public bool[] grid1D;
    [HideInInspector]
    public Vector2 gridSize;
    public int iterations;
    [HideInInspector]
    public DimensionalAnchor grid;

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


    public void GenMap()
    {
        NewGrid();
        Vector2 startPos = new Vector2(Mathf.Ceil(gridSize.x / 2) - Mathf.Floor(startRoom.Size.x / 2), 0);
        if (startRoom.Size.x * 2 >= gridSize.x || startRoom.Size.y * 2 >= gridSize.y)
        {
            Debug.Log("Make Grid Bigger");
            return;
        }
        NewRoom(this, startRoom, startPos, 0);
        Debug.Log("Job Done!");
    }
    RoomScriptable RollDoor(MapGenScriptiable gen)
    {
        float percentile = 0;
        float full = 0;
        float percent = Random.Range(0, 100);
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
    void NewRoom(MapGenScriptiable gen, RoomScriptable currentRoom, Vector2 globalPos, int count)
    {
        if (count <= gen.iterations)
        {
            SetMap(gen, currentRoom, globalPos);
            RoomScriptable room = null;
            Vector2 pos = Vector2.zero;

            foreach (var hostDoor in currentRoom.doors)
            {
                bool fit = false;
                int i = 0;
                while (i < 10 && !fit)
                {
                    room = Instantiate(RollDoor(gen));
                    room.RotateTo((RoomScriptable.Rotated)(int)Random.Range(0, 3));
                    for (int it = 0; it < 10; it++)
                    {
                        foreach (var door in room.doors)
                        {
                            if ((hostDoor.Direction() + door.Direction()).magnitude == 0)
                            {
                                pos = globalPos + hostDoor.GridPos - door.GridPos + hostDoor.Direction();
                                if (CanFit(gen, room, pos))
                                {
                                    NewRoom(gen, room, pos, count + 1);
                                    fit = true;
                                    break;
                                }
                            }
                        }
                        if (!fit)
                            room.RotateTo((RoomScriptable.Rotated)(int)Random.Range(0, 4));
                        else
                            break;
                    }
                    i++;
                }
            }
        }
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
        {
            for (int y = (int)start.y; y < start.y + room.Size.y; y++)
            {
                if (x >= gen.Size.x || x < 0 || y >= gen.Size.y || y < 0)
                    return false;
                if (gen.grid[x, y] == true && room.roomGrid[x - (int)start.x, y - (int)start.y] == true)
                    return false;
            }
        }
        return true;

    }
}
