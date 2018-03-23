using UnityEngine;

namespace Skyrates.Game.Event
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
    public class GameEvents
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
        public bool GetDelegate(GameEventID eventID, out GameEventDelegate value)
        {
            switch (eventID)
            {
                case GameEventID.GameStart:
                    value = this.GameStart;
                    break;
                #region General/Scene
                case GameEventID.SceneLoaded:
                    value = this.SceneLoaded;
                    break;
                #endregion
                #region Entity
                case GameEventID.EntityInstantiate:
                    value = this.EntityInstantiate;
                    break;
                case GameEventID.EntityStart:
                    value = this.EntityStart;
                    break;
                case GameEventID.EntityDestroy:
                    value = this.EntityDestroy;
                    break;
                case GameEventID.SpawnEntityProjectile:
                    value = this.SpawnEntityProjectile;
                    break;
                case GameEventID.ArtilleryFired:
                    value = this.ArtilleryFired;
                    break;
                case GameEventID.EntityShipDamaged:
                    value = this.EntityShipDamaged;
                    break;
                case GameEventID.EntityShipHitByProjectile:
                    value = this.EntityShipHitByProjectile;
                    break;
                case GameEventID.EntityShipHitByRam:
                    value = this.EntityShipHitByRam;
                    break;
                case GameEventID.EnemyTargetEngage:
                    value = this.EnemyTargetEngage;
                    break;
                case GameEventID.EnemyTargetDisengage:
                    value = this.EnemyTargetDisengage;
                    #endregion
                    break;
                #region Player
                case GameEventID.PlayerMoved:
                    value = this.PlayerMoved;
                    break;
                case GameEventID.PlayerLeft:
                    value = this.PlayerLeft;
                    break;
                case GameEventID.LootCollided:
                    value = this.LootCollided;
                    break;
                case GameEventID.LootCollected:
                    value = this.LootCollected;
                    break;
                case GameEventID.ActiveReloadBegin:
                    value = this.ActiveReloadBegin;
                    break;
                case GameEventID.PlayerInteract:
                    value = this.PlayerInteract;
                    break;
                #endregion
                case GameEventID.MenuButtonPressed:
                    value = this.MenuButtonPressed;
                    break;
                default:
                    Debug.LogWarning(string.Format("No delegate for event {0}", eventID));
                    value = null;
                    break;
            }
            return value != null;
        }

        public static GameEvents operator+(GameEvents self, GameEventDelegate action)
        {
            self.GameStart += action;
            #region General/Scene
            self.SceneLoaded += action;
            #endregion
            #region Entity
            self.EntityInstantiate += action;
            self.EntityStart += action;
            self.EntityDestroy += action;
            self.SpawnEntityProjectile += action;
            self.ArtilleryFired += action;
            self.EntityShipDamaged += action;
            self.EntityShipHitByProjectile += action;
            self.EntityShipHitByRam += action;
            self.EnemyTargetEngage += action;
            self.EnemyTargetDisengage += action;
            #endregion
            #region Player
            self.PlayerMoved += action;
            self.PlayerLeft += action;
            self.LootCollided += action;
            self.LootCollected += action;
            self.ActiveReloadBegin += action;
            self.PlayerInteract += action;
            #endregion
            self.MenuButtonPressed += action;
            return self;
        }

        public static GameEvents operator -(GameEvents self, GameEventDelegate action)
        {
            self.GameStart -= action;
            #region General/Scene
            self.SceneLoaded -= action;
            #endregion
            #region Entity
            self.EntityInstantiate -= action;
            self.EntityStart -= action;
            self.EntityDestroy -= action;
            self.SpawnEntityProjectile -= action;
            self.ArtilleryFired -= action;
            self.EntityShipDamaged -= action;
            self.EntityShipHitByProjectile -= action;
            self.EntityShipHitByRam -= action;
            self.EnemyTargetEngage -= action;
            self.EnemyTargetDisengage -= action;
            #endregion
            #region Player
            self.PlayerMoved -= action;
            self.PlayerLeft -= action;
            self.LootCollided -= action;
            self.LootCollected -= action;
            self.ActiveReloadBegin -= action;
            self.PlayerInteract -= action;
            #endregion
            self.MenuButtonPressed -= action;
            return self;
        }

        /// <summary>
        /// Invoke all subcriptions to the event delegate for the event.
        /// </summary>
        /// <param name="evt"></param>
        public void Dispatch(GameEvent evt)
        {
            GameEventDelegate evtDelegate;
            if (this.GetDelegate(evt.EventID, out evtDelegate))
            {
                evtDelegate.Invoke(evt);
            }
        }
        
    }

}