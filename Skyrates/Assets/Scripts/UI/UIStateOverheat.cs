using System.Collections;
using System.Collections.Generic;
using Skyrates.Misc;
using Skyrates.UI;
using UnityEngine;

public class UIStateOverheat : UIStateCooldown<StateOverheat>
{

    public override void UpdateWith(StateOverheat state, bool isVisible)
    {
        base.UpdateWith(state, isVisible);
        this.PercentComplete = 1.0f - state.GetPercentLoaded();
    }

}
