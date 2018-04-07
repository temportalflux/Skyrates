using System;
using System.Collections.Generic;
using System.Linq;
using Skyrates.Game;
using Skyrates.Game.Event;
using UnityEngine;

namespace Skyrates.UI
{


    [Serializable]
    public class CanvasSelectorEntry : CanvasSelectorEntry<EventMenu.CanvasType>
    {
    }
    
    public class CanvasSelector : CanvasSelector<EventMenu.CanvasType, CanvasSelectorEntry>
    {
        
        public override bool IsEqual(EventMenu.CanvasType v1, EventMenu.CanvasType v2)
        {
            return v1 == v2;
        }

        void OnEnable()
        {
            GameManager.Events.MenuOpen += this.OnMenu;
            GameManager.Events.MenuClose += this.OnMenu;
        }

        void OnDisable()
        {
            GameManager.Events.MenuOpen -= this.OnMenu;
            GameManager.Events.MenuClose -= this.OnMenu;
        }

        void OnMenu(GameEvent evt)
        {
            EventMenu evtMenu = (EventMenu) evt;
            if (evtMenu.ShouldOpen)
            {
                this.Enter(evtMenu.Type);
            }
            else
            {
                this.Exit();
                if (evtMenu.Type != EventMenu.CanvasType.Hud)
                {
                    this.Enter(EventMenu.CanvasType.Hud);
                }
            }
        }

    }

    public abstract class CanvasSelectorEntry<TEntryKey>
    {
        public TEntryKey Key;
        public Canvas Value;
    }

    public abstract class CanvasSelector<TKey, TEntry> : MonoBehaviour
        where TEntry : CanvasSelectorEntry<TKey>
    {

        [SerializeField]
        public TKey Invalid;

        [SerializeField]
        public TKey Default;

        public TEntry[] Entries;

        private Dictionary<TKey, Canvas> _canvasSet;
        private TKey _current;

        public abstract bool IsEqual(TKey v1, TKey v2);

        protected virtual void Awake()
        {
            this._canvasSet = this.Entries.ToDictionary(entry => entry.Key, entry => entry.Value);
            this.Enter(this.Default);
        }

        protected void Enter(TKey key)
        {
            if (!this.IsEqual(this._current, this.Invalid))
            {
                this.Exit();
            }

            this._current = key;

            if (this._canvasSet.ContainsKey(this._current))
            {
                this.Toggle(this._canvasSet[this._current], true);
            }
        }

        protected void Exit()
        {
            if (this._canvasSet.ContainsKey(this._current))
            {
                this.Toggle(this._canvasSet[this._current], false);
            }
            this._current = this.Invalid;
        }

        private void Toggle(Canvas canvas, bool active)
        {
            canvas.gameObject.SetActive(active);
        }

    }

}
