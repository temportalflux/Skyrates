using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Ship;
using UnityEditor;
using UnityEngine;

using ComponentType = ShipData.ComponentType;

namespace Skyrates.Client.Ship
{

    [CustomEditor(typeof(ShipHull))]
    public class HullTargetEditor : Editor
    {

        private ShipHull _instance;

        private static readonly bool[] ToggleComp = new bool[ShipData.ComponentTypes.Length];

        public void OnEnable()
        {
            this._instance = this.target as ShipHull;
        }

        public override void OnInspectorGUI()
        {
            this.DrawScriptField(this._instance);

            foreach (ComponentType compType in ShipData.ComponentTypes)
            {
                if (compType == ComponentType.Hull) continue;

                EditorGUILayout.Separator();

                int iComp = (int) compType - 1;

                Transform[] roots = this._instance.Mounts[iComp].Roots;

                this.Array(compType.ToString(), ref ToggleComp[iComp], ref roots,
                    doBlock: true, GetFieldName: (Transform t) => t == null ? "Pos/Rot" : t.name);

                this._instance.Mounts[iComp].Roots = roots;

            }

            EditorUtility.SetDirty(this._instance);
        }

    }

}
