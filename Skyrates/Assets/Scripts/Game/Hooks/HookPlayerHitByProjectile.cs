using Skyrates.Client.Entity;
using Skyrates.Client.Game.Event;

namespace Skyrates.Game.Event
{

    public class HookPlayerHitByProjectile : GameEventHook
    {

        protected override void OnEvt(GameEvent evt)
        {
            EventEntityShipHitByProjectile evtHit = evt as EventEntityShipHitByProjectile;
            if (evtHit != null && evtHit.Ship is EntityPlayerShip)
            {
                base.OnEvt(evt);
            }
        }

    }

}