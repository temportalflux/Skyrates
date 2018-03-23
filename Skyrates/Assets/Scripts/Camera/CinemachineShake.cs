using System.Collections;
using Cinemachine;
using UnityEngine;

namespace Skyrates.Cinemachine
{

    public class CinemachineShake : MonoBehaviour
    {

        // Transform of the camera to shake
        public CinemachineStateDrivenCamera camera;

        private Coroutine _shake = null;

        public void SetNoise(float amplitude, float frequency)
        {
            foreach (CinemachineVirtualCameraBase cameraBase in this.camera.ChildCameras)
            {
                this.SetNoise(cameraBase, amplitude, frequency);
            }
        }

        private void SetNoise(CinemachineVirtualCameraBase cameraBase, float amplitude, float frequency)
        {
            if (cameraBase is CinemachineVirtualCamera)
            {
                this.SetNoise(
                    (cameraBase as CinemachineVirtualCamera).
                        GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>(),
                    amplitude, frequency);
            }
            else if (cameraBase is CinemachineFreeLook)
            {
                for (int i = 0; i < 3; i++)
                    this.SetNoise((cameraBase as CinemachineFreeLook).GetRig(i), amplitude, frequency);
            }
        }

        private void SetNoise(CinemachineBasicMultiChannelPerlin comp, float amplitude, float frequency)
        {
            comp.m_AmplitudeGain = amplitude;
            comp.m_FrequencyGain = frequency;
        }

        public void StartShake(float amplitude, float frequency, float duration)
        {
            if (this._shake != null)
            {
                StopCoroutine(this._shake);
                this._shake = null;
            }

            this._shake = StartCoroutine(this.Shake(amplitude, frequency, duration));
        }

        IEnumerator Shake(float amplitude, float frequency, float duration)
        {
            this.SetNoise(amplitude, frequency);

            yield return new WaitForSeconds(duration);

            this.SetNoise(0, 0);

            this._shake = null;
        }

    }
    
}
