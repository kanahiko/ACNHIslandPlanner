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
    SerializedProperty selectedColor;
    SerializedProperty borderColor;
    SerializedProperty normalColor;
    SerializedProperty pressedColor;

    private bool showDefaultInspector = true;
    
    private void OnEnable()
    {
        toggleEvents = serializedObject.FindProperty("onClick");
        selectedColor = serializedObject.FindProperty("selectedColor");
        borderColor = serializedObject.FindProperty("borderColor");
        normalColor = serializedObject.FindProperty("normalColor");
        pressedColor = serializedObject.FindProperty("pressedColor");
        //toggleTransition = serializedObject.FindProperty("toggleTransition");
    }

    public override void OnInspectorGUI()
    {
        FancyToggleButton toggle = (FancyToggleButton)target;

        toggle.backgroundImage = EditorGUILayout.ObjectField("Toggle background image", toggle.backgroundImage, typeof(Image), true) as Image;
        //toggle.normalColor = EditorGUILayout.ColorField("Normal background color", toggle.normalColor);
        //toggle.pressedColor = EditorGUILayout.ColorField("Selected background color", toggle.pressedColor);
        this.serializedObject.Update();
        EditorGUILayout.PropertyField(normalColor, new GUIContent("Normal background color"));
        this.serializedObject.ApplyModifiedProperties();
        
        this.serializedObject.Update();
        EditorGUILayout.PropertyField(pressedColor, new GUIContent("Selected background color"));
        this.serializedObject.ApplyModifiedProperties();

        toggle.borderImage = EditorGUILayout.ObjectField("Toggle border image", toggle.borderImage, typeof(Image), true) as Image;
        toggle.hoverBorderSize = EditorGUILayout.Vector2Field("Toggle border hover size", toggle.hoverBorderSize);

        toggle.buttonsGroup = EditorGUILayout.ObjectField("Panel", toggle.buttonsGroup, typeof(Canvas), true) as Canvas;
        toggle.childrenToggleGroup = EditorGUILayout.ObjectField("Children toggle group", toggle.childrenToggleGroup, typeof(ButtonToggleGroup), true) as ButtonToggleGroup;

        toggle.interactable = EditorGUILayout.Toggle("Interactable", toggle.interactable);

        toggle.isOn = EditorGUILayout.Toggle("Is on",toggle.isOn);

        /*this.serializedObject.Update();
        EditorGUILayout.PropertyField(toggleTransition, new GUIContent("Toggle transition"));
        this.serializedObject.ApplyModifiedProperties();*/

        toggle.toggleGroup = EditorGUILayout.ObjectField("Toggle group", toggle.toggleGroup, typeof(ButtonToggleGroup), true) as ButtonToggleGroup;
        toggle.graphic = EditorGUILayout.ObjectField("Checkmark graphic", toggle.graphic, typeof(Image), true) as Image;
        //toggle.borderColor= EditorGUILayout.ColorField("Border color", toggle.borderColor);
        //toggle.borderColor.a = 1;
        
        this.serializedObject.Update();
        EditorGUILayout.PropertyField(selectedColor, new GUIContent("Selected color"));
        this.serializedObject.ApplyModifiedProperties();

        this.serializedObject.Update();
        EditorGUILayout.PropertyField(borderColor, new GUIContent("Border color"));
        this.serializedObject.ApplyModifiedProperties();
        
        if (toggle.borderImage != null)
        {
            toggle.borderColor.a = 1;
            toggle.borderImage.color = toggle.borderColor;
        }
        /* if (toggle.backgroundImage != null)
         {
             toggle.backgroundImage.color = toggle.normalColor;
         }*/
        if (toggle.graphic != null)
        {
            toggle.selectedColor.a = 1;
            toggle.graphic.color = toggle.selectedColor;
        }
        this.serializedObject.ApplyModifiedProperties();
        
        if (toggleEvents != null)
        {
            this.serializedObject.Update();
            EditorGUILayout.PropertyField(toggleEvents, new GUIContent("On Click"));
            this.serializedObject.ApplyModifiedProperties();
        }
        this.serializedObject.ApplyModifiedProperties();
        showDefaultInspector = EditorGUILayout.Foldout(showDefaultInspector, "Show default inspector");
        // Show default inspector property editor
        if (showDefaultInspector)
        {
            DrawDefaultInspector();
        }
    }
}
