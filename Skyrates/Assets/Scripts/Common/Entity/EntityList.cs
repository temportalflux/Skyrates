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

        // useful only for editor
        public static EntityList Instance;

        /// <summary>
        /// The prefab of players who are created via the network.
        /// </summary>
        [SerializeField]
        public Client.EntityPlayerShip PrefabEntityPlayer;

        void OnEnable()
        {
            Instance = this;
            this.Setup(Entity.ListableTypes, Entity.ListableClassTypes);

            foreach (Category category in Categories)
            {
                for (int i = 0; i < category.Prefabs.Length; i++)
                {
                    if (category.Prefabs[i] == null)
                    {
                        Debug.LogWarning(string.Format("Entity List prefab of type {0} at index {1} is null. This will break multiplayer.", this.GetKeyFrom(i), i));
                    }
                }
            }
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
