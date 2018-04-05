using Skyrates.Ship;

namespace Skyrates.UI
{

    public class StatPanelHullArmor : StatPanel
    {

        public StatBar Defense;
        public StatBar Protection;

        public override void UpdateStats(ShipComponentList compList, ShipData rig)
        {
            this.Defense.SetAmountFilled(this.GetStat<ShipHullArmor>(compList, rig, comp => comp.Defense));
            this.Protection.SetAmountFilled(this.GetStat<ShipHullArmor>(compList, rig, comp => comp.Protection));
        }
        
    }

}
