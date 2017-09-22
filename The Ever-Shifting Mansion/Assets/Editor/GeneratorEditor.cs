using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(MapGenScriptiable))]
public class GeneratorEditor : Editor
{
    protected virtual void OnEnable()
    {
        MapGenScriptiable gen = target as MapGenScriptiable;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
        gen.Initilise();
    }
    protected virtual void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }
    protected virtual void OnSceneGUI(SceneView sceneView)
    {
        MapGenScriptiable gen = target as MapGenScriptiable;
        for (int x = 0; x <= gen.Size.x; x++)
        {
            Handles.DrawLine(new Vector3(x, 0, 0), new Vector3(x, 0, gen.Size.y));
        }
        for (int y = 0; y <= gen.Size.y; y++)
        {
            Handles.DrawLine(new Vector3(0, 0, y), new Vector3(gen.Size.x, 0, y));
        }

        for (int x = 0; x < gen.Size.x; x++)
        {
            for (int y = 0; y < gen.Size.y; y++)
            {
                if (gen.grid[x, y])
                {
                    Color color = Color.blue;
                    color.a = .2f;
                    Handles.DrawSolidRectangleWithOutline(new Vector3[] { new Vector3(x, 0, y), new Vector3(x, 0, y + 1), new Vector3(x + 1, 0, y + 1), new Vector3(x + 1, 0, y) }, color, Color.red);
                }
            }
        }
        //foreach (var door in room.doors)
        //{
        //    Color color = Color.red;
        //    color.a = .2f;
        //    Handles.DrawSolidRectangleWithOutline(new Vector3[] { new Vector3(door.posOnGrid.x, 0, door.posOnGrid.y), new Vector3(door.posOnGrid.x, 0, door.posOnGrid.y + 1), new Vector3(door.posOnGrid.x + 1, 0, door.posOnGrid.y + 1), new Vector3(door.posOnGrid.x + 1, 0, door.posOnGrid.y) }, color, Color.blue);



        //    //draw Door
        //    //
        //    Vector2 center = new Vector2(door.posOnGrid.x + .5f, door.posOnGrid.y + .5f);
        //    Vector2 doorCenter = VectorAdd.Vector2Add.Vec2Add(center, (door.Direction() / 2));

        //    if (door.direction == RoomScriptable.EnumDirection.NORTH || door.direction == RoomScriptable.EnumDirection.SOUTH)
        //        Handles.DrawSolidRectangleWithOutline(new Vector3[] {
        //        new Vector3(doorCenter.x - 0.4f, 0,doorCenter.y - 0.1f),
        //        new Vector3(doorCenter.x- 0.4f, 0, doorCenter.y +0.1f),
        //        new Vector3(doorCenter.x + 0.4f, 0, doorCenter.y + 0.1f),
        //        new Vector3(doorCenter.x + 0.4f, 0, doorCenter.y -0.1f) },
        //            color, Color.blue);
        //    else
        //        Handles.DrawSolidRectangleWithOutline(new Vector3[] {
        //        new Vector3(doorCenter.x - 0.1f, 0,doorCenter.y -0.4f),
        //        new Vector3(doorCenter.x- 0.1f, 0, doorCenter.y +0.4f),
        //        new Vector3(doorCenter.x +0.1f, 0, doorCenter.y +0.4f),
        //        new Vector3(doorCenter.x + 0.1f, 0, doorCenter.y -0.4f) },
        //           color, Color.blue);


        //}

    }

    public override void OnInspectorGUI()
    {
        MapGenScriptiable gen = target as MapGenScriptiable;

        DrawDefaultInspector();
        EditorGUI.BeginChangeCheck();
        Vector3 size = EditorGUILayout.Vector2Field("Room Size", gen.Size);
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(gen);
            gen.Size = new Vector2(Mathf.Round(size.x), Mathf.Round(size.y));
        }
        if (GUILayout.Button("Generate Map"))
        {
            gen.NewGrid();
            Vector2 startPos = new Vector2(Mathf.Ceil(gen.gridSize.x / 2) - Mathf.Floor(gen.startRoom.Size.x / 2), 0);
            if (gen.startRoom.Size.x * 2 >= gen.gridSize.x || gen.startRoom.Size.y * 2 >= gen.gridSize.y)
            {
                Debug.Log("Make Grid Bigger");
                return;
            }
            foreach (var door in gen.startRoom.doors)
                NewRoom(gen, gen.startRoom, startPos, 0);
            Debug.Log("Job Done!");
            //RoomScriptable currentRoom = gen.startRoom;
            //Vector2 globalPosCurrent = startPos;
            //SetMap(gen, gen.startRoom, startPos);
            //foreach (var doorHost in currentRoom.doors)
            //{
            //    RoomScriptable testingRoom = gen.useableRooms[Random.Range(0, gen.useableRooms.Count)];
            //    foreach (var doorClient in testingRoom.doors)
            //    {
            //        if ((doorHost.Direction() + doorClient.Direction()).magnitude == 0)
            //        {
            //            Vector2 pos = globalPosCurrent + doorHost.posOnGrid - doorClient.posOnGrid + doorHost.Direction();
            //            if (CanFit(gen, testingRoom, pos))
            //            {
            //                Debug.Log("somehting fit");
            //                SetMap(gen, testingRoom, pos);
            //            }
            //        }
            //    }
            //}
        }

    }
    RoomScriptable RollDoor(MapGenScriptiable gen)
    {
        return gen.useableRooms[Random.Range(0, gen.useableRooms.Count)];
    }
    void NewRoom(MapGenScriptiable gen, RoomScriptable currentRoom, Vector2 globalPos, int count)
    {
        if (count <= gen.iterations)
        {
            SetMap(gen, currentRoom, globalPos);
            RoomScriptable room = null;
            Vector2 pos = Vector2.zero;
            int i = 0;
            foreach (var hostDoor in currentRoom.doors)
            {
                bool fit = false;
                while (i < 10 && !fit)
                {
                    room = RollDoor(gen);
                    foreach (var door in room.doors)
                    {
                        if ((hostDoor.Direction() + door.Direction()).magnitude == 0)
                        {
                            pos = globalPos + hostDoor.posOnGrid - door.posOnGrid + hostDoor.Direction();
                            if (CanFit(gen, room, pos))
                            {
                                NewRoom(gen, room, pos, count + 1);
                                fit = true;
                                break;
                            }
                        }
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
