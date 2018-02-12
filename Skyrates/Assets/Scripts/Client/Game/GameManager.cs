using System;
using Skyrates.Client.Game.Event;
using Skyrates.Common.Entity;
using Skyrates.Common.Network;
using UnityEngine;

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

        #region Events

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
                Common.Entity.Entity e = this.SpawnEntity(new Common.Entity.Entity.TypeData(Common.Entity.Entity.Type.Player, -1), NetworkComponent.GetSession.PlayerGuid);
                System.Diagnostics.Debug.Assert(e != null, "e != null");
                e.OwnerNetworkID = NetworkComponent.GetSession.NetworkID;
            }
        }

        #endregion

        public Common.Entity.Entity SpawnEntity(Common.Entity.Entity.TypeData typeData, Guid guid)
        {
            Common.Entity.Entity spawned = null;

            if (typeData.EntityType == Common.Entity.Entity.Type.Player)
            {
                EntityPlayerShip player = Instantiate(this.EntityList.PrefabEntityPlayer.gameObject).GetComponent<EntityPlayerShip>();
                player.Physics.SetPositionAndRotation(this.playerSpawn.position, this.playerSpawn.rotation);
                player.transform.position = player.Physics.LinearPosition;
                player.transform.rotation = player.Physics.RotationPosition;
                spawned = player;
            }
            else
            {
                try
                {
                    spawned = Instantiate(this.EntityList.Categories[typeData.EntityTypeAsInt]
                        .Prefabs[typeData.EntityTypeIndex].gameObject).GetComponent<Common.Entity.Entity>();
                }
                catch (Exception)
                {
                    Debug.Log(string.Format("Error, cannot spawn entity type {0}", typeData.EntityType));
                }
            }

            if (spawned == null) return null;
            
            spawned.Init(guid, typeData);

            GameManager.Events.Dispatch(new EventEntity(GameEventID.EntityInstantiate, spawned));

            return spawned;

        }
    }

}
