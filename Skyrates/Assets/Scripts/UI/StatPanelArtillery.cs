using Skyrates.Ship;

namespace Skyrates.UI
{

    public class StatPanelArtillery : StatPanel
    {

        public StatBar FireRate;
        public StatBar Range;
        public StatBar Damage;

        public override void UpdateStats(ShipComponentList compList, ShipData rig)
        {
            this.FireRate.SetAmountFilled(this.GetStat<ShipArtillery>(compList, rig, comp => comp.RateOfFireModifier));
            this.Range.SetAmountFilled(this.GetStat<ShipArtillery>(compList, rig, comp => comp.DistanceModifier));
            this.Damage.SetAmountFilled(this.GetStat<ShipArtillery>(compList, rig, comp => comp.AttackModifier));
        }

    }

}
