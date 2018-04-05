using Skyrates.Ship;

namespace Skyrates.UI
{
    
    public class StatPanelNavigation : StatPanel
    {

        public StatBar TurnSpeed;

        public override void UpdateStats(ShipComponentList compList, ShipData rig)
        {
            this.TurnSpeed.SetAmountFilled(this.GetStat<ShipNavigation>(compList, rig, comp => comp.TurnSpeed));
        }

    }

}
