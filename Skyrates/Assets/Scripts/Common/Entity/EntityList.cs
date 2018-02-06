using Skyrates.Client.Util;
using UnityEngine;

namespace Skyrates.Common.Entity
{

    [CreateAssetMenu(menuName = "Data/List: Entity")]
    public class EntityList : PrefabList
    {

        [SerializeField]
        public EntityDynamic PrefabPlayer;

        void OnEnable()
        {
            this.Setup(Entity.ListableTypes, Entity.ListableClassTypes);
        }

        public override int GetIndexFrom(object key)
        {
            return (int) key;
        }

        public override object GetKeyFrom(int index)
        {
            return (Entity.Type) index;
        }

    }

}
