using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShipHull.HullTargets))]
public class HullTargetEditor : Editor
{

    private ShipHull.HullTargets instance;

    public void OnEnable()
    {
        this.instance = this.target as ShipHull.HullTargets;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Ship Builder");

        EditorGUILayout.BeginHorizontal();
        instance.shipComponentList = (ShipComponentList)EditorGUILayout.ObjectField("Component List", instance.shipComponentList, typeof(ShipComponentList), false);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();
        

        EditorUtility.SetDirty(this.instance);

    }

}
