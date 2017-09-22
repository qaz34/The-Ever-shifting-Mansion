using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[CreateAssetMenu(fileName = "Map Room", menuName = "Map/Room", order = 1)]
[System.Serializable]
public class RoomScriptable : ScriptableObject
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

    public enum EnumDirection
    {
        NORTH,
        EAST,
        SOUTH,
        WEST
    }
    [System.Serializable]
    public struct Door
    {
        public Door(Vector2 pos, EnumDirection dir)
        {
            posOnGrid = pos;
            direction = dir;
        }
        public Vector2 posOnGrid;
        public EnumDirection direction;
        public Vector2 Direction()
        {
            switch (direction)
            {
                case EnumDirection.NORTH:
                    return new Vector2(0, 1);
                case EnumDirection.EAST:
                    return new Vector2(1, 0);
                case EnumDirection.SOUTH:
                    return new Vector2(0, -1);
                case EnumDirection.WEST:
                    return new Vector2(-1, 0);
            }
            return new Vector2();
        }
    }

    public Scene roomScene;
    [HideInInspector]
    Vector2 size;
    public List<Door> doors = new List<Door>();
    public bool[] roomGrid1D = new bool[0];
    public DimensionalAnchor roomGrid;

    public Vector2 Size
    {
        get
        {
            return size;
        }
        set
        {
            Vector2 endSize;
            endSize = new Vector2(Mathf.Max(1, value.x), Mathf.Max(1, value.y));
            bool[] newArray = new bool[(int)endSize.x * (int)endSize.y];
            int smallX = (int)Mathf.Min(endSize.x, size.x);
            int smallY = (int)Mathf.Min(endSize.y, size.y);
            for (int x = 0; x < smallX; x++)
            {
                for (int y = 0; y < smallY; y++)
                {
                    if (endSize.x < size.x)
                        newArray[smallX * y + x] = roomGrid1D[(int)size.x * y + x];
                    else
                        newArray[(int)endSize.x * y + x] = roomGrid1D[(int)smallX * y + x];
                }
            }
            roomGrid1D = newArray;
            roomGrid = new DimensionalAnchor() { Grid = roomGrid1D, Rows = (int)endSize.y, Columns = (int)endSize.x };
  
            size = endSize;
        }
    }
}
