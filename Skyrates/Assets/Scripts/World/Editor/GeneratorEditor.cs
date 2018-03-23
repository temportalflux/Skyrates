using System.Collections;
using System.Collections.Generic;
using Skyrates.World;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Generator))]
public class GeneratorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Build"))
        {
            ((Generator) this.target).Generate();
        }
    }

}
