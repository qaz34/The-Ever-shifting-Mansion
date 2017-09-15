using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
[CustomEditor(typeof(MeleeWep))]
public class WeaponEditor : Editor
{
    ArcHandle m_ArcHandle = new ArcHandle();
    ArcHandle m_ArcHandleUp = new ArcHandle();
    protected virtual void OnEnable()
    {
        // arc handle has no radius handle by default
        m_ArcHandle.SetColorWithRadiusHandle(Color.white, 0.1f);
        m_ArcHandleUp.SetColorWithRadiusHandle(Color.white, 0.1f);
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }
    protected virtual void OnDisable()
    {
        // arc handle has no radius handle by default      
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }
    protected virtual void OnSceneGUI(SceneView sceneView)
    {
        MeleeWep wep = target as MeleeWep;
        m_ArcHandle.angle = wep.arcAngle;
        m_ArcHandle.radius = wep.arcRadius;

        m_ArcHandleUp.angle = wep.knockBackAngle;
        m_ArcHandleUp.radius = wep.knockBackForce;

        if (GameObject.FindGameObjectWithTag("Player") == null)
            return;
        Transform player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        Vector3 handleDirection = player.forward;
        Matrix4x4 handleMatrix = Matrix4x4.TRS(
                player.position,
                Quaternion.LookRotation(player.forward, player.up),
                Vector3.one
                );

        using (new Handles.DrawingScope(handleMatrix))
        {
            EditorGUI.BeginChangeCheck();
            m_ArcHandle.DrawHandle();
            if (EditorGUI.EndChangeCheck())
            {
                // record the target object before setting new values so changes can be undone/redone
                Undo.RecordObject(wep, "Change Projectile Properties");

                // copy the handle's updated data back to the target object
                wep.arcAngle = m_ArcHandle.angle;
                wep.arcRadius = m_ArcHandle.radius;
            }
        }
        handleMatrix = Matrix4x4.TRS(
                player.position,
                Quaternion.LookRotation(player.forward, -player.up),
                Vector3.one
                );
        using (new Handles.DrawingScope(handleMatrix))
        {
            EditorGUI.BeginChangeCheck();
            m_ArcHandle.DrawHandle();
            if (EditorGUI.EndChangeCheck())
            {
                // record the target object before setting new values so changes can be undone/redone
                Undo.RecordObject(wep, "Change Projectile Properties");

                // copy the handle's updated data back to the target object
                wep.arcAngle = m_ArcHandle.angle;
                wep.arcRadius = m_ArcHandle.radius;
            }
        }
        handleMatrix = Matrix4x4.TRS(
              player.position,
              Quaternion.LookRotation(player.forward, -player.right),
              Vector3.one
              );
        using (new Handles.DrawingScope(handleMatrix))
        {
            EditorGUI.BeginChangeCheck();
            m_ArcHandleUp.DrawHandle();
            if (EditorGUI.EndChangeCheck())
            {
                // record the target object before setting new values so changes can be undone/redone
                Undo.RecordObject(wep, "Change Projectile Properties");

                // copy the handle's updated data back to the target object
                wep.knockBackAngle = m_ArcHandleUp.angle;
                wep.knockBackForce = m_ArcHandleUp.radius;
            }
        }
    }
}
