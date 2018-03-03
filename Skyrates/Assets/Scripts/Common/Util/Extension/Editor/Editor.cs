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

    public static bool DrawArray<T>(this Editor editor, string label, ref T[] array,
        bool togglable = true, bool isToggled = true,
        Func<T, int, T> DrawBlock = null)
    {
        bool[] toggles = new bool[0];
        return editor.DrawArray(label, ref array,
            togglable, isToggled,
            false, ref toggles,
            GetFieldName: null,
            DrawBlock: DrawBlock
        );
    }

    public static bool DrawArray<T>(this Editor editor, string label, ref T[] array,
        bool canToggleBlock, bool blockToggle,
        bool canToggleEntries, ref bool[] entryToggles,
        Func<T, int, string> GetFieldName = null,
        Func<T, int, T> DrawBlock = null)
    {
        int size = array.Length;

        if (entryToggles == null) entryToggles = new bool[size];

        EditorGUILayout.BeginHorizontal();
        {
            if (canToggleBlock)
            {
                blockToggle = EditorGUILayout.Foldout(blockToggle, label);
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
            System.Array.Resize(ref entryToggles, size);
        }
        EditorGUILayout.EndHorizontal();

        if (blockToggle && DrawBlock != null)
        {
            for (int i = 0; i < size; i++)
            {
                if (canToggleEntries) EditorGUI.indentLevel++;
                if (canToggleEntries)
                {
                    entryToggles[i] = EditorGUILayout.Foldout(entryToggles[i],
                        GetFieldName == null ? i.ToString() : GetFieldName(array[i], i));
                }
                if (!canToggleEntries || entryToggles[i])
                {
                    array[i] = DrawBlock(array[i], i);
                }
                if (canToggleEntries) EditorGUI.indentLevel--;
            }
        }

        return blockToggle;
    }

    public static bool DrawArray<T>(this Editor editor, string label, ref T[] array,
        bool toggle, bool allowToggle = true, bool doBlock = true, Func<T, int, string> GetFieldName = null) where T: Object
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
                array[i] = (T)EditorGUILayout.ObjectField(GetFieldName(array[i], i), array[i], typeof(T), true);
            }
        }

        return toggle;
    }
#endif

}
