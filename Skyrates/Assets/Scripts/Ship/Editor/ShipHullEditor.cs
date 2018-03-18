using System;
using UnityEditor;
using UnityEngine;

using ComponentType = ShipData.ComponentType;

namespace Skyrates.Client.Ship
{

    [CustomEditor(typeof(ShipHull))]
    public class ShipHullEditor : Editor
    {
        protected ShipHull _instance;

        private static bool[] ToggleComp = new bool[ShipData.NonHullComponents.Length];
        private static bool ToggleLootMounts = false;

        public virtual void OnEnable()
        {
            this._instance = this.target as ShipHull;
        }

        protected void DrawScript()
        {
            this.DrawScriptField(this._instance);
        }

        protected virtual void CheckArrayLengths()
        {
            //if (this._instance.Components == null) this._instance.Components = new ShipComponent[ShipData.NonHullComponents.Length][];
            //if (this._instance.Components.Length != ShipData.NonHullComponents.Length)
            //{
            //    Array.Resize(ref this._instance.Components, ShipData.NonHullComponents.Length);
            //}
            if (ToggleComp.Length != ShipData.NonHullComponents.Length)
                Array.Resize(ref ToggleComp, ShipData.NonHullComponents.Length);
        }

        protected virtual void DrawComponentList(ref bool[] toggleComponentBlock)
        {
            foreach (ComponentType compType in ShipData.NonHullComponents)
            {
                EditorGUILayout.Separator();

                int iComp = ShipData.HulllessComponentIndex[(int)compType];

                ShipComponent[] componentsOfType = this._instance.Components[iComp].Value ?? new ShipComponent[0];

                toggleComponentBlock[iComp] = this.DrawArray(
                    compType.ToString(), ref componentsOfType,
                    true, toggleComponentBlock[iComp],
                    DrawBlock: ((t, i) => (ShipComponent)EditorGUILayout.ObjectField(
                        t == null ? "Component" : t.name,
                        t, ShipData.ComponentClassTypes[iComp], allowSceneObjects: true
                    ))
                );

                this._instance.Components[iComp].Value = componentsOfType;

            }
        }

        protected void DrawLootRoots()
        {
            this.DrawArrayArea("Drag loot set", ref this._instance.LootMounts, (o => (o as GameObject).transform));

            ToggleLootMounts = this.DrawArray("Loot", ref this._instance.LootMounts, togglable: true, isToggled: ToggleLootMounts,
                DrawBlock: ((mount, i) =>
                {
                    mount = (Transform)EditorGUILayout.ObjectField("Pos/Rot/Scale", mount, typeof(Transform), allowSceneObjects: true);
                    return mount;
                })
            );
        }

        public override void OnInspectorGUI()
        {
            this.DrawScript();
            this.CheckArrayLengths();
            this.DrawComponentList(ref ToggleComp);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            this.DrawLootRoots();

            EditorUtility.SetDirty(this._instance);
        }

    }

}
