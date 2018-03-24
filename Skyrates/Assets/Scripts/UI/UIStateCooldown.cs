using System.Collections;
using System.Collections.Generic;
using Skyrates.Misc;
using UnityEngine;
using UnityEngine.UI;

namespace Skyrates.UI
{

    public class UIStateCooldown : UIStateCooldown<StateCooldown>
    {

        public override void UpdateWith(StateCooldown state, bool isVisible)
        {
            base.UpdateWith(state, isVisible);
            this.PercentComplete = state.GetPercentLoaded();
        }

    }

    [RequireComponent(typeof(RectTransform))]
    public class UIStateCooldown<T> : MonoBehaviour where T : StateCooldown
    {

        public Canvas Owner;
        public Image Empty;
        public Image Full;

        protected RectTransform Rect;

        protected bool Active = false;

        private bool _isVisible = false;

        protected bool IsVisible
        {
            get { return this._isVisible; }
            set
            {
                this._isVisible = value && this.Active;
                this.Empty.enabled = this._isVisible;
                this.Full.enabled = this._isVisible;
                this.OnSetVisibility();
            }
        }

        protected float PercentComplete
        {
            get { return this.Full.fillAmount; }
            set { this.Full.fillAmount = value; }
        }

        void Awake()
        {
            this.Rect = this.GetComponent<RectTransform>();
            this.Full.rectTransform.sizeDelta = this.Rect.sizeDelta;
            this.Active = this.IsActiveOnAwake();
            this.IsVisible = false;
        }

        protected virtual bool IsActiveOnAwake()
        {
            return true;
        }
        
        protected virtual void OnSetVisibility()
        {
        }

        public virtual void UpdateWith(T state, bool isVisible)
        {
            this.IsVisible = isVisible;
        }

    }

}