using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(RightClickButton))]
[CanEditMultipleObjects]
public class RightClickButtonEditor : Editor
{
    SerializedProperty leftClickEvent;
    SerializedProperty rightClickEvent;
    
    private void OnEnable()
    {
        leftClickEvent = serializedObject.FindProperty("onLeftClick");
        rightClickEvent = serializedObject.FindProperty("onRightClick");
    }
    
    public override void OnInspectorGUI()
    {
        if (leftClickEvent != null)
        {
            this.serializedObject.Update();
            EditorGUILayout.PropertyField(leftClickEvent, new GUIContent("On Left Click"));
            this.serializedObject.ApplyModifiedProperties();
        }
        
        if (rightClickEvent != null)
        {
            this.serializedObject.Update();
            EditorGUILayout.PropertyField(rightClickEvent, new GUIContent("On Right Click"));
            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
