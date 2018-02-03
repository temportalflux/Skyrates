using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Ship))]
public class ShipEditor : Editor
{
    private Ship instance;

    public void OnEnable()
    {
        this.instance = this.target as Ship;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Build"))
        {
            this.instance.Destroy();
            this.instance.Generate();
        }

        EditorUtility.SetDirty(this.instance);

    }
    

}
