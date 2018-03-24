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
            this.PercentComplete = 1 - state.GetPercentLoaded();
            if (this.PercentComplete <= 0.0f) this.PercentComplete = 1.0f;
        }

    }

    [RequireComponent(typeof(RectTransform))]
    public class UIStateCooldown<T> : MonoBehaviour where T : StateCooldown
    {

        public Canvas Owner;
        public Image Empty;
        public Image Full;

        protected RectTransform Rect;

        public bool _isIsActive = false;

        protected bool IsActive
        {
            get { return this._isIsActive; }
            set
            {
                this._isIsActive = value;
                this.IsVisible = this.IsVisible;
            }
        }

        private bool _isVisible = false;

        protected bool IsVisible
        {
            get { return this._isVisible; }
            set
            {
                this._isVisible = value && this.IsActive;
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

        protected virtual void Awake()
        {
            this.Rect = this.GetComponent<RectTransform>();
            this.Full.rectTransform.sizeDelta = this.Rect.sizeDelta;
            this.IsActive = this.IsActiveOnAwake();
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