using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Common.Entity;
using Skyrates.Common.Network;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Skyrates.Client.Game
{

    public class GameManager : Singleton<GameManager>
    {

        public static GameManager Instance;

        public static GameEvents Events
        {
            get { return Instance._events; }
        }

        public EntityList EntityList;
        private GameEvents _events;

        public Transform playerSpawn;

        void Awake()
        {
            this.loadSingleton(this, ref Instance);
            this._events = new GameEvents();
        }

        #region Spawn Local Player

        void OnEnable()
        {
            Events.SceneLoaded += this.OnSceneLoaded;
        }
        void OnDisable()
        {
            Events.SceneLoaded -= this.OnSceneLoaded;
        }

        void OnSceneLoaded(GameEvent evt)
        {
            // TODO: Move this to owner classes (DummyClient & ClientServer)
            if (NetworkComponent.GetSession.IsOwner && ((EventSceneLoaded) evt).Scene == SceneData.SceneKey.World)
            {
                Entity e = this.SpawnEntity(new TypeData(Entity.Type.Player, -1), NetworkComponent.GetSession.PlayerGuid);
                System.Diagnostics.Debug.Assert(e != null, "e != null");
                e.OwnerNetworkID = NetworkComponent.GetSession.NetworkID;
            }
        }

        #endregion

        public Entity SpawnEntity(TypeData typeData, Guid guid)
        {
            Entity spawned = null;

            if (typeData.EntityType == Entity.Type.Player)
            {
                spawned = Instantiate(this.EntityList.PrefabEntityPlayer.gameObject).GetComponent<EntityPlayer>();
            }
            else
            {
                try
                {
                    spawned = Instantiate(this.EntityList.Categories[typeData.EntityTypeAsInt]
                        .Prefabs[typeData.EntityTypeIndex].gameObject).GetComponent<Entity>();
                }
                catch (Exception)
                {
                    Debug.Log(string.Format("Error, cannot spawn entity type {0}", typeData.EntityType));
                }
            }

            if (spawned == null) return null;

            spawned.Init(guid, typeData);
            spawned.OwnerNetworkID = -1;

            EntityDynamic entityDynamic = spawned as EntityDynamic;
            if (entityDynamic != null)
            {
                entityDynamic.Physics.SetPositionAndRotation(this.playerSpawn.position, this.playerSpawn.rotation);
            }

            GameManager.Events.Dispatch(new EventEntity(GameEventID.EntityInstantiate, spawned));

            return spawned;

        }
    }

}
