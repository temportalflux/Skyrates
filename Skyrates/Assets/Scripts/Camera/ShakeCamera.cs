using UnityEngine;

namespace Skyrates.Cinemachine
{
    
    public class ShakeCamera : MonoBehaviour
    {

        public CinemachineShake Shaker;

        // How long the object should shake for.
        public float Duration = 1.0f;

        // Amplitude of the shake. A larger value shakes the camera harder.
        public float Amplitude = 0.2f;

        public float Frequency = 2.0f;

        public bool Continuous = false;
        
        void Update()
        {
            if (this.Continuous)
                this.Shaker.SetNoise(this.Amplitude, this.Frequency);
        }
        
        public void StartShake()
        {
            this.Shaker.StartShake(this.Amplitude, this.Frequency, this.Duration);
        }

    }

}
