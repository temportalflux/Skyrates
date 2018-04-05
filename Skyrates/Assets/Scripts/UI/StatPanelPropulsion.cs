using Skyrates.Ship;

namespace Skyrates.UI
{

    public class StatPanelPropulsion : StatPanel
    {

        public StatBar Thrust;

        public override void UpdateStats(ShipComponentList compList, ShipData rig)
        {
            this.Thrust.SetAmountFilled(this.GetStat<ShipPropulsion>(compList, rig, comp => comp.Thrust));
        }

    }

}
