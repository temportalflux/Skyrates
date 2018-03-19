using System;
using UnityEngine;
using UnityEngine.Events;

namespace Skyrates.Game.Event
{

    public class GameEventHook : MonoBehaviour
    {

        [Serializable]
        public class GameEventAction : UnityEvent<GameEvent>
        {
        }

        public GameEventID Event;

        public GameEventAction Action;

        public void OnEnable()
        {
            GameManager.Events += this._OnEvt;
        }

        public void OnDisable()
        {
            GameManager.Events -= this._OnEvt;
        }

        protected void _OnEvt(GameEvent evt)
        {
            if (evt.EventID == this.Event) this.OnEvt(evt);
        }

        protected virtual void OnEvt(GameEvent evt)
        {
            this.Action.Invoke(evt);
        }

    }

}