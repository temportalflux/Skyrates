using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShipHull))]
public class HullTargetEditor : Editor
{

    private ShipHull instance;

    static bool[] toggleComp = new bool[ShipBuilder.ComponentTypes.Length];

    public void OnEnable()
    {
        this.instance = this.target as ShipHull;
    }

    public override void OnInspectorGUI()
    {
        this.ScriptField(this.instance);

        ShipHull.HullTargets targets = this.instance.targets;
        {
            foreach (ShipBuilder.ComponentType compType in ShipBuilder.ComponentTypes)
            {
                EditorGUILayout.Separator();
                int iComp = (int) compType;

                Transform[] roots = targets.list[iComp].roots;

                int targetSize = roots.Length;

                EditorGUILayout.BeginHorizontal();
                {
                    toggleComp[iComp] = EditorGUILayout.Foldout(
                        toggleComp[iComp], compType.ToString());

                    GUILayout.FlexibleSpace();

                    EditorGUILayout.BeginHorizontal();
                    {
                        targetSize = EditorGUILayout.IntField(targetSize);

                        if (GUILayout.Button("+", GUILayout.Width(20)))
                        {
                            targetSize++;
                        }

                        if (GUILayout.Button("-", GUILayout.Width(20)))
                        {
                            targetSize--;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    Array.Resize(ref roots, targetSize);

                }
                EditorGUILayout.EndHorizontal();

                if (toggleComp[iComp])
                {
                    for (int iRoot = 0; iRoot < targetSize; iRoot++)
                    {
                        roots[iRoot] = (Transform)EditorGUILayout.ObjectField(
                            "Pos/Rot", roots[iRoot], typeof(Transform), true);
                    }
                }

                targets.list[iComp].roots = roots;

            }

        }
        this.instance.targets = targets;

        EditorUtility.SetDirty(this.instance);
    }

}
