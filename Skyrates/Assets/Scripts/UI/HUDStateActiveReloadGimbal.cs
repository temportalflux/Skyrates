using Skyrates.Data;
using Skyrates.Misc;

namespace Skyrates.UI
{

    public class HUDStateActiveReloadGimbal : HUDStateActiveReload
    {

        public override bool IsVisible(PlayerData source)
        {
            return source.ViewMode == PlayerData.CameraMode.FREE;
        }

        public override StateActiveReload GetState(PlayerData source)
        {
            return source.Artillery.Gimbal;
        }

    }

}