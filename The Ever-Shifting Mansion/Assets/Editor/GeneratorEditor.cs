using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
[CustomEditor(typeof(MapGenScriptiable))]
public class GeneratorEditor : Editor
{
    ReorderableList usableRooms;
    ReorderableList SpecialRooms;
    protected virtual void OnEnable()
    {
        MapGenScriptiable gen = target as MapGenScriptiable;
        usableRooms = new ReorderableList(serializedObject, serializedObject.FindProperty("useableRooms"), true, true, true, true)
        {
            drawElementCallback = DrawUseable
        };
        SpecialRooms = new ReorderableList(serializedObject, serializedObject.FindProperty("specialRooms"), true, true, true, true)
        {
            drawElementCallback = DrawSpecial
        };
        SceneView.onSceneGUIDelegate += OnSceneGUI;
        gen.Initilise();
    }


    void DrawHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, "Useable Rooms");
    }
    void DrawUseable(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = usableRooms.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;
        float posX = rect.x;
        float width = EditorGUIUtility.currentViewWidth / 2;
        EditorGUI.PropertyField(new Rect(posX, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("room"), GUIContent.none);
        posX += width;
        width = EditorGUIUtility.currentViewWidth / 6;
        EditorGUI.TextField(new Rect(posX, rect.y, width, EditorGUIUtility.singleLineHeight), "Weight", GUIStyle.none);
        posX += width;
        width = EditorGUIUtility.currentViewWidth / 6;
        EditorGUI.PropertyField(new Rect(posX, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("chanceToPlace"), GUIContent.none);
    }
    void DrawSpecial(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = SpecialRooms.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;
        float posX = rect.x;
        float width = EditorGUIUtility.currentViewWidth / 2;
        EditorGUI.PropertyField(new Rect(posX, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("room"), GUIContent.none);
        posX += width;
        width = EditorGUIUtility.currentViewWidth / 6;
        EditorGUI.TextField(new Rect(posX, rect.y, width, EditorGUIUtility.singleLineHeight), "Has to place", GUIStyle.none);
        posX += width;
        width = EditorGUIUtility.currentViewWidth / 6;
        EditorGUI.PropertyField(new Rect(posX, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("needToBePlaced"), GUIContent.none);
    }
    void OnAdd(ReorderableList _list)
    {
        MapGenScriptiable gen = target as MapGenScriptiable;
        if (gen.startRoom != null)
        {
            RoomScriptable room = Instantiate(gen.startRoom);
            gen.useableRooms.Add(new MapGenScriptiable.RoomWithWeighting() { room = room, chanceToPlace = .5f });
        }
        else
            Debug.Log("Set StartRoom");
        EditorUtility.SetDirty(target);
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

        int i = 0;
        foreach (var room in gen.rooms)
        {
            Random.InitState(i);
            i++;
            Handles.color = Color.white;
            Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);


            for (int x = (int)room.posOnGrid.x; x < room.posOnGrid.x + room.Size.x; x++)
            {
                for (int y = (int)room.posOnGrid.y; y < room.posOnGrid.y + room.Size.y; y++)
                {
                    if (room.roomGrid[x - (int)room.posOnGrid.x, y - (int)room.posOnGrid.y])
                    {
                        color.a = .2f;
                        Handles.DrawSolidRectangleWithOutline(new Vector3[] { new Vector3(x, 0, y), new Vector3(x, 0, y + 1), new Vector3(x + 1, 0, y + 1), new Vector3(x + 1, 0, y) }, color, color);
                    }
                }
            }
            Vector3 lablePos = new Vector3(room.size.x / 2 + room.posOnGrid.x, 0, room.size.y / 2 + room.posOnGrid.y);
            Handles.Label(lablePos, room.distanceFromStart.ToString());
            foreach (var door in room.doors.Where(door => door.connectedScene != null))
            {
                color = Color.red;


                color.a = .2f;
                Vector2 pos = room.posOnGrid + door.GridPos;
                Handles.DrawSolidRectangleWithOutline(new Vector3[] { new Vector3(pos.x, 0, pos.y), new Vector3(pos.x, 0, pos.y + 1), new Vector3(pos.x + 1, 0, pos.y + 1), new Vector3(pos.x + 1, 0, pos.y) }, color, Color.blue);
                foreach (var otherDoor in door.connectedScene.doors.Where(otherDoor => otherDoor.connectedScene == room))
                {
                    Vector2 posO = door.connectedScene.posOnGrid + otherDoor.GridPos;
                    Handles.DrawLine(new Vector3(pos.x + .5f, 0, pos.y + .5f), new Vector3(posO.x + .5f, 0, posO.y + .5f));
                }

                //draw Door
                //
                Vector2 center = new Vector2(pos.x + .5f, pos.y + .5f);
                Vector2 doorCenter = center + (door.Direction() / 2);

                if (door.direction == RoomScriptable.EnumDirection.NORTH || door.direction == RoomScriptable.EnumDirection.SOUTH)
                    Handles.DrawSolidRectangleWithOutline(new Vector3[] {
                new Vector3(doorCenter.x - 0.4f, 0,doorCenter.y - 0.1f),
                new Vector3(doorCenter.x- 0.4f, 0, doorCenter.y +0.1f),
                new Vector3(doorCenter.x + 0.4f, 0, doorCenter.y + 0.1f),
                new Vector3(doorCenter.x + 0.4f, 0, doorCenter.y -0.1f) },
                        color, Color.blue);
                else
                    Handles.DrawSolidRectangleWithOutline(new Vector3[] {
                new Vector3(doorCenter.x - 0.1f, 0,doorCenter.y -0.4f),
                new Vector3(doorCenter.x- 0.1f, 0, doorCenter.y +0.4f),
                new Vector3(doorCenter.x +0.1f, 0, doorCenter.y +0.4f),
                new Vector3(doorCenter.x + 0.1f, 0, doorCenter.y -0.4f) },
                       color, Color.blue);



            }
        }
        Random.InitState(System.DateTime.Now.Second);

    }

    public override void OnInspectorGUI()
    {
        MapGenScriptiable gen = target as MapGenScriptiable;


        serializedObject.Update();
        usableRooms.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
        SpecialRooms.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
        EditorGUI.BeginChangeCheck();
        RoomScriptable startRoom = (RoomScriptable)EditorGUILayout.ObjectField(gen.startRoom, typeof(RoomScriptable), false);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(gen, "gen changed");
            EditorUtility.SetDirty(gen);
            gen.startRoom = startRoom;
        }
        EditorGUI.BeginChangeCheck();
        Vector3 size = EditorGUILayout.Vector2Field("Room Size", gen.Size);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(gen, "gen changed");
            EditorUtility.SetDirty(gen);
            gen.Size = new Vector2(Mathf.Round(size.x), Mathf.Round(size.y));
        }

        EditorGUI.BeginChangeCheck();
        int iterations = EditorGUILayout.IntField("Iterations", gen.iterations);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(gen, "gen changed");
            EditorUtility.SetDirty(gen);
            gen.iterations = iterations;
        }
        EditorGUI.BeginChangeCheck();
        int enemies = EditorGUILayout.IntField("Target enemies", gen.targetEnemies);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(gen, "gen changed");

            EditorUtility.SetDirty(gen);
            gen.targetEnemies = enemies;
        }
        if (GUILayout.Button("Generate Map"))
        {
            Undo.RecordObject(gen, "gen changed");
            EditorUtility.SetDirty(gen);
            gen.GenMap();
        }

    }
}
