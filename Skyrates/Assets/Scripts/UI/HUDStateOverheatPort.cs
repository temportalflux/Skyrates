using Skyrates.Data;
using Skyrates.Misc;

namespace Skyrates.UI
{

    public class HUDStateOverheatPort : HUDStateOverheat
    {

        public override bool IsVisible(PlayerData source)
        {
            return source.ViewMode == PlayerData.CameraMode.LOCK_LEFT;
        }

        public override StateOverheat GetState(PlayerData source)
        {
            return source.Artillery.Port;
        }

    }

}