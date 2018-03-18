using System;
using UnityEditor;
using UnityEngine;

using ComponentType = ShipData.ComponentType;

namespace Skyrates.Client.Ship
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

        protected override void CheckArrayLengths()
        {
            base.CheckArrayLengths();
            //if (this._instanceGenerated.Mounts == null) this._instanceGenerated.Mounts = new Transform[ShipData.NonHullComponents.Length][];
            //if (this._instanceGenerated.Mounts.Length != ShipData.NonHullComponents.Length)
            //{
            //    Array.Resize(ref this._instanceGenerated.Mounts, ShipData.NonHullComponents.Length);
            //}
        }

        protected override void DrawComponentList(ref bool[] toggleComponentBlock)
        {
            foreach (ComponentType compType in ShipData.NonHullComponents)
            {
                EditorGUILayout.Separator();

                int iComp = ShipData.HulllessComponentIndex[(int)compType];

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
        }

    }

}
