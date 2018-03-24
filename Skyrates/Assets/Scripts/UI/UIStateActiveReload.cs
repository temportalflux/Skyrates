using System;
using System.Collections;
using Skyrates.Misc;
using UnityEngine;
using UnityEngine.UI;

namespace Skyrates.UI
{

    public class UIStateActiveReload : UIStateCooldown<StateActiveReload>
    {
        
        public Image Cutoff;

        protected override bool IsActiveOnAwake()
        {
            return false;
        }

        protected override void OnSetVisibility()
        {
            this.Cutoff.enabled = this.IsVisible;
        }

        public override void UpdateWith(StateActiveReload state, bool isVisible)
        {
            base.UpdateWith(state, isVisible);
            if (!this.Active && state.IsLoading)
            {
                // Start
                this.SetCutoff(state.PercentStart, state.PercentEnd);
                this.PercentComplete = 0.0f;
                this.Active = true;
            }
            else if (this.Active && !state.IsLoading)
            {
                // End
                this.Active = false;
                this.SetCutoff(0.0f, 0.0f);
                this.PercentComplete = 0.0f;
            }
            else
            {
                // Update
                this.PercentComplete = Mathf.Min(1.0f, state.GetPercentLoaded());
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