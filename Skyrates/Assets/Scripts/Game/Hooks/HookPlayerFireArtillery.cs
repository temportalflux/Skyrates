using Skyrates.Entity;

namespace Skyrates.Game.Event
{

    public class HookPlayerFireArtillery : GameEventHook
    {

        protected override void OnEvt(GameEvent evt)
        {
            EventArtilleryFired evtFired = evt as EventArtilleryFired;
            if (evtFired != null && evtFired.Entity is EntityPlayerShip)
            {
                base.OnEvt(evt);
            }
        }

    }

}
