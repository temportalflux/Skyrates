using System;
using UnityEditor;
using UnityEngine;

using ComponentType = Skyrates.Ship.ShipData.ComponentType;

namespace Skyrates.Ship
{

    [CustomEditor(typeof(ShipHullGenerated))]
    public class ShipHullGeneratedEditor : ShipHullEditor
    {

        private ShipHullGenerated _instanceGenerated;

        public override void OnEnable()
        {
            base.OnEnable();
            this._instanceGenerated = this._instance as ShipHullGenerated;
        }

        protected override void DrawComponentList(ref bool[] toggleComponentBlock)
        {
            ToggleComponents = EditorGUILayout.Foldout(ToggleComponents, "Components");

            if (!ToggleComponents) return;

            EditorGUI.indentLevel++;

            foreach (ComponentType compType in ShipData.ComponentTypes)
            {
                EditorGUILayout.Separator();

                int iComp = (int)compType;

                Transform[] roots = this._instanceGenerated.Mounts[iComp].Value ?? new Transform[0];

                toggleComponentBlock[iComp] = this.DrawArray(
                    compType.ToString(), ref roots,
                    true, toggleComponentBlock[iComp],
                    DrawBlock: ((t, i) => (Transform)EditorGUILayout.ObjectField(
                        t == null ? "Pos/Rot" : t.name,
                        t, typeof(Transform), allowSceneObjects: true
                    ))
                );

                this._instanceGenerated.Mounts[iComp].Value = roots;

            }

            EditorGUI.indentLevel--;
        }

    }

}
