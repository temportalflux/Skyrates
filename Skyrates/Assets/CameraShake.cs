using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using UnityEngine;

// source: https://gist.github.com/ftvs/5822103
public class CameraShake : MonoBehaviour
{
    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    public Transform camTransform;

    // How long the object should shake for.
    public float ShakeDuration = 0f;
    private float _shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    Vector3 originalPos;

    void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        originalPos = camTransform.localPosition;
        GameManager.Events.EntityShipHitByProjectile += this.OnHitBy;
    }

    void OnDisable()
    {
        GameManager.Events.EntityShipHitByProjectile -= this.OnHitBy;
    }

    void Update()
    {
        if (_shakeDuration > 0)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            _shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            _shakeDuration = 0f;
            camTransform.localPosition = originalPos;
        }
    }

    void OnHitBy(GameEvent evt)
    {
        this._shakeDuration = this.ShakeDuration;
    }

}
