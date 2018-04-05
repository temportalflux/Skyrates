using Skyrates.Data;
using UnityEngine;

namespace Skyrates.UI
{
    public class UIGameplay : MonoBehaviour
    {

        public PlayerData Source;

        public HUDState Gimbal;
        public HUDState Starboard;
        public HUDState Port;
        public HUDState Bombs;
        
        void Update()
        {
            this.Gimbal.UpdateWith(this.Source);
            this.Starboard.UpdateWith(this.Source);
            this.Port.UpdateWith(this.Source);
            this.Bombs.UpdateWith(this.Source);
        }

    }
}