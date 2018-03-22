using Skyrates.Entity;

namespace Skyrates.Game.Event
{
    
    public class EventActiveReloadBegin : EventEntityPlayerShip
    {

        public bool IsStarboard;

        public float ActiveReloadStart;
        public float ActiveReloadEnd;

        public EventActiveReloadBegin(EntityPlayerShip playerShip,
            bool isStarboard, float percentStart, float percentEnd)
            : base(GameEventID.ActiveReloadBegin, playerShip)
        {
            this.IsStarboard = isStarboard;
            this.ActiveReloadStart = percentStart;
            this.ActiveReloadEnd = percentEnd;
        }

        public float GetPercentUpdate()
        {
            return (this.IsStarboard
                ? this.PlayerShip.PlayerData.Artillery.Starboard.Reload
                : this.PlayerShip.PlayerData.Artillery.Port.Reload).GetPercentLoaded();
        }

    }

}
