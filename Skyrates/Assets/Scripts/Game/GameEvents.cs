using UnityEngine;

namespace Skyrates.Client.Game.Event
{

    /// <summary>
    /// All the types of Game Events (each must have their own event delegate).
    /// </summary>
    public enum GameEventID
    {

        GameStart,

        #region General/Scene

        SceneLoaded,

        #endregion

        #region Entity

        EntityInstantiate,
        EntityStart,
        EntityDestroy,
        
        SpawnEntityProjectile,
        ArtilleryFired,
        EntityShipDamaged,
        EntityShipHitByProjectile,
        EntityShipHitByRam,

        EnemyTargetEngage,
        EnemyTargetDisengage,

        #endregion

        #region Player

        PlayerMoved,
        PlayerLeft,
        LootCollided,
        LootCollected,
        ActiveReloadBegin,
        PlayerInteract,

        #endregion

        MenuButtonPressed,

    }

    /// <summary>
    /// The template for events of GameEvent.
    /// </summary>
    /// <param name="evt"></param>
    public delegate void GameEventDelegate(GameEvent evt);

    /// <summary>
    /// Dispatcher of <see cref="GameEvent"/> instances, and place for observers to subscribe to events via their delegate.
    /// </summary>
    public class GameEvents : IEventDelegate<GameEventID, GameEventDelegate>
    {

        public event GameEventDelegate GameStart;
        #region General/Scene
        public event GameEventDelegate SceneLoaded;
        #endregion
        #region Entity
        public event GameEventDelegate EntityInstantiate;
        public event GameEventDelegate EntityStart;
        public event GameEventDelegate EntityDestroy;
        public event GameEventDelegate SpawnEntityProjectile;
        public event GameEventDelegate ArtilleryFired;
        public event GameEventDelegate EntityShipDamaged;
        public event GameEventDelegate EntityShipHitByProjectile;
        public event GameEventDelegate EntityShipHitByRam;
        public event GameEventDelegate EnemyTargetEngage;
        public event GameEventDelegate EnemyTargetDisengage;
        #endregion
        #region Player
        public event GameEventDelegate PlayerMoved;
        public event GameEventDelegate PlayerLeft;
        public event GameEventDelegate LootCollided;
        public event GameEventDelegate LootCollected;
        public event GameEventDelegate ActiveReloadBegin;
        public event GameEventDelegate PlayerInteract;
        #endregion

        public event GameEventDelegate MenuButtonPressed;

        /// <summary>
        /// Get the event delegate for the event ID.
        /// </summary>
        /// <param name="eventID"></param>
        /// <returns></returns>
        public GameEventDelegate Delegate(GameEventID eventID)
        {
            switch (eventID)
            {
                case GameEventID.GameStart:
                    return this.GameStart;
                #region General/Scene
                case GameEventID.SceneLoaded:
                    return this.SceneLoaded;
                #endregion
                #region Entity
                case GameEventID.EntityInstantiate:
                    return this.EntityInstantiate;
                case GameEventID.EntityStart:
                    return this.EntityStart;
                case GameEventID.EntityDestroy:
                    return this.EntityDestroy;
                case GameEventID.SpawnEntityProjectile:
                    return this.SpawnEntityProjectile;
                case GameEventID.ArtilleryFired:
                    return this.ArtilleryFired;
                case GameEventID.EntityShipDamaged:
                    return this.EntityShipDamaged;
                case GameEventID.EntityShipHitByProjectile:
                    return this.EntityShipHitByProjectile;
                case GameEventID.EntityShipHitByRam:
                    return this.EntityShipHitByRam;
                case GameEventID.EnemyTargetEngage:
                    return this.EnemyTargetEngage;
                case GameEventID.EnemyTargetDisengage:
                    return this.EnemyTargetDisengage;
                #endregion
                #region Player
                case GameEventID.PlayerMoved:
                    return this.PlayerMoved;
                case GameEventID.PlayerLeft:
                    return this.PlayerLeft;
                case GameEventID.LootCollided:
                    return this.LootCollided;
                case GameEventID.LootCollected:
                    return this.LootCollected;
                case GameEventID.ActiveReloadBegin:
                    return this.ActiveReloadBegin;
                case GameEventID.PlayerInteract:
                    return this.PlayerInteract;
                #endregion
                case GameEventID.MenuButtonPressed:
                    return this.MenuButtonPressed;
                default:
                    Debug.LogWarning(string.Format("No delegate for event {0}", eventID));
                    return null;
            }
        }

        /// <summary>
        /// Invoke all subcriptions to the event delegate for the event.
        /// </summary>
        /// <param name="evt"></param>
        public void Dispatch(GameEvent evt)
        {
            GameEventDelegate evtDelegate = this.Delegate(evt.EventID);
            if (evtDelegate != null)
            {
                evtDelegate.Invoke(evt);
            }
        }

    }

}
