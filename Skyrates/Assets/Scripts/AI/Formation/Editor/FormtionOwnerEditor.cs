using System.Collections;
using System.Collections.Generic;
using Skyrates.AI.Formation;
using UnityEditor;
using UnityEngine;

namespace Skyrates.Common.AI.Formation
{

    [CustomEditor(typeof(FormationOwner))]
    public class FormtionOwnerEditor : Editor
    {

        private FormationOwner _instance;

        private static bool ToggleSlotsBlock = false;
        private static bool[] ToggleSlotsEntries = new bool[0];

        void OnEnable()
        {
            this._instance = this.target as FormationOwner;
        }

        public override void OnInspectorGUI()
        {
            this.DrawScriptField(this._instance);

            this._instance.NearbyRange = EditorGUILayout.FloatField("Nearby Range", this._instance.NearbyRange);
            this._instance.ThreatRange = EditorGUILayout.FloatField("Threat Range", this._instance.ThreatRange);

            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("GizmoColorNearby"), true);
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("GizmoColorNearbyTargets"), true);
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("GizmoColorThreat"), true);
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("GizmoColorSlots"), true);
            
            this.DrawArrayArea("Slots", ref this._instance.Slots, o => (o as GameObject).GetComponent<FormationSlot>());

            ToggleSlotsBlock = this.DrawArray("Slots", ref this._instance.Slots,
                true, ToggleSlotsBlock,
                false, ref ToggleSlotsEntries,
                (slot, i) => slot.name,
                (slot, i) => (FormationSlot)EditorGUILayout.ObjectField(
                    slot, typeof(FormationSlot), true)
            );

            Undo.RecordObject(this._instance, string.Format("Edit {0}", this._instance.name));
            EditorUtility.SetDirty(this._instance);
        }

    }

}
