using System;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Skyrates.Client.Game.Event
{

    public enum GameEventID
    {

        #region Entity

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

        public event GameEventDelegate EntityStart;
        public event GameEventDelegate EntityDestroy;
        public event GameEventDelegate PlayerMoved;

        public GameEventDelegate Delegate(GameEventID eventID)
        {
            switch (eventID)
            {
                case GameEventID.EntityStart:
                    return this.EntityStart;
                case GameEventID.EntityDestroy:
                    return this.EntityDestroy;
                case GameEventID.PlayerMoved:
                    return this.PlayerMoved;
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
