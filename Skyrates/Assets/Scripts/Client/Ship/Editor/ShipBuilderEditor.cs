using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

        foreach (ShipBuilder.ComponentType compType in ShipBuilder.ComponentTypes)
        {
            int arrayIndex = (int)compType;

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(compType.ToString());

            if (instance.shipComponentList != null)
            {
                instance.components[arrayIndex] = EditorGUILayout.Popup(instance.components[arrayIndex], instance.shipComponentList.GetNames(compType));
            }
            else
            {
                EditorGUILayout.LabelField(instance.components[arrayIndex].ToString());
            }

            EditorGUILayout.EndHorizontal();

        }

        EditorUtility.SetDirty(this.instance);

    }

}
