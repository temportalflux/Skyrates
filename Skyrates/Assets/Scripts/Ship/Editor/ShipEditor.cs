using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Skyrates.Ship
{

    [CustomEditor(typeof(Ship))]
    public class ShipEditor : Editor
    {
        private Ship _instance;

        public void OnEnable()
        {
            this._instance = this.target as Ship;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Build"))
            {
                this._instance.Destroy();
                // TODO: Dont generate with no entity
                this._instance.Generate(null);
            }

            EditorUtility.SetDirty(this._instance);

        }


    }

}
