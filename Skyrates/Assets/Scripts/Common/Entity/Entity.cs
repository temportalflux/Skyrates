using System;
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
        [Serializable]
        public class TypeData
        {

            [BitSerialize(0)]
            [SerializeField]
            public int EntityTypeAsInt;

            [BitSerialize(1)]
            [SerializeField]
            public int EntityTypeIndex;

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

        void OnDestroy()
        {
            GameManager.Events.Dispatch(new EventEntity(GameEventID.EntityDestroy, this));
        }
        
        public static Guid NewGuid()
        {
            return Guid.NewGuid();
        }

        public void Init(Guid guid, TypeData typeData)
        {
            this.Guid = guid;
            this.EntityType = typeData;
            //this.EntityType = typeData.EntityType;
            //this.EntityTypeArrayIndex = typeData.EntityTypeIndex;
        }

        public void Init(TypeData typeData)
        {
            this.Init(NewGuid(), typeData);
        }

        #region Network

        public virtual bool ShouldDeserialize()
        {
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
