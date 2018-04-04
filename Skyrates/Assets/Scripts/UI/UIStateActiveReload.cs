using Skyrates.Misc;
using UnityEngine;
using UnityEngine.UI;

namespace Skyrates.UI
{

    public class UIStateActiveReload : UIStateCooldown<StateActiveReload>
    {
        
        public Image Cutoff;

        protected override void OnSetVisibility()
        {
            base.OnSetVisibility();
            this.Cutoff.enabled = this.IsActive;
        }

        public override void UpdateWith(StateActiveReload state)
        {
            base.UpdateWith(state);
            // Cannon not loaded
            if (!state.IsLoaded())
            {
                // Cannon not loaded
                // UI inactive
                // = UI Active
                if (!this.IsActive)
                {
                    this.SetCutoff(state.PercentStart, state.PercentEnd);
                    this.PercentComplete = 0.0f;
                    this.IsActive = true;
                }
                // Cannon not loaded
                // Cannon loading
                // = Update
                else if (state.IsLoading)
                {
                    this.PercentComplete = Mathf.Min(1.0f, state.GetPercentLoaded());
                }
            }
            // Cannon loaded
            // UI Active
            // = UI inactive
            else if (this.IsActive)
            {
                this.IsActive = false;
                this.SetCutoff(0.0f, 0.0f);
                this.PercentComplete = 0.0f;
            }
        }

        // assume: percent [0, 1]
        public void SetCutoff(float percentStart, float percentEnd)
        {
            this.Cutoff.rectTransform.position = new Vector3(
                this.Empty.rectTransform.position.x + this.Owner.scaleFactor * this.Rect.sizeDelta.x * 0.2f,
                this.Rect.position.y,
                this.Rect.position.z);
            this.Cutoff.rectTransform.sizeDelta = new Vector2(
                this.Rect.sizeDelta.x * (percentEnd - percentStart),
                this.Rect.sizeDelta.y
            );
        }

    }
}