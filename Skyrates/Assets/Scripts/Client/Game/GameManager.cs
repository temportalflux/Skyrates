using System;
using Skyrates.Client.Data;
using Skyrates.Client.Entity;
using Skyrates.Client.Game.Event;
using Skyrates.Client.Scene;
using Skyrates.Common.Entity;
using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Client.Game
{

    /// <inheritdoc />
    /// <summary>
    /// Singleton which encapsulates <see cref="T:Skyrates.Client.Game.Event.GameEvents" />, local data, and win state.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {

        /// <summary>
        /// The <see cref="Singleton{GameManager}"/> instance.
        /// </summary>
        public static GameManager Instance;

        /// <summary>
        /// Static reference to the <see cref="GameEvents"/> dispatcher.
        /// </summary>
        public static GameEvents Events
        {
            get { return Instance != null ? Instance._events : new GameEvents(); }
        }

        /// <summary>
        /// The <see cref="GameEvents"/> dispatcher.
        /// </summary>
        private GameEvents _events;

        // TODO: Move this somewhere pertinent to only the world.
        /// <summary>
        /// The spawn location of the player.
        /// </summary>
        public Transform PlayerSpawn;

        /// <summary>
        /// Local player data - nonnetworked.
        /// </summary>
        public LocalData PlayerData;

        public EntityPlayerShip PlayerPrefab;

        /// <inheritdoc />
        private void Awake()
        {
            this.loadSingleton(this, ref Instance);
            this._events = new GameEvents();
        }

        #region Events

        /// <inheritdoc />
        private void OnEnable()
        {
            Events.SceneLoaded += this.OnSceneLoaded;
            Events.LootCollected += this.OnLootCollected;
        }

        /// <inheritdoc />
        private void OnDisable()
        {
            Events.SceneLoaded -= this.OnSceneLoaded;
            Events.LootCollected -= this.OnLootCollected;
        }

        /// <summary>
        /// Called when <see cref="SceneLoader"/> has finished loading a scene.
        /// Used to spawn the player.
        /// </summary>
        /// <param name="evt"></param>
        private void OnSceneLoaded(GameEvent evt)
        {
            EventSceneLoaded evtScene = (EventSceneLoaded) evt;
            switch (evtScene.Scene)
            {
                case SceneData.SceneKey.MenuMain:
                    break;
                case SceneData.SceneKey.World:
                    // spawn the player
                    EntityPlayerShip player = this.SpawnEntity(this.PlayerPrefab) as EntityPlayerShip;
                    player.Physics.SetPositionAndRotation(this.PlayerSpawn.position, this.PlayerSpawn.rotation);
                    player.transform.position = player.Physics.LinearPosition;
                    player.transform.rotation = player.Physics.RotationPosition;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Called when a player collects a loot. Used to check win-state.
        /// </summary>
        /// <param name="evt"></param>
        private void OnLootCollected(GameEvent evt)
        {
            // TODO: Remove the temporary winstate based on number of loot collected.
            if (this.PlayerData.LootCount >= this.PlayerData.LootGoal)
            {
                //SceneLoader.Instance.Enqueue(SceneData.SceneKey.MenuMain);
                //SceneLoader.Instance.ActivateNext();
            }
        }

        #endregion

        /// <summary>
        /// Spawns an entity based on the prefab in <see cref="EntityList"/> keyed by <see cref="Entity.TypeData"/> and some <see cref="Guid"/>.
        /// </summary>
        /// <param name="typeData"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Common.Entity.Entity SpawnEntity(Common.Entity.Entity prefab)
        {
            Debug.Assert(prefab != null);

            Common.Entity.Entity spawned = Instantiate(prefab.gameObject).GetComponent<Common.Entity.Entity>();
            
            if (spawned == null) return null;

            GameManager.Events.Dispatch(new EventEntity(GameEventID.EntityInstantiate, spawned));

            return spawned;

        }

    }

}
