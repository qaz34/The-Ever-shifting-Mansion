using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Map Gen", menuName = "Map/Generator", order = 1)]

public class MapGenScriptiable : ScriptableObject
{
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


    public List<RoomScriptable> useableRooms;
    public RoomScriptable startRoom;
    public bool[] grid1D;
    [HideInInspector]
    public Vector2 gridSize;
    public int iterations;

    public DimensionalAnchor grid;

    public void Initilise()
    {
        grid = new DimensionalAnchor() { Grid = grid1D, Columns = (int)gridSize.x, Rows = (int)gridSize.y };
        if (grid1D == null || grid1D.Length == 0)
        {
            grid1D = new bool[(int)gridSize.x * (int)gridSize.y];
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    grid[x, y] = false;
                }
            }
        }
    }
    public void NewGrid()
    {
        grid = new DimensionalAnchor() { Grid = grid1D, Columns = (int)gridSize.x, Rows = (int)gridSize.y };
        grid1D = new bool[(int)gridSize.x * (int)gridSize.y];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                grid[x, y] = false;
            }
        }
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
}
