using Skyrates.Misc;
using UnityEngine;
using UnityEngine.UI;

namespace Skyrates.UI
{

    public class UIStateCooldown : UIStateCooldown<StateCooldown>
    {
        
        public override void UpdateWith(StateCooldown state)
        {
            base.UpdateWith(state);
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

        public bool IsActive
        {
            get { return this._isIsActive; }
            set
            {
                this._isIsActive = value;
                this.OnSetVisibility();
            }
        }
        
        protected float PercentComplete
        {
            get { return this.Full.fillAmount; }
            set { this.Full.fillAmount = value; }
        }

        public virtual void OnAwake()
        {
            this.Rect = this.GetComponent<RectTransform>();
            this.Full.rectTransform.sizeDelta = this.Rect.sizeDelta;
            this.IsActive = false;
        }
        
        protected virtual void OnSetVisibility()
        {
            this.Empty.enabled = this.IsActive;
            this.Full.enabled = this.IsActive;
        }

        public virtual void UpdateWith(T state)
        {
        }

    }

}