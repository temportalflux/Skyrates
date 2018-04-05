using UnityEditor;
using UnityEngine;

namespace Skyrates.Ship
{

    [CustomEditor(typeof(ShipGenerator))]
    public class ShipEditor : Editor
    {
        private ShipGenerator _instance;

        public void OnEnable()
        {
            this._instance = this.target as ShipGenerator;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate"))
            {
                this._instance.Destroy();
                this._instance.Generate();
            }

            if (GUILayout.Button("Destroy"))
            {
                this._instance.Destroy();
            }

            Undo.RecordObject(this._instance, string.Format("Edit {0}", this._instance.name));
            EditorUtility.SetDirty(this._instance);

        }


    }

}
