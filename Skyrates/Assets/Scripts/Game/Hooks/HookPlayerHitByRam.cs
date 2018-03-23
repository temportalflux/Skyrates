using Skyrates.Entity;

namespace Skyrates.Game.Event
{

    public class HookPlayerHitByRam : GameEventHook
    {

        protected override void OnEvt(GameEvent evt)
        {
            EventEntityShipHitByRam evtHit = evt as EventEntityShipHitByRam;
            if (evtHit != null && evtHit.Ship is EntityPlayerShip)
            {
                base.OnEvt(evt);
            }
        }

    }

}