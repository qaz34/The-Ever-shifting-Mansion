using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(RoomScriptable))]
public class RoomEditor : Editor
{
    bool mouseDown = false;
    void newGrid(RoomScriptable room)
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
        if (!(room.roomGrid1D.Length > 0))
            newGrid(room);
        if (room.roomGrid.Grid == null)
            room.roomGrid = new RoomScriptable.DimensionalAnchor() { Grid = room.roomGrid1D, Columns = (int)room.Size.x, Rows = (int)room.Size.y };
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }
    protected virtual void OnDisable()
    {
        RoomScriptable room = target as RoomScriptable;
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }
    protected virtual void OnSceneGUI(SceneView sceneView)
    {
        RoomScriptable room = target as RoomScriptable;

        for (int x = 0; x <= room.Size.x; x++)
        {
            Handles.DrawLine(new Vector3(x, 0, 0), new Vector3(x, 0, room.Size.y));
        }
        for (int y = 0; y <= room.Size.y; y++)
        {
            Handles.DrawLine(new Vector3(0, 0, y), new Vector3(room.Size.x, 0, y));
        }

        Quaternion rotation = new Quaternion();
        rotation.eulerAngles = new Vector3(90, 0, 0);
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
                        if (door.posOnGrid == new Vector2(x, y))
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
            Handles.DrawSolidRectangleWithOutline(new Vector3[] { new Vector3(door.posOnGrid.x, 0, door.posOnGrid.y), new Vector3(door.posOnGrid.x, 0, door.posOnGrid.y + 1), new Vector3(door.posOnGrid.x + 1, 0, door.posOnGrid.y + 1), new Vector3(door.posOnGrid.x + 1, 0, door.posOnGrid.y) }, color, Color.blue);



            //draw Door
            //
            Vector2 center = new Vector2(door.posOnGrid.x + .5f, door.posOnGrid.y + .5f);
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
        }
        if (mouseDown)
        {
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

                            RoomScriptable.Door door = new RoomScriptable.Door(pos, dir);
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
            EditorUtility.SetDirty(room);
            room.Size = new Vector2(Mathf.Round(size.x), Mathf.Round(size.y));
            newGrid(room);
        }
        DrawDefaultInspector();
    }
}
