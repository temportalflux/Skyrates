using Skyrates.Client.Ship;

namespace Skyrates.Client.Game.Event
{

    public class EventArtilleryFired : EventEntityPlayerShip
    {

        public ShipArtillery[] Artillery;

        public Projectile[] Projectiles;

        public EventArtilleryFired(EntityPlayerShip playerShip, ShipArtillery[] artillery, Projectile[] projectiles)
            : base(GameEventID.ArtilleryFired, playerShip)
        {
            this.Artillery = artillery;
            this.Projectiles = projectiles;
        }

    }

}
