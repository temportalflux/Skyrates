using System.Collections;
using System.Collections.Generic;
using Skyrates.Misc;
using Skyrates.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIStateOverheat : UIStateCooldown<StateOverheat>
{

    public Image Cutoff;

    protected override void OnSetVisibility()
    {
        this.Cutoff.enabled = this.IsVisible;
    }

    public override void UpdateWith(StateOverheat state, bool isVisible)
    {
        base.UpdateWith(state, isVisible);
        this.SetCutoff(state.OverheatMin, 1.0f);
        this.PercentComplete = state.PercentComplete;
    }

    // assume: percent [0, 1]
    public void SetCutoff(float percentStart, float percentEnd)
    {
        this.Cutoff.rectTransform.position = new Vector3(
            this.Empty.rectTransform.position.x + this.Owner.scaleFactor * (
                -(0.5f * this.Rect.sizeDelta.x)
                +(percentStart * this.Rect.sizeDelta.x)
            ),
            this.Rect.position.y,
            this.Rect.position.z);
        this.Cutoff.rectTransform.sizeDelta = new Vector2(
            this.Rect.sizeDelta.x * (percentEnd - percentStart),
            this.Rect.sizeDelta.y
        );
    }

}
