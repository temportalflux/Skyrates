using System;
using Rewired;
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
        public PlayerData PlayerData;

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
            ReInput.players.GetPlayer(0).AddInputEventDelegate(this.OnInputMenu, UpdateLoopType.Update, "Menu");
            ReInput.players.GetPlayer(0).AddInputEventDelegate(this.OnInputExit, UpdateLoopType.Update, "Exit");
            Events.MenuButtonPressed += this.OnMenuButton;
        }

        /// <inheritdoc />
        private void OnDisable()
        {
            Events.SceneLoaded -= this.OnSceneLoaded;
            ReInput.players.GetPlayer(0).RemoveInputEventDelegate(this.OnInputMenu);
            ReInput.players.GetPlayer(0).RemoveInputEventDelegate(this.OnInputExit);
            Events.MenuButtonPressed -= this.OnMenuButton;
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

        void OnInputMenu(InputActionEventData evt)
        {
            if (!evt.GetButtonDown())
                return;

            GameManager.Events.Dispatch(new EventMenuButtonPressed(EventMenuButtonPressed.MenuButton.Menu));
        }

        void OnInputExit(InputActionEventData evt)
        {
            if (!evt.GetButtonDown())
                return;

            GameManager.Events.Dispatch(new EventMenuButtonPressed(EventMenuButtonPressed.MenuButton.Back));
        }

        private void OnMenuButton(GameEvent evt)
        {
            EventMenuButtonPressed evtButton = (EventMenuButtonPressed) evt;
            switch (evtButton.Button)
            {
                case EventMenuButtonPressed.MenuButton.Menu:
                    // Go back to main menu                
                    SceneLoader.Instance.Enqueue(SceneData.SceneKey.MenuMain);
                    SceneLoader.Instance.ActivateNext();
                    break;
                case EventMenuButtonPressed.MenuButton.Back:
                    // Exit the game
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();          
#endif
                    break;
                default:
                    break;
            }
        }

        #endregion

        public void StartStandalone()
        {
            this.StartGame();
            // Start the world asap
            SceneLoader.Instance.ActivateNext();
        }

        private void StartGame()
        {
            SceneLoader.Instance.Enqueue(SceneData.SceneKey.LoadingWorld);
            SceneLoader.Instance.ActivateNext();

            SceneLoader.Instance.Enqueue(SceneData.SceneKey.World);

            GameManager.Events.Dispatch(new EventGameStart());
        }

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
