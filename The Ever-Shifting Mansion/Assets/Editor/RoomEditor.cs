using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
[CustomEditor(typeof(RoomScriptable))]
public class RoomEditor : Editor
{
    ReorderableList spawnItems;
    ReorderableList spawnWeps;
    bool mouseDown = false;
    void NewGrid(RoomScriptable room)
    {

        room.roomGrid = new RoomScriptable.DimensionalAnchor() { Grid = room.roomGrid1D, Columns = (int)room.Size.x, Rows = (int)room.Size.y };
        for (int x = 0; x < room.Size.x; x++)
        {
            for (int y = 0; y < room.Size.y; y++)
            {
                room.roomGrid[x, y] = false;
            }
        }
    }

    protected virtual void OnEnable()
    {
        RoomScriptable room = target as RoomScriptable;

        spawnItems = new ReorderableList(serializedObject, serializedObject.FindProperty("spawnableItems"), true, true, true, true)
        {
            drawElementCallback = DrawUseable,
            drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Spawnable Items");
            }
        };
        spawnWeps = new ReorderableList(serializedObject, serializedObject.FindProperty("spawnableWeps"), true, true, true, true)
        {
            drawElementCallback = DrawSpecial,
            drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Spawnable Weapons");
            }
        };

        if (room.roomGrid1D == null)
        {
            room.roomGrid1D = new bool[1];
            NewGrid(room);
            room.Size = new Vector2(1, 1);
            room.doors = new List<RoomScriptable.Door>();
        }
        if (!(room.roomGrid1D.Length > 0))
            NewGrid(room);
        if (room.roomGrid.Grid == null)
            room.roomGrid = new RoomScriptable.DimensionalAnchor() { Grid = room.roomGrid1D, Columns = (int)room.Size.x, Rows = (int)room.Size.y };
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }


    void DrawUseable(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = spawnItems.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;
        float posX = rect.x;
        float width = EditorGUIUtility.currentViewWidth / 2;
        EditorGUI.PropertyField(new Rect(posX, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("item"), GUIContent.none);
        posX += width;
        width = EditorGUIUtility.currentViewWidth / 6;
        EditorGUI.TextField(new Rect(posX, rect.y, width, EditorGUIUtility.singleLineHeight), "Weight", GUIStyle.none);
        posX += width;
        width = EditorGUIUtility.currentViewWidth / 6;
        EditorGUI.PropertyField(new Rect(posX, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("weight"), GUIContent.none);
    }
    void DrawSpecial(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = spawnWeps.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;
        float posX = rect.x;
        float width = EditorGUIUtility.currentViewWidth / 2;
        EditorGUI.PropertyField(new Rect(posX, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("item"), GUIContent.none);
        posX += width;
        width = EditorGUIUtility.currentViewWidth / 6;
        EditorGUI.TextField(new Rect(posX, rect.y, width, EditorGUIUtility.singleLineHeight), "Weight", GUIStyle.none);
        posX += width;
        width = EditorGUIUtility.currentViewWidth / 6;
        EditorGUI.PropertyField(new Rect(posX, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("weight"), GUIContent.none);
    }


    protected virtual void OnDisable()
    {

        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }
    protected virtual void OnSceneGUI(SceneView sceneView)
    {
        RoomScriptable room = target as RoomScriptable;
        Handles.color = Color.black;
        for (int x = 0; x <= room.Size.x; x++)
        {
            Handles.DrawLine(new Vector3(x, 0, 0), new Vector3(x, 0, room.Size.y));
        }
        for (int y = 0; y <= room.Size.y; y++)
        {
            Handles.DrawLine(new Vector3(0, 0, y), new Vector3(room.Size.x, 0, y));
        }
        Handles.color = Color.white;
        // Handles.color = Color.blue;
        for (int x = 0; x < room.Size.x; x++)
        {
            for (int y = 0; y < room.Size.y; y++)
            {
                if (room.roomGrid[x, y])
                {
                    bool set = true;
                    foreach (var door in room.doors)
                    {
                        if (door.GridPos == new Vector2(x, y))
                            set = false;
                    }
                    if (set)
                    {
                        Color color = Color.blue;
                        color.a = .2f;
                        Handles.DrawSolidRectangleWithOutline(new Vector3[] { new Vector3(x, 0, y), new Vector3(x, 0, y + 1), new Vector3(x + 1, 0, y + 1), new Vector3(x + 1, 0, y) }, color, Color.red);
                    }
                }
            }
        }
        foreach (var door in room.doors)
        {
            Color color = Color.red;
            color.a = .2f;
            Handles.DrawSolidRectangleWithOutline(new Vector3[] { new Vector3(door.GridPos.x, 0, door.GridPos.y), new Vector3(door.GridPos.x, 0, door.GridPos.y + 1), new Vector3(door.GridPos.x + 1, 0, door.GridPos.y + 1), new Vector3(door.GridPos.x + 1, 0, door.GridPos.y) }, color, Color.blue);



            //draw Door
            //
            Vector2 center = new Vector2(door.GridPos.x + .5f, door.GridPos.y + .5f);
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
        if (!ActiveEditorTracker.sharedTracker.isLocked)
            return;
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        if (Event.current.type == EventType.mouseDown && Event.current.button == 0)
        {
            mouseDown = true;
        }
        else if (Event.current.type == EventType.mouseUp && Event.current.button == 0)
        {
            mouseDown = false;
            EditorUtility.SetDirty(room);
            AssetDatabase.SaveAssets();
        }
        if (mouseDown && !(Event.current.alt))
        {
            Undo.RecordObject(room, "room changed");

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            //var ray = Camera.current.ScreenPointToRay(Event.current.mousePosition);
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.localScale *= 100;
            plane.layer = LayerMask.NameToLayer("TransparentFX");
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << plane.layer))
            {
                if (hit.point.x > 0 && hit.point.z > 0)
                    if (hit.point.x < room.Size.x && hit.point.z < room.Size.y)
                    {
                        room.roomGrid[(int)Mathf.Floor(hit.point.x), (int)Mathf.Floor(hit.point.z)] = true;
                        if (Event.current.control)
                            room.roomGrid[(int)Mathf.Floor(hit.point.x), (int)Mathf.Floor(hit.point.z)] = false;

                        if (Event.current.shift && Event.current.type == EventType.mouseDown)
                        {
                            Vector2 pos = new Vector2((int)Mathf.Floor(hit.point.x), (int)Mathf.Floor(hit.point.z));
                            Vector2 unitVec = new Vector2((hit.point.x % 1) - .5f, (hit.point.z % 1) - .5f);
                            RoomScriptable.EnumDirection dir = RoomScriptable.EnumDirection.NORTH;
                            if (unitVec.y < unitVec.x && unitVec.y > -unitVec.x)
                                dir = RoomScriptable.EnumDirection.EAST;
                            else if (unitVec.y > unitVec.x && unitVec.y > -unitVec.x)
                                dir = RoomScriptable.EnumDirection.NORTH;
                            else if (unitVec.y < unitVec.x && unitVec.y < -unitVec.x)
                                dir = RoomScriptable.EnumDirection.SOUTH;
                            else if (unitVec.y > unitVec.x && unitVec.y < -unitVec.x)
                                dir = RoomScriptable.EnumDirection.WEST;

                            RoomScriptable.Door door = new RoomScriptable.Door(pos, dir, RoomScriptable.Rotated.ZERO)
                            {
                                size = room.Size
                            };
                            if (room.doors.Contains(door))
                                room.doors.Remove(door);
                            else
                                room.doors.Add(door);
                        }
                    }
            }
            DestroyImmediate(plane);
        }
    }
    public override void OnInspectorGUI()
    {
        RoomScriptable room = target as RoomScriptable;

        EditorGUI.BeginChangeCheck();
        Vector3 size = EditorGUILayout.Vector2Field("Room Size", room.Size);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(room, "Scene changed");

            room.Size = new Vector2(Mathf.Round(size.x), Mathf.Round(size.y));
            NewGrid(room);
            EditorUtility.SetDirty(room);
        }

        serializedObject.Update();
        spawnItems.DoLayoutList();
        spawnWeps.DoLayoutList();
        serializedObject.ApplyModifiedProperties();


        EditorGUI.BeginChangeCheck();
        SceneAsset scene = (SceneAsset)EditorGUILayout.ObjectField("Scene object", (room.connectedScene == null) ? new SceneAsset() : room.connectedScene, typeof(SceneAsset), true);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(room, "Scene changed");

            room.connectedScene = scene;
            room.connectedSceneName = scene.name;
            EditorUtility.SetDirty(room);
        }
        EditorGUI.BeginChangeCheck();
        GameObject door = (GameObject)EditorGUILayout.ObjectField("Door object", room.doorObject, typeof(GameObject), false);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(room, "Scene changed");

            room.doorObject = door;
            EditorUtility.SetDirty(room);
        }
        EditorGUI.BeginChangeCheck();
        int enemies = EditorGUILayout.IntField("Max enemies", room.maxEnemies);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(room, "Scene changed");

            room.maxEnemies = enemies;
            EditorUtility.SetDirty(room);
        }
        EditorGUI.BeginChangeCheck();
        int items = EditorGUILayout.IntField("Max items", room.maxItems);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(room, "Scene changed");

            room.maxItems = items;
            EditorUtility.SetDirty(room);
        }
        AssetDatabase.SaveAssets();
    }
}
