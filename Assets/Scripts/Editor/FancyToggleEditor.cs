using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(FancyToggleButton))]
[CanEditMultipleObjects]
public class FancyToggleButtonEditor : Editor
{
    SerializedProperty toggleEvents;
    SerializedProperty toggleTransition;
    private void OnEnable()
    {
        toggleEvents = serializedObject.FindProperty("onValueChanged");
        toggleTransition = serializedObject.FindProperty("toggleTransition");
    }

    public override void OnInspectorGUI()
    {
        FancyToggleButton toggle = (FancyToggleButton)target;

        toggle.backgroundImage = EditorGUILayout.ObjectField("Toggle background image", toggle.backgroundImage, typeof(Image), true) as Image;
        toggle.normalColor = EditorGUILayout.ColorField("Normal background color", toggle.normalColor);
        toggle.pressedColor = EditorGUILayout.ColorField("Selected background color", toggle.pressedColor);


        toggle.borderImage = EditorGUILayout.ObjectField("Toggle border image", toggle.borderImage, typeof(Image), true) as Image;
        toggle.hoverBorderSize = EditorGUILayout.Vector2Field("Toggle border hover size", toggle.hoverBorderSize);

        toggle.buttonsGroup = EditorGUILayout.ObjectField("Panel", toggle.buttonsGroup, typeof(Canvas), true) as Canvas;
        toggle.childrenToggleGroup = EditorGUILayout.ObjectField("Children toggle group", toggle.childrenToggleGroup, typeof(ToggleGroup), true) as ToggleGroup;

        toggle.interactable = EditorGUILayout.Toggle("Interactable", toggle.interactable);

        toggle.isOn = EditorGUILayout.Toggle("Is on",toggle.isOn);

        this.serializedObject.Update();
        EditorGUILayout.PropertyField(toggleTransition, new GUIContent("Toggle transition"));
        this.serializedObject.ApplyModifiedProperties();

        toggle.toggleGroup = EditorGUILayout.ObjectField("Toggle group", toggle.toggleGroup, typeof(ToggleGroup), true) as ToggleGroup;
        toggle.graphic = EditorGUILayout.ObjectField("Checkmark graphic", toggle.graphic, typeof(Image), true) as Image;

        this.serializedObject.Update();
        EditorGUILayout.PropertyField(toggleEvents, new GUIContent("On Value Changed (bool)"));
        this.serializedObject.ApplyModifiedProperties();
        // Show default inspector property editor
        //DrawDefaultInspector();
    }
}
