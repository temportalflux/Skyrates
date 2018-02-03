using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

using ComponentType = ShipData.ComponentType;

[CustomEditor(typeof(ShipComponentList))]
public class ShipComponentListEditor : Editor
{

    private ShipComponentList instance;

    public void OnEnable()
    {
        this.instance = this.target as ShipComponentList;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Ship Components");

        // Get the list of which categories are already folded out
        bool[] enabled_editor = this.instance.editor_showComponentArray;
        if (enabled_editor == null) enabled_editor = new bool[ShipData.ComponentTypes.Length];

        bool changed = false;

        foreach (ComponentType compType in ShipData.ComponentTypes)
        {
            // Get the index of the Component type
            int arrayIndex = (int)compType;


            // Fetch the list of componets of this type
            ShipComponentList.ComponentList components = this.instance.components[arrayIndex];
            if (components.components == null) components.components = new ShipComponent[0];
            // Get the script class type of this component type
            Type shipComponentType = ShipData.ComponentClassTypes[(int)compType];

            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            {
                enabled_editor[arrayIndex] = EditorGUILayout.Foldout(enabled_editor[arrayIndex], compType.ToString());

                int prevSize = components.components.Length;
                int arrSize = EditorGUILayout.IntField("Size", prevSize);
                if (prevSize != arrSize)
                {
                    Array.Resize(ref components.components, arrSize);
                    changed = true;
                }
            }
            EditorGUILayout.EndHorizontal();

            if (enabled_editor[arrayIndex])
            {
                for (int iComponent = 0; iComponent < components.components.Length; iComponent++)
                {
                    ShipComponent prev = components.components[iComponent];
                    string name = prev == null ? "None" : prev.name;
                    components.components[iComponent] = (ShipComponent) EditorGUILayout.ObjectField(iComponent + ") " + name,
                        prev, shipComponentType, false);
                    if (components.components[iComponent] != prev)
                    {
                        changed = true;
                    }
                }
            }

            this.instance.components[arrayIndex] = components;
        }

        this.instance.editor_showComponentArray = enabled_editor;

        if (changed)
        {
            this.instance.GenerateNames();
        }

        EditorUtility.SetDirty(this.instance);
        
    }


}
