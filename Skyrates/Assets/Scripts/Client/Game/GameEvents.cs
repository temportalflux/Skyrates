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

        #endregion

        #region Player

        PlayerMoved,

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
        #endregion
        #region Player
        public event GameEventDelegate PlayerMoved;
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
                #endregion
                #region Player
                case GameEventID.PlayerMoved:
                    return this.PlayerMoved;
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
