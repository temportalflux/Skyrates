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
            if (((EventSceneLoaded) evt).Scene == SceneData.SceneKey.World)
            {
                Entity e = this.SpawnEntity(new TypeData(Entity.Type.Player, -1), NetworkComponent.GetSession.PlayerGuid);
                System.Diagnostics.Debug.Assert(e != null, "e != null");
                e.OwnerNetworkID = NetworkComponent.GetSession.NetworkID;
            }
        }

        #endregion

        public Entity SpawnEntity(TypeData typeData, Guid guid)
        {
            switch (typeData.EntityType)
            {
                case Entity.Type.Player:
                    EntityPlayer entityPlayer = Instantiate(this.EntityList.PrefabEntityPlayer.gameObject).GetComponent<EntityPlayer>();
                    entityPlayer.Physics.SetPositionAndRotation(this.playerSpawn.position, this.playerSpawn.rotation);
                    entityPlayer.Init(guid, typeData);
                    entityPlayer.OwnerNetworkID = -1;
                    GameManager.Events.Dispatch(new EventEntity(GameEventID.EntityInstantiate, entityPlayer));
                    return entityPlayer;
                default:
                    Debug.Log(string.Format("Spawn {0}:{1} {2}", typeData.EntityType, typeData.EntityTypeIndex, guid));
                    return null;
            }
        }

    }

}
