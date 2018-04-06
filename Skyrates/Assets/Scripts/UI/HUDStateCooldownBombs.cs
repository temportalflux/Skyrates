using Skyrates.Data;
using Skyrates.Misc;

namespace Skyrates.UI
{

    public class HUDStateCooldownBombs : HUDStateCooldown
    {

        public override bool IsVisible(PlayerData source)
        {
            return source.ViewMode == PlayerData.CameraMode.LOCK_DOWN;
        }

        public override StateCooldown GetState(PlayerData source)
        {
            return source.Artillery.Bombs;
        }

    }

}