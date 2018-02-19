﻿using Skyrates.Common.Network;
using System;
using System.Collections.Generic;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using UnityEngine;

namespace Skyrates.Common.Entity
{

    /// <summary>
    /// The set of entities tracked via <see cref="EntityTracker"/>,
    /// of a single <see cref="Entity.Type"/>.
    /// </summary>
    public class EntitySet
    {
        
        /// <summary>
        /// The type that this set contains.
        /// </summary>
        public Entity.Type Key;
        
        /// <summary>
        /// The list of entities being tracked, keyed by their <see cref="Guid"/>.
        /// </summary>
        public readonly Dictionary<Guid, Entity> Entities;

        public EntitySet(Entity.Type type)
        {
            this.Key = type;
            this.Entities = new Dictionary<Guid, Entity>();
        }

        /// <summary>
        /// Add an entity to the tracker.
        /// </summary>
        /// <param name="e"></param>
        public bool Add(Entity e)
        {
            // Cannot track and entity already being tracked.
            if (this.ContainsKey(e.Guid)) return false;
            this.Entities.Add(e.Guid, e);
            return true;
        }

        /// <summary>
        /// Remove an entity from the tracker.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entity Remove(Guid id)
        {
            if (!this.Entities.ContainsKey(id)) return null;
            Entity e = this.Entities[id];
            this.Entities.Remove(id);
            return e;
        }

        /// <summary>
        /// Attempts to find an entity via its <see cref="Guid"/> in this set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool TryGetValue<T>(Guid id, out T entity) where T : Entity
        {
            entity = null;

            Entity e;
            bool found = this.Entities.TryGetValue(id, out e);
            if (found)
            {
                Debug.Assert(e is T);
                entity = e as T;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if there in an entity with a specific <see cref="Guid"/> under this set.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ContainsKey(Guid id)
        {
            return this.Entities.ContainsKey(id);
        }

    }

    /// <summary>
    /// A tracker which watches all entities which are synced over the network.
    /// Implemented by <see cref="EntityDispatcher"/> and <see cref="EntityReceiver"/> to handle
    /// serializing and deserializing entities.
    /// </summary>
    public class EntityTracker : ISerializing
    {
        
        /// <summary>
        /// The dictionary of all entities, keyed by a type of entity.
        /// </summary>
        public readonly Dictionary<Entity.Type, EntitySet> Entities = new Dictionary<Entity.Type, EntitySet>();

        public EntityTracker()
        {
            foreach (Entity.Type type in Entity.AllTypes)
            {
                this.Entities.Add(type, new EntitySet(type));
            }
        }

        #region Manager

        /// <summary>
        /// Add an entity to the map.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="e"></param>
        public bool Add(Entity e)
        {
            Debug.Assert(this.Entities.ContainsKey(e.EntityType.EntityType));
            return this.Entities[e.EntityType.EntityType].Add(e);
        }

        /// <summary>
        /// Tries to remove an entity from the map.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entity Remove(Entity.Type type, Guid id)
        {
            return this.Entities[type].Remove(id);
        }

        /// <summary>
        /// Returns if there is an entity with a <see cref="Guid"/> under a specific <see cref="Entity.Type"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="entityGuid"></param>
        /// <returns></returns>
        public bool ContainsKey(Entity.Type type, Guid entityGuid)
        {
            return this.Entities[type].ContainsKey(entityGuid);
        }

        /// <summary>
        /// Tries to get an entity under a <see cref="Entity.Type"/> with a specific <see cref="Guid"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool TryGetValue(Entity.Type type, Guid id, out Entity e)
        {
            e = null;
            EntitySet set;
            return this.Entities.TryGetValue(type, out set) && set.TryGetValue(id, out e);
        }

        #endregion

        #region Events

        public virtual void SubscribeEvents()
        {
            GameManager.Events.EntityInstantiate += this.OnEntityInstantiate;
            GameManager.Events.EntityDestroy += this.OnEntityDestroy;
        }

        public virtual void UnsubscribeEvents()
        {
            GameManager.Events.EntityInstantiate -= this.OnEntityInstantiate;
            GameManager.Events.EntityDestroy -= this.OnEntityDestroy;
        }

        void OnEntityInstantiate(GameEvent evt)
        {
            this.OnEntityStart(((EventEntity) evt).Entity);
        }

        protected void OnEntityStart(Entity entity)
        {
            // double check in case this was already covered by entity instantiate
            if (!this.ContainsKey(entity.EntityType.EntityType, entity.Guid))
            {
                this.Add(entity);
            }
        }

        void OnEntityDestroy(GameEvent evt)
        {
            Entity ent = ((EventEntity) evt).Entity;
            this.Remove(ent.EntityType.EntityType, ent.Guid);
        }

        #endregion
        
        #region Serializing

        /// <summary>
        /// 
        /// </summary>
        public virtual void GenerateData() { }

        /// <inheritdoc />
        public virtual int GetSize()
        {
            return 0;
        }

        /// <inheritdoc />
        public virtual void Serialize(ref byte[] data, ref int lastIndex)
        {
        }

        /// <inheritdoc />
        public virtual void Deserialize(byte[] data, ref int lastIndex)
        {
        }

        #endregion
        
    }

}