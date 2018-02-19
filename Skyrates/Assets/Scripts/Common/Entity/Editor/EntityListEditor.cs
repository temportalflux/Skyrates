using System.Collections;
using System.Collections.Generic;
using Skyrates.Client;
using Skyrates.Client.Entity;
using UnityEditor;
using UnityEngine;

namespace Skyrates.Common.Entity
{

    [CustomEditor(typeof(EntityList))]
    public class EntityListEditor : PrefabListEditor<Entity.Type, Entity>
    {

        private static bool[] _keysEnabled;

        protected override bool[] GetStaticKeyToggleArray()
        {
            return _keysEnabled;
        }

        protected override void SetStaticKeyToggleArray(bool[] keyBlocksToggled)
        {
            _keysEnabled = keyBlocksToggled;
        }

        protected override void PreDraw()
        {
            EntityList instance = (EntityList) this.Instance;

            instance.PrefabEntityPlayer = (EntityPlayerShip)EditorGUILayout.ObjectField(
                "Player Prefab", instance.PrefabEntityPlayer, typeof(EntityPlayerShip), false);

            EditorUtility.SetDirty(instance);
        }

    }

}
