using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//It is common to create a class to contain all of your
//extension methods. This class must be static.
public static partial class ExtensionMethods
{

    public static void ScriptField(this Editor editor, ScriptableObject obj)
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

    public static void ScriptField(this Editor editor, MonoBehaviour obj)
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

}
