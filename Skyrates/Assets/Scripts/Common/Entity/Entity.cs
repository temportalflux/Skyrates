using System;
using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Common.Entity
{
    
    [Serializable]
    public class TypeData
    {

        [BitSerialize(0)]
        public int EntityTypeAsInt;

        [BitSerialize(1)]
        public int EntityTypeIndex;

        [BitSerialize(2)]
        public int OwnerNetworkID;

        public Entity.Type EntityType
        {
            get { return (Entity.Type) this.EntityTypeAsInt; }
            set { this.EntityTypeAsInt = (int)value; }
        }

        public TypeData(Entity.Type type, int index) : this(type, index, (int)NetworkComponent.GetSession.NetworkID)
        {
        }

        public TypeData(Entity.Type type, int index, int clientId)
        {
            this.EntityType = type;
            this.EntityTypeIndex = index;
            this.OwnerNetworkID = clientId;
        }

        public TypeData()
        {
        }

    }

    /// <summary>
    /// Any entity that in the world. Used predominantly for its ability to be syncced via networking.
    /// </summary>
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
        [HideInInspector]
        public Guid Guid;

        [BitSerialize(1)]
        [SerializeField]
        [HideInInspector]
        public TypeData TypeData;

        // TODO: Fire an event when entities are created (via editor OR via script)
        // TODO: Fire an event when entities are destroyed
        // TODO: Respond to these events by editting EntityTracker IF HOST OR NOT NETWORKED
        
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

        public virtual bool ShouldDeserialize()
        {
            return true;
        }


    }

}
