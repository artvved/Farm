using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FixedJoystick))]
public class JoystickEditor : Editor
{
    private Joystick current;
    private RectTransform rectTransform;

    private void OnEnable()
    {
        current = target as Joystick;
        rectTransform = current.GetComponent<RectTransform>();
    }

    private void OnSceneGUI()
    {
        Handles.color = Color.green;
        EditorGUI.BeginChangeCheck();
        float distance = Handles.RadiusHandle(Quaternion.identity, current.transform.position, rectTransform.rect.width * 0.5f * current.handleDistance);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(current, "Changed Handle Size");
            current.handleDistance = distance / (rectTransform.rect.width * 0.5f);
        }

        Handles.color = Color.red;
        EditorGUI.BeginChangeCheck();
        float dead = Handles.RadiusHandle(Quaternion.identity, current.transform.position, rectTransform.rect.width * 0.5f * current.deadZone);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(current, "Changed Dead Zone");
            current.deadZone = dead / (rectTransform.rect.width * 0.5f);
        }
    }
}