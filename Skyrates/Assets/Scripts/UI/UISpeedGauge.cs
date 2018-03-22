using Skyrates.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Skyrates.UI
{

    [RequireComponent(typeof(Image))]
    public class UISpeedGauge : MonoBehaviour
    {

        public PlayerData PlayerData;

        private Image _arrow;

        public float AngleMax;
        public float AngleMin;

        void Start()
        {
            this._arrow = this.GetComponent<Image>();
        }
        
        private void Update()
        {
            float speedRange = this.PlayerData.Movement.SpeedMax - this.PlayerData.Movement.SpeedMin;
            float speedOffset = this.PlayerData.Movement.CurrentSpeed - (speedRange * 0.5f);
            float scaled = speedOffset / speedRange + 0.5f;

            float angleRange = this.AngleMax - this.AngleMin;
            scaled *= angleRange;
            scaled += this.AngleMin;

            this._arrow.rectTransform.rotation = Quaternion.Euler(0.0f, 0.0f, -scaled);
        }

    }

}