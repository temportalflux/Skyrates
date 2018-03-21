using System;
using UnityEditor;
using UnityEngine;

using ComponentType = Skyrates.Ship.ShipData.ComponentType;

namespace Skyrates.Ship
{

    [CustomEditor(typeof(ShipHull))]
    public class ShipHullEditor : Editor
    {
        protected ShipHull _instance;

        private static bool[] ToggleComp = new bool[ShipData.ComponentTypes.Length];
        private static bool ToggleLootMounts = false;
        private static bool ToggleStats = true;
        private static bool ToggleHealthFeedback = true;

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
            if (ToggleComp.Length != ShipData.ComponentTypes.Length)
                Array.Resize(ref ToggleComp, ShipData.ComponentTypes.Length);
        }

        protected virtual void DrawComponentList(ref bool[] toggleComponentBlock)
        {
            EditorGUILayout.LabelField("Components");
            foreach (ComponentType compType in ShipData.ComponentTypes)
            {
                EditorGUILayout.Separator();

                int iComp = (int)compType;

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

            // Stats
            {
                ToggleStats = EditorGUILayout.Foldout(ToggleStats, "Stats");

                if (ToggleStats)
                {
                    this._instance.MaxHealth = EditorGUILayout.IntField("Max HP", this._instance.MaxHealth);
                    this._instance.HealthRegenAmount = EditorGUILayout.FloatField("HP Regen Amount", this._instance.HealthRegenAmount);
                    this._instance.HealthRegenDelay = EditorGUILayout.FloatField("HP Regen Delay", this._instance.HealthRegenDelay);
                }
            }
            EditorGUILayout.Separator();
            // Health Feedback
            {
                ToggleHealthFeedback = EditorGUILayout.Foldout(ToggleHealthFeedback, "Health Feedback");

                if (ToggleHealthFeedback)
                {
                    EditorGUILayout.LabelField("Smoke Damage");
                    this.MinMax(ref this._instance.HealthFeedbackData.SmokeDamage);
                    EditorGUILayout.LabelField("Smoke Emission");
                    this.MinMax(ref this._instance.HealthFeedbackData.SmokeEmissionAmount);
                    EditorGUILayout.LabelField("Fire Damage");
                    this.MinMax(ref this._instance.HealthFeedbackData.FireDamage);
                    EditorGUILayout.LabelField("Fire Emission");
                    this.MinMax(ref this._instance.HealthFeedbackData.FireEmissionAmount);
                }
            }
            EditorGUILayout.Separator();

            // Components
            this.DrawComponentList(ref ToggleComp);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            this.DrawLootRoots();

            EditorUtility.SetDirty(this._instance);
        }

        private void MinMax(ref Vector2Int bounds)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Min");
            EditorGUILayout.LabelField("Max");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            bounds.x = EditorGUILayout.IntField(bounds.x);
            bounds.y = EditorGUILayout.IntField(bounds.y);
            EditorGUILayout.EndHorizontal();
        }

    }

}
