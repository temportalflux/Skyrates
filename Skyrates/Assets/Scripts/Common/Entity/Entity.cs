﻿using System;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
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

        public Entity.Type EntityType
        {
            get { return (Entity.Type) this.EntityTypeAsInt; }
            set { this.EntityTypeAsInt = (int)value; }
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

        //[BitSerialize(2)]
        //[SerializeField]
        //[HideInInspector]
        //public TypeData TypeData;

        // TODO: Made an editor so these two properties are unessessary
        [BitSerialize(2)]
        public int EntityTypeIndex;

        public Type EntityType
        {
            get { return (Entity.Type) EntityTypeIndex; }
            set { this.EntityTypeIndex = (int) value; }
        }
        [BitSerialize(3)]
        public int EntityTypeArrayIndex;

        protected virtual void Start()
        {
            if (this.EntityType != Type.Player)
            {
                this.Guid = NewGuid();
                this.OwnerNetworkID = NetworkComponent.GetSession.NetworkID;
            }
            // TODO: Firing this before GameManager.SpawnEntity.EntityInstantiate could result in duplicates in eneity tracker
            //GameManager.Events.Dispatch(new EventEntity(GameEventID.EntityStart, this));
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
            //this.TypeData = typeData;
            this.EntityType = typeData.EntityType;
            this.EntityTypeArrayIndex = typeData.EntityTypeIndex;
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
