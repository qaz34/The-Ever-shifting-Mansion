using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
[CustomEditor(typeof(MapGenScriptiable))]
public class GeneratorEditor : Editor
{
    ReorderableList Prop;
    protected virtual void OnEnable()
    {
        MapGenScriptiable gen = target as MapGenScriptiable;
        Prop = new ReorderableList(serializedObject, serializedObject.FindProperty("useableRooms"), true, true, true, true)
        {
            drawElementCallback = DrawElementP
        };

        SceneView.onSceneGUIDelegate += OnSceneGUI;
        gen.Initilise();
    }


    void DrawHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, "Useable Rooms");
    }
    void DrawElementP(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = Prop.serializedProperty.GetArrayElementAtIndex(index);
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
    void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        MapGenScriptiable gen = target as MapGenScriptiable;
        RoomScriptable room = gen.useableRooms[index].room;
        rect.y += 2;
        EditorGUI.ObjectField(new Rect(rect.x, rect.y, 240, EditorGUIUtility.singleLineHeight), room, typeof(RoomScriptable), false);
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


        serializedObject.Update();
        Prop.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
        EditorGUI.BeginChangeCheck();
        RoomScriptable startRoom = (RoomScriptable)EditorGUILayout.ObjectField(gen.startRoom, typeof(RoomScriptable), false);
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(gen);
            gen.startRoom = startRoom;
        }
        EditorGUI.BeginChangeCheck();
        Vector3 size = EditorGUILayout.Vector2Field("Room Size", gen.Size);
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(gen);
            gen.Size = new Vector2(Mathf.Round(size.x), Mathf.Round(size.y));
        }

        EditorGUI.BeginChangeCheck();
        int iterations = EditorGUILayout.IntField("Iterations", gen.iterations);
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(gen);
            gen.iterations = iterations;
        }

        if (GUILayout.Button("Generate Map"))
        {
            gen.GenMap();
        }

    }
}
