using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Ship))]
public class ShipEditor : Editor
{
    private Ship _instance;

    public void OnEnable()
    {
        this._instance = this.target as Ship;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Build"))
        {
            this._instance.Destroy();
            this._instance.Generate();
        }

        EditorUtility.SetDirty(this._instance);

    }
    

}
