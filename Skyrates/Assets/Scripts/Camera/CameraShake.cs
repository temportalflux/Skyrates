using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Skyrates.Client.Entity;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using UnityEngine;

// source: https://gist.github.com/ftvs/5822103
public class CameraShake : MonoBehaviour
{

    // Transform of the camera to shake
    public CinemachineStateDrivenCamera camera;

    // How long the object should shake for.
    public float ShakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float Amplitude = 0.7f;

    public float Frequency = 1.0f;

    public bool Continuous = false;

    private Coroutine _shake = null;
    
    private void SetNoise(float amplitude, float frequency)
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

    void OnEnable()
    {
        GameManager.Events.EntityShipHitByProjectile += this.OnHitBy;
    }

    void OnDisable()
    {
        GameManager.Events.EntityShipHitByProjectile -= this.OnHitBy;
    }

    void Update()
    {
        if (this.Continuous)
            this.SetNoise(this.Amplitude, this.Frequency);
    }

    void StartShake()
    {
        if (this._shake != null)
        {
            StopCoroutine(this._shake);
            this._shake = null;
        }

        this._shake = StartCoroutine(this.Shake());
    }
    
    IEnumerator Shake()
    {
        this.SetNoise(this.Amplitude, this.Frequency);

        yield return new WaitForSeconds(this.ShakeDuration);

        this.SetNoise(0, 0);

        this._shake = null;
    }

    void OnHitBy(GameEvent evt)
    {
        EventEntityShipHitByProjectile evtHit = (EventEntityShipHitByProjectile) evt;
        if (evtHit.Ship is EntityPlayerShip)
        {
            this.StartShake();
        }
    }

}
