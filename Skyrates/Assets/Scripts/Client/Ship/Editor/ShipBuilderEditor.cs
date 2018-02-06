﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ComponentType = ShipData.ComponentType;

[CustomEditor(typeof(ShipBuilder))]
public class ShipBuilderEditor : Editor
{

    private ShipBuilder instance;

    public void OnEnable()
    {
        this.instance = this.target as ShipBuilder;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Ship Builder");

        EditorGUILayout.BeginHorizontal();
        instance.shipComponentList = (ShipComponentList)EditorGUILayout.ObjectField("Component List", instance.shipComponentList, typeof(ShipComponentList), false);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();

        foreach (ComponentType compType in ShipData.ComponentTypes)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(compType.ToString());

            if (instance.shipComponentList != null)
            {
                string[] names = instance.shipComponentList.GetNames(compType);
                List<string> namesWithNil = new List<string>();
                namesWithNil.Add("None");
                namesWithNil.AddRange(names);
                instance.shipData[compType] = EditorGUILayout.Popup(
                    instance.shipData[compType] + 1,
                    namesWithNil.ToArray()
                ) - 1;
            }
            else
            {
                EditorGUILayout.LabelField(instance.shipData[compType].ToString());
            }

            EditorGUILayout.EndHorizontal();

        }

        EditorUtility.SetDirty(this.instance);

    }

}
