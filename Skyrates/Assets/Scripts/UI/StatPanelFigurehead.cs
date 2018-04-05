using Skyrates.Ship;

namespace Skyrates.UI
{

    public class StatPanelFigurehead : StatPanel
    {

        public StatBar Damage;

        public override void UpdateStats(ShipComponentList compList, ShipData rig)
        {
            this.Damage.SetAmountFilled(this.GetStat<ShipFigurehead>(compList, rig, comp => comp.Attack));
        }

    }

}
