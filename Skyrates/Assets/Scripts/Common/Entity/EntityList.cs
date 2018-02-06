using Skyrates.Client.Util;
using UnityEngine;

namespace Skyrates.Common.Entity
{

    /// <summary>
    /// The list of all entity prefabs (all spawnable entities).
    /// ASSUME: All entites which are networked have a prefab in this list.
    /// </summary>
    [CreateAssetMenu(menuName = "Data/List: Entity")]
    public class EntityList : PrefabList
    {

        /// <summary>
        /// The prefab of players who are created via the network.
        /// </summary>
        [SerializeField]
        public EntityDynamic PrefabPlayer;

        void OnEnable()
        {
            this.Setup(Entity.ListableTypes, Entity.ListableClassTypes);
        }

        /// <inheritdoc />
        public override int GetIndexFrom(object key)
        {
            return (int) key;
        }

        /// <inheritdoc />
        public override object GetKeyFrom(int index)
        {
            return (Entity.Type) index;
        }

    }

}
