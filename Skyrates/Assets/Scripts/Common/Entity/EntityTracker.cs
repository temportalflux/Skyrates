using Skyrates.Common.Network;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Common.Entity
{
    public class EntitySet
    {
        
        public Entity.EntityType _key;
        
        public readonly Dictionary<Guid, Entity> _entities;

        public EntitySet(Entity.EntityType type)
        {
            this._key = type;
            this._entities = new Dictionary<Guid, Entity>();
        }

        public void Add(Entity e)
        {
            this._entities.Add(e.Guid, e);
        }

        public Entity Remove(Guid id)
        {
            if (!this._entities.ContainsKey(id)) return null;
            Entity e = this._entities[id];
            this._entities.Remove(id);
            return e;
        }

        public bool TryGetValue<T>(Guid id, out T entity) where T : Entity
        {
            entity = null;

            Entity e;
            bool found = this._entities.TryGetValue(id, out e);
            if (found)
            {
                Debug.Assert(e is T);
                entity = e as T;
                return true;
            }

            return false;
        }

        public bool ContainsKey(Guid id)
        {
            return this._entities.ContainsKey(id);
        }

    }


    public class EntityTracker : ISerializing
    {
        
        public readonly Dictionary<Entity.EntityType, EntitySet> _entities = new Dictionary<Entity.EntityType, EntitySet>();

        void Awake()
        {
            foreach (Entity.EntityType type in Entity.TYPES)
            {
                this._entities.Add(type, new EntitySet(type));
            }
        }

        public void Add(Entity.EntityType type, Entity e)
        {
            Debug.Assert(this._entities.ContainsKey(type));
            this._entities[type].Add(e);
        }

        public Entity Remove(Entity.EntityType type, Guid id)
        {
            return this._entities[type].Remove(id);
        }

        public bool ContainsKey(Entity.EntityType type, Guid entityGuid)
        {
            return this._entities[type].ContainsKey(entityGuid);
        }

        public bool TryGetValue(Entity.EntityType type, Guid id, out Entity e)
        {
            e = null;
            EntitySet set;
            return this._entities.TryGetValue(type, out set) && set.TryGetValue(id, out e);
        }

        /* TODO: Add event for spawning/destroying entities
        public static void Spawn(GameStateData.Client client)
        {
            Debug.Log("Spawning player for client " + client.clientID);
            SpawnPlayer(client.playerEntityGuid).SetDummy(!client.IsLocalClient);
        }

        public static Player SpawnPlayer(Guid entityID)
        {
            // TODO: This is put on a DoNotDestroyOnLoad object
            Player player = GameObject.Instantiate(Instance.PlayerPrefab.gameObject, Instance.transform)
                .GetComponent<Player>();

            player.transform.SetPositionAndRotation(Instance.spawn.position, Instance.spawn.rotation);

            // Generate identifier
            player.Init(entityID);
            // Generate ship object
            player.GenerateShip();

            Instance._playerEntities.Add(player.GetGuid(), player);

            return player;
        }

        public static void Destroy(GameStateData.Client client)
        {
            Debug.Log("Destroy player for client " + client.clientID);

            if (client.playerEntityGuidValid)
            {

                Debug.Assert(Instance._playerEntities.ContainsKey(client.playerEntityGuid),
                    "Cannot destroy an entity that is not tracked");

                Player player = Instance._playerEntities[client.playerEntityGuid];
                Instance._playerEntities.Remove(client.playerEntityGuid);

                Destroy(player.gameObject);

            }

        }
        //*/

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
