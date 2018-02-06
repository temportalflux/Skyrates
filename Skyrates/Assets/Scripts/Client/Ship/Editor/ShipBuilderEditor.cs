﻿using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Ship;
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
        instance.ShipComponentList = (ShipComponentList)EditorGUILayout.ObjectField("Component List", instance.ShipComponentList, typeof(ShipComponentList), false);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();

        foreach (ComponentType compType in ShipData.ComponentTypes)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(compType.ToString());

            if (instance.ShipComponentList != null)
            {
                string[] names = instance.ShipComponentList.GetNames(compType);
                List<string> namesWithNil = new List<string>();
                namesWithNil.Add("None");
                namesWithNil.AddRange(names);
                instance.ShipData[compType] = EditorGUILayout.Popup(
                    instance.ShipData[compType] + 1,
                    namesWithNil.ToArray()
                ) - 1;
            }
            else
            {
                EditorGUILayout.LabelField(instance.ShipData[compType].ToString());
            }

            EditorGUILayout.EndHorizontal();

        }

        EditorUtility.SetDirty(this.instance);

    }

}
