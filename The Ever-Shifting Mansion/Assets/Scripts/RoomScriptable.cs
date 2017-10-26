using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[CreateAssetMenu(fileName = "Map Room", menuName = "Map/Room", order = 1)]
public class RoomScriptable : ScriptableObject
{
    public struct ItemsWithWeight
    {
        public Item item;
        public float weight;
    }
    [HideInInspector]
    public Object connectedScene;
    [HideInInspector]
    public string connectedSceneName;
    public GameObject doorObject;
    [HideInInspector]
    public Vector2 size = new Vector2(1, 1);
    public List<Door> doors = new List<Door>();
    public bool[] roomGrid1D;
    public DimensionalAnchor roomGrid;
    [HideInInspector]
    public Rotated rotation;
    public Vector2 posOnGrid;
    public int maxEnemies;
    public int enemiesInRoom;
    public List<Item> spawnList = new List<Item>();
    public List<ItemsWithWeight> spawnableWeps;
    public List<ItemsWithWeight> spawnableItems;
    public int maxItems;
    public int distanceFromStart;
    [System.Serializable]
    public struct DimensionalAnchor
    {
        public bool[] Grid;
        public int Columns, Rows;
        public Rotated rotation;

        public bool this[int column, int row]
        {
            get
            {
                switch (rotation)
                {
                    case Rotated.ZERO:
                        return Grid[Columns * row + column];//

                    case Rotated.DEG90:
                        return Grid[Columns * column + (Columns - 1 - row)];//

                    case Rotated.DEG180:
                        return Grid[Columns * (Rows - 1 - row) + (Columns - 1 - column)];//

                    case Rotated.DEG270:
                        return Grid[Columns * (Rows - 1 - column) + row];

                    default:
                        return Grid[Columns * row + column];
                }
            }
            set
            {
                Grid[Columns * row + column] = value;
            }
        }
    }
    public enum Rotated
    {
        ZERO,
        DEG90,
        DEG180,
        DEG270,
    }

    public void RotateClockwise90()
    {
        roomGrid.rotation = (Rotated)(((int)roomGrid.rotation + 1) & 4);
        rotation = roomGrid.rotation;
        foreach (Door door in doors)
        {
            door.rotation = roomGrid.rotation;
        }
    }

    public void Rotate180()
    {
        roomGrid.rotation = (Rotated)(((int)roomGrid.rotation + 2) & 4);
        rotation = roomGrid.rotation;
        foreach (Door door in doors)
        {
            door.rotation = roomGrid.rotation;
        }
    }
    public void Rotate0()
    {
        roomGrid.rotation = Rotated.ZERO;
        rotation = roomGrid.rotation;
        foreach (Door door in doors)
        {
            door.rotation = roomGrid.rotation;
        }
    }
    public void RotateTo(Rotated _rotation)
    {
        roomGrid.rotation = _rotation;
        rotation = roomGrid.rotation;
        foreach (Door door in doors)
        {
            door.rotation = roomGrid.rotation;
        }
    }
    public void RotateAnitclockwise90()
    {
        roomGrid.rotation = (Rotated)(((int)roomGrid.rotation + 3) & 4);
        rotation = roomGrid.rotation;
        foreach (Door door in doors)
        {
            door.rotation = roomGrid.rotation;
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

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class Door
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {

        [HideInInspector, SerializeField]
        public Vector2 posOnGrid;
        public Rotated rotation;
        public EnumDirection direction;
        public Vector2 size;
        public RoomScriptable connectedScene;

        public Door(Vector2 pos, EnumDirection dir, Rotated rot)
        {
            posOnGrid = pos;
            direction = dir;
            rotation = rot;
        }
        public Vector2 GridPos
        {
            get
            {
                Vector2 posRotated = posOnGrid;
                switch (rotation)
                {
                    case Rotated.DEG90:
                        posRotated = new Vector2(posRotated.y, size.x - 1 - posRotated.x);
                        break;

                    case Rotated.DEG180:
                        posRotated = new Vector2(size.x - 1 - posRotated.x, size.y - 1 - posRotated.y);
                        break;

                    case Rotated.DEG270:
                        posRotated = new Vector2(size.y - 1 - posRotated.y, posRotated.x);
                        break;
                }

                return posRotated;
            }

            set
            {
                posOnGrid = value;
            }
        }

        public Vector2 Direction()
        {
            Vector2 posRotated = new Vector2(0, 1);
            switch (direction)
            {
                case EnumDirection.NORTH:
                    posRotated = new Vector2(0, 1);
                    break;
                case EnumDirection.EAST:
                    posRotated = new Vector2(1, 0);
                    break;
                case EnumDirection.SOUTH:
                    posRotated = new Vector2(0, -1);
                    break;
                case EnumDirection.WEST:
                    posRotated = new Vector2(-1, 0);
                    break;
            }
            switch (rotation)
            {
                case Rotated.DEG90:
                    posRotated = new Vector2(posRotated.y, -posRotated.x);
                    break;

                case Rotated.DEG180:
                    posRotated = new Vector2(-posRotated.x, -posRotated.y);
                    break;

                case Rotated.DEG270:
                    posRotated = new Vector2(-posRotated.y, posRotated.x);
                    break;
            }
            return posRotated;
        }
        public Vector2 Direction(bool rotated)
        {
            Vector2 posRotated = new Vector2(0, 1);
            switch (direction)
            {
                case EnumDirection.NORTH:
                    posRotated = new Vector2(0, 1);
                    break;
                case EnumDirection.EAST:
                    posRotated = new Vector2(1, 0);
                    break;
                case EnumDirection.SOUTH:
                    posRotated = new Vector2(0, -1);
                    break;
                case EnumDirection.WEST:
                    posRotated = new Vector2(-1, 0);
                    break;
            }
            return posRotated;
        }
        public override bool Equals(object obj)
        {
            var door = obj as Door;
            return door != null &&
                   EqualityComparer<Vector2>.Default.Equals(posOnGrid, door.posOnGrid) &&
                   rotation == door.rotation &&
                   direction == door.direction;

        }

    }
    public Vector2 Size
    {
        get
        {
            Vector2 posRotated = size;
            switch (rotation)
            {
                case Rotated.DEG90:
                    posRotated = new Vector2(posRotated.y, posRotated.x);
                    break;

                case Rotated.DEG180:
                    posRotated = new Vector2(posRotated.x, posRotated.y);
                    break;

                case Rotated.DEG270:
                    posRotated = new Vector2(posRotated.y, posRotated.x);
                    break;
            }
            return posRotated;
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
            bool done = false;
            while (!done)
            {
                done = true;
                foreach (Door door in doors)
                {
                    if (door.GridPos.x < endSize.x || door.GridPos.y < endSize.y)
                    {
                        done = false;
                        doors.Remove(door);
                        break;
                    }
                    else
                        door.size = endSize;
                }

            }
            size = endSize;
        }
    }
}
