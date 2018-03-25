using Skyrates.Ship;
using UnityEditor;
using UnityEngine;

namespace Skyrates.Loot
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

            if (this._instance.Table == null)
                this._instance.Table = new LootTable.Row[0];

            this.DrawArray(
                "Table", ref this._instance.Table,
                togglable: false, isToggled: true,
                DrawBlock: ((row, i) => {
                    if (row == null) row = new LootTable.Row();

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField((row.Percentage * 100).ToString("F1") + "%",
                            GUILayout.Width(40)
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

                        row.Item = (ShipData.BrokenComponentType)EditorGUILayout.EnumPopup(
                            row.Item
                        );
                        row.Prefab = (Loot)EditorGUILayout.ObjectField(
                            row.Prefab, typeof(Loot), allowSceneObjects: true
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

            Undo.RecordObject(this._instance, string.Format("Edit {0}", this._instance.name));
        }

    }

}
