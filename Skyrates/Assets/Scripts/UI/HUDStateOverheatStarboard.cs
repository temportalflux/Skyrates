using Skyrates.Data;
using Skyrates.Misc;

namespace Skyrates.UI
{

    public class HUDStateOverheatStarboard : HUDStateOverheat
    {

        public override bool IsVisible(PlayerData source)
        {
            return source.ViewMode == PlayerData.CameraMode.LOCK_RIGHT;
        }

        public override StateOverheat GetState(PlayerData source)
        {
            return source.Artillery.Starboard;
        }

    }

}