using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

//It is common to create a class to contain all of your
//extension methods. This class must be static.
public static partial class ExtensionMethods
{
#if UNITY_EDITOR
    public static void DrawScriptField(this Editor editor, ScriptableObject obj)
    {
        // Draw script line
        GUI.enabled = false;
        EditorGUILayout.ObjectField(
            "Script",
            MonoScript.FromScriptableObject(obj),
            typeof(MonoScript), false
        );
        GUI.enabled = true;
    }

    public static void DrawScriptField(this Editor editor, MonoBehaviour obj)
    {
        // Draw script line
        GUI.enabled = false;
        EditorGUILayout.ObjectField(
            "Script",
            MonoScript.FromMonoBehaviour(obj),
            typeof(MonoScript), false
        );
        GUI.enabled = true;
    }

    public static bool DrawArray<T>(this Editor editor, 
        string label, ref T[] array,
        bool togglable = true,
        bool isToggled = true,
        Func<T, T> DrawBlock = null
    )
    {
        int size = array.Length;

        EditorGUILayout.BeginHorizontal();
        {
            if (togglable)
            {
                isToggled = EditorGUILayout.Foldout(isToggled, label);
            }
            else
            {
                EditorGUILayout.LabelField(label);
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            {
                size = EditorGUILayout.IntField(size);

                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    size++;
                }

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    size--;
                }
            }
            EditorGUILayout.EndHorizontal();

            System.Array.Resize(ref array, size);
        }
        EditorGUILayout.EndHorizontal();

        if (isToggled && DrawBlock != null)
        {
            for (int i = 0; i < size; i++)
            {
                array[i] = DrawBlock(array[i]);
            }
        }

        return isToggled;
    }

    public static bool DrawArray<T>(this Editor editor, string label, ref T[] array,
        bool toggle, bool allowToggle = true, bool doBlock = true, Func<T, string> GetFieldName = null) where T: Object
    {
        int size = array.Length;

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (allowToggle)
                {
                    toggle = EditorGUILayout.Foldout(toggle, label);
                }
                else
                {
                    EditorGUILayout.LabelField(label);
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            {
                size = EditorGUILayout.IntField(size);

                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    size++;
                }

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    size--;
                }
            }
            EditorGUILayout.EndHorizontal();

            System.Array.Resize(ref array, size);
        }
        EditorGUILayout.EndHorizontal();

        if (doBlock && toggle)
        {
            for (int i = 0; i < size; i++)
            {
                array[i] = (T)EditorGUILayout.ObjectField(GetFieldName(array[i]), array[i], typeof(T), true);
            }
        }

        return toggle;
    }
#endif

}
