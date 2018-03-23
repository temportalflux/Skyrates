using Skyrates.Entity;

namespace Skyrates.Game.Event
{

    public class HookPlayerHitWithRam : GameEventHook
    {

        protected override void OnEvt(GameEvent evt)
        {
            EventEntityShipHitByRam evtHit = evt as EventEntityShipHitByRam;
            if (evtHit != null && evtHit.Figurehead.Ship is EntityPlayerShip)
            {
                base.OnEvt(evt);
            }
        }

    }

}