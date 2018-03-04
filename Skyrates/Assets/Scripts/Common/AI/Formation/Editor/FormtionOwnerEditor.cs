using System.Collections;
using System.Collections.Generic;
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

            this.DrawArrayArea("Slots", ref this._instance.Slots, o => o.transform);

            ToggleSlotsBlock = this.DrawArray("Slots", ref this._instance.Slots,
                true, ToggleSlotsBlock,
                false, ref ToggleSlotsEntries,
                ((transform, i) => transform.name),
                ((transform, i) =>
                {
                    transform = (Transform) EditorGUILayout.ObjectField(
                        transform, typeof(Transform),
                        allowSceneObjects: true);
                    return transform;
                }));

        }

    }

}
