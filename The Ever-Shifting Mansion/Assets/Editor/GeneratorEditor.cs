using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
[CustomEditor(typeof(MapGenScriptiable))]
public class GeneratorEditor : Editor
{
    ReorderableList list;
    protected virtual void OnEnable()
    {
        MapGenScriptiable gen = target as MapGenScriptiable;

        //list = new ReorderableList(serializedObject, serializedObject.FindProperty("useableRooms"), true, true, true, true);
        //list.drawHeaderCallback = (Rect rect) =>
        //{
        //    EditorGUI.LabelField(rect, "Useable Rooms");
        //};

        //list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        //{
        //    var element = list.serializedProperty.GetArrayElementAtIndex(index);
        //    rect.y += 2;
        //    EditorGUI.PropertyField(new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Type"), GUIContent.none);
        //    //EditorGUI.PropertyField(
        //    //    new Rect(rect.x + 60, rect.y, rect.width - 60 - 30, EditorGUIUtility.singleLineHeight),
        //    //    element.FindPropertyRelative("Prefab"), GUIContent.none);
        //    //EditorGUI.PropertyField(
        //    //    new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight),
        //    //    element.FindPropertyRelative("Count"), GUIContent.none);
        //};


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
        Handles.color = Color.black;
        for (int x = 0; x <= gen.Size.x; x++)
        {
            Handles.DrawLine(new Vector3(x, 0, 0), new Vector3(x, 0, gen.Size.y));
        }
        for (int y = 0; y <= gen.Size.y; y++)
        {
            Handles.DrawLine(new Vector3(0, 0, y), new Vector3(gen.Size.x, 0, y));
        }
        Handles.color = Color.white;
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
    }

    public override void OnInspectorGUI()
    {
        MapGenScriptiable gen = target as MapGenScriptiable;

        //serializedObject.Update();
        //list.DoLayoutList();
        //serializedObject.ApplyModifiedProperties();
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
            NewRoom(gen, gen.startRoom, startPos, 0);
            Debug.Log("Job Done!");
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

            foreach (var hostDoor in currentRoom.doors)
            {
                bool fit = false;
                int i = 0;
                while (i < 10 && !fit)
                {
                    room = Instantiate(RollDoor(gen));
                    room.RotateTo((RoomScriptable.Rotated)(int)Random.Range(0, 3));
                    for (int it = 0; it < 4; it++)
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
                            room.RotateTo((RoomScriptable.Rotated)(int)Random.Range(0, 3));
                        else
                            break;
                    }
                    room.Rotate0();
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
