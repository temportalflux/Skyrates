using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Ship;
using UnityEditor;
using UnityEngine;

namespace Skyrates.Client.Loot
{

    [CustomEditor(typeof(LootTable))]
    public class LootTableEditor : Editor
    {

        private LootTable _instance;

        void OnEnable()
        {
            this._instance = this.target as LootTable;
        }

        public override void OnInspectorGUI()
        {
            this.DrawScriptField(this);

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Iterations", GUILayout.Width(60));

                GUILayout.FlexibleSpace();

                this._instance.AmountMin = EditorGUILayout.IntField(
                    this._instance.AmountMin, GUILayout.Width(50));
                EditorGUILayout.LabelField("-", GUILayout.Width(10));
                this._instance.AmountMax = EditorGUILayout.IntField(
                    this._instance.AmountMax, GUILayout.Width(50));
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
            
            bool weightsChanged = false;
            int sumWeight = 0;

            this.DrawArray(
                "Table", ref this._instance.Table,
                togglable:false, isToggled:true,
                DrawBlock: (row =>
                {
                    if (row == null) row = new LootTable.Row();

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField((row.Percentage * 100) + "%",
                            GUILayout.Width(30)
                        );

                        GUILayout.FlexibleSpace();

                        int preWeight = row.Weight;
                        row.Weight = EditorGUILayout.IntField(preWeight,
                            GUILayout.Width(50)
                        );
                        sumWeight += row.Weight;
                        if (row.Weight != preWeight)
                        {
                            weightsChanged = true;
                        }

                        // TODO: Change this
                        row.Item = (ShipComponent)EditorGUILayout.ObjectField(
                            row.Item, typeof(ShipComponent), allowSceneObjects: true
                        );
                    }
                    EditorGUILayout.EndHorizontal();

                    return row;
                })
            );

            if (weightsChanged)
            {
                this._instance.CalculatePercentages(sumWeight);
            }

            EditorUtility.SetDirty(this._instance);
        }

    }

}
