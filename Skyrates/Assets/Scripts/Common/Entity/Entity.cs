using System;
using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Common.Entity
{
    
    [Serializable]
    public class TypeData
    {

        [BitSerialize(0)]
        public Entity.Type EntityType;

        [BitSerialize(1)]
        public int EntityTypeIndex;

        public TypeData(Entity.Type type, int index)
        {
            this.EntityType = type;
            this.EntityTypeIndex = index;
        }

    }

    public class Entity : MonoBehaviour
    {

        #region Static

        public enum Type
        {
            // LISTABLES MUST COME FIRST
            Static,
            Dynamic,

            // Then others...
            Player,
        }

        public static readonly Type[] AllTypes = { Type.Static, Type.Dynamic, Type.Player };
        public static readonly object[] ListableTypes = { Type.Static, Type.Dynamic };
        public static readonly System.Type[] ListableClassTypes = { typeof(Entity), typeof(EntityDynamic) };

        #endregion

        /// <summary>
        /// A unique identifier for this entity
        /// </summary>
        [BitSerialize(0)]
        [SerializeField]
        public Guid Guid;

        [BitSerialize(1)]
        [SerializeField]
        public TypeData TypeData;

        #region Guid

        public static Guid NewGuid()
        {
            return Guid.NewGuid();
        }

        public void Init(Guid guid, TypeData typeData)
        {
            this.Guid = guid;
            this.TypeData = typeData;
        }

        public void Init(TypeData typeData)
        {
            this.Init(NewGuid(), typeData);
        }

        #endregion

        #region Network

        /// <summary>
        /// Called by <see cref="EntityReceiver.Deserialize"/> when the entity's data was successfully deserialized into it.
        /// </summary>
        public void OnDeserializeSuccess()
        {

        }

        /// <summary>
        /// Called by <see cref="EntityReceiver.Deserialize"/> when the entity was not found in the server state and thus should be removed.
        /// </summary>
        public void OnDeserializeFail()
        {
            Destroy(this.gameObject);
        }

        #endregion
        
    }

}
