﻿using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

// SOURCE: https://forum.unity.com/threads/editor-tool-better-scriptableobject-inspector-editing.484393/
[CustomPropertyDrawer(typeof(ScriptableObject), true)]
public class ScriptableObjectDrawer : PropertyDrawer
{
    // Cached scriptable object editor
    private Editor editor = null;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Draw label
        EditorGUI.PropertyField(position, property, label, true);

        // Draw foldout arrow
        if (property.objectReferenceValue != null)
        {
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);
        }

        // Draw foldout properties
        if (property.isExpanded && property.objectReferenceValue != null)
        {
            // Make child fields be indented
            EditorGUI.indentLevel++;

            // Draw object properties
            if (!editor)
                Editor.CreateCachedEditor(property.objectReferenceValue, null, ref editor);
            try
            {
                editor.OnInspectorGUI();
            }
            catch (Exception)
            {

            }

            // Set indent back to what it was
            EditorGUI.indentLevel--;
        }
    }
}