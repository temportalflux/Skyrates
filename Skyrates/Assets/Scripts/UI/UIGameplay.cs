using Skyrates.Data;
using Skyrates.Game;
using Skyrates.Game.Event;
using UnityEngine;

namespace Skyrates.UI
{
    public class UIGameplay : MonoBehaviour
    {

        public PlayerData Source;

        public UIStateActiveReload Gimbal;
        public UIStateOverheat Starboard;
        public UIStateOverheat Port;
        public UIStateCooldown Bombs;
        
        void Update()
        {
            this.Gimbal.UpdateWith(this.Source.Artillery.Gimbal,
                this.Source.ViewMode == PlayerData.CameraMode.FREE);
            this.Starboard.UpdateWith(this.Source.Artillery.Starboard,
                this.Source.ViewMode == PlayerData.CameraMode.LOCK_RIGHT);
            this.Port.UpdateWith(this.Source.Artillery.Port,
                this.Source.ViewMode == PlayerData.CameraMode.LOCK_LEFT);
            this.Bombs.UpdateWith(this.Source.Artillery.Bombs,
                this.Source.ViewMode == PlayerData.CameraMode.LOCK_DOWN);
        }

    }
}