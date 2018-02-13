using System;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Skyrates.Client.Game.Event
{

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

        #endregion

        #region Player

        PlayerMoved,
        PlayerLeft,
        LootCollided,
        LootCollected,

        #endregion

    }

    public delegate void GameEventDelegate(GameEvent evt);

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
        #endregion
        #region Player
        public event GameEventDelegate PlayerMoved;
        public event GameEventDelegate PlayerLeft;
        public event GameEventDelegate LootCollided;
        public event GameEventDelegate LootCollected;
        #endregion

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
                #endregion
                default:
                    Debug.LogWarning(string.Format("No delegate for event {0}", eventID));
                    return null;
            }
        }

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
