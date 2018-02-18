using System;
using Skyrates.Client.Entity;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Common.Entity
{
    /// <summary>
    /// Any entity that in the world. Used predominantly for its ability to be syncced via networking.
    /// </summary>
    public class Entity : MonoBehaviour
    {

        /// <summary>
        /// Data pertaining to how to find the entity in <see cref="EntityList"/>.
        /// </summary>
        [Serializable]
        public class TypeData
        {

            /// <summary>
            /// <see cref="Entity.Type"/> as a number for network serialization.
            /// </summary>
            [BitSerialize(0)]
            [SerializeField]
            public int EntityTypeAsInt;

            /// <summary>
            /// The index of the entity prefab in <see cref="EntityList"/> under the category of <see cref="EntityType"/>.
            /// </summary>
            [BitSerialize(1)]
            [SerializeField]
            public int EntityTypeIndex;

            /// <summary>
            /// The type of entity. Encapsultes <see cref="EntityTypeAsInt"/>.
            /// </summary>
            public Entity.Type EntityType
            {
                get { return (Entity.Type) this.EntityTypeAsInt; }
                set { this.EntityTypeAsInt = (int) value; }
            }

            public TypeData(Entity.Type type, int index)
            {
                this.EntityType = type;
                this.EntityTypeIndex = index;
            }

            public TypeData()
            {
            }

        }

        #region Static

        public enum Type
        {
            // LISTABLES MUST COME FIRST
            Static,
            Dynamic,
            Projectile,

            // Then others...
            Player,
        }

        public static readonly Type[] AllTypes = { Type.Static, Type.Dynamic, Type.Projectile, Type.Player };
        public static readonly string[] AllTypesString = { Type.Static.ToString(), Type.Dynamic.ToString(), Type.Projectile.ToString(), Type.Player.ToString() };
        public static readonly object[] ListableTypes = { Type.Static, Type.Dynamic, Type.Projectile };
        public static readonly System.Type[] ListableClassTypes = { typeof(Entity), typeof(EntityDynamic), typeof(EntityProjectile) };

        #endregion

        // THE ORDER OF THESE FIELDS IS SENSITIVE FOR DESERIALIZATION IN ENTITYRECEIVER

        /// <summary>
        /// A unique identifier for this entity
        /// </summary>
        [BitSerialize(0)]
        [SerializeField]
        [HideInInspector]
        public Guid Guid;

        /// <summary>
        /// Which client "owns" this entity (sends updates about it).
        /// </summary>
        [BitSerialize(1)]
        [HideInInspector]
        public int OwnerNetworkID;

        [BitSerialize(2)]
        [SerializeField]
        public TypeData EntityType;

        /// <summary>
        /// If the entity is owned by the current instance (its <see cref="OwnerNetworkID"/> matches the <see cref="Session.NetworkID"/>).
        /// </summary>
        public bool IsLocallyControlled
        {
            get { return this.OwnerNetworkID == NetworkComponent.GetSession.NetworkID; }
        }

        protected virtual void Start()
        {
            if (this.EntityType.EntityType != Type.Player)
            {
                this.Guid = NewGuid();
                this.OwnerNetworkID = NetworkComponent.GetSession.NetworkID;
            }
            GameManager.Events.Dispatch(new EventEntity(GameEventID.EntityStart, this));
        }

        protected virtual void OnDestroy()
        {
            GameManager.Events.Dispatch(new EventEntity(GameEventID.EntityDestroy, this));
        }

        protected virtual void Update()
        {
        }

        /// <summary>
        /// Creates a unique and fresh ID. <see cref="Guid.NewGuid()"/>.
        /// </summary>
        /// <returns></returns>
        public static Guid NewGuid()
        {
            return Guid.NewGuid();
        }

        /// <summary>
        /// Initializes the entity with a specific <see cref="Guid"/> and <see cref="TypeData"/>.
        /// VERY important for syncing entities over the network.
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="typeData"></param>
        public void Init(Guid guid, TypeData typeData)
        {
            this.Guid = guid;
            this.EntityType = typeData;
        }

        /// <summary>
        /// Initializes the entity with a new <see cref="Guid"/> and <see cref="TypeData"/>.
        /// VERY important for syncing entities over the network.
        /// </summary>
        /// <param name="typeData"></param>
        public void Init(TypeData typeData)
        {
            this.Init(NewGuid(), typeData);
        }

        #region Network

        /// <summary>
        /// If this object should receive data sent over the network.
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldDeserialize()
        {
            // TODO: Check how this affects entity deserializatio in EntityReceiver. Does it skip data and not update the next index?
            return true;
        }

        /// <summary>
        /// Called by <see cref="EntityReceiver.Deserialize"/> when the entity's data was successfully deserialized into it.
        /// </summary>
        public virtual void OnDeserializeSuccess()
        {

        }

        /// <summary>
        /// Called by <see cref="EntityReceiver.Deserialize"/> when the entity was not found in the server state and thus should be removed.
        /// </summary>
        public virtual void OnDeserializeFail()
        {
            Destroy(this.gameObject);
        }

        #endregion


    }

}
