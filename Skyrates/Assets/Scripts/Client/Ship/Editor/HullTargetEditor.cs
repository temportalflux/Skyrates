using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using ComponentType = ShipData.ComponentType;

[CustomEditor(typeof(ShipHull))]
public class HullTargetEditor : Editor
{

    private ShipHull instance;

    static bool[] toggleComp = new bool[ShipData.ComponentTypes.Length];

    public void OnEnable()
    {
        this.instance = this.target as ShipHull;
    }

    public override void OnInspectorGUI()
    {
        this.DrawScriptField(this.instance);

        ShipHull.HullTargets targets = this.instance.targets;
        {
            foreach (ComponentType compType in ShipData.ComponentTypes)
            {
                if (compType == ComponentType.Hull) continue;

                EditorGUILayout.Separator();

                int iComp = (int) compType - 1;

                Transform[] roots = targets.list[iComp].roots;

                this.Array(compType.ToString(), ref toggleComp[iComp], ref roots,
                    doBlock:true, GetFieldName:(Transform t) => t == null ? "Pos/Rot" : t.name);
                
                targets.list[iComp].roots = roots;

            }

        }
        this.instance.targets = targets;

        EditorUtility.SetDirty(this.instance);
    }

}
