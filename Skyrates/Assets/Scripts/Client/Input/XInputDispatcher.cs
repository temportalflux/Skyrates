using System.Collections;
using System.Collections.Generic;
using Skyrates.Client;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using UnityEngine;
using XInputDotNetPure;

public class XInputDispatcher : MonoBehaviour
{

    public class Pulse
    {
        public float Duration; // in ms
        public float MotorStart;
        public float MotorEnd;

        private float DurationInv;
        private float TimeElapsed; // in ms

        public Pulse(float start, float end, float duration)
        {
            this.MotorStart = start;
            this.MotorEnd = end;
            this.Duration = duration;
            this.DurationInv = 1 / this.Duration;
            this.TimeElapsed = 0;
        }

        public float Delta()
        {
            return Mathf.Min(1.0f, this.TimeElapsed * this.DurationInv);
        }

        public static Pulse operator +(Pulse p, float delta)
        {
            p.TimeElapsed += delta;
            return p;
        }
    }

    private Pulse left, right;

    public float OnHitStrength = 0.25f;
    public float OnHitDuration = 0.3f;

    void Start()
    {
        this.left = null;
        this.right = null;
    }

    void OnEnable()
    {
        GameManager.Events.EntityShipHitByProjectile += this.OnEntityHitByProjectile;
    }

    void OnDisable()
    {
        GameManager.Events.EntityShipHitByProjectile -= this.OnEntityHitByProjectile;
    }

    float Lerp(float start, float end, float delta)
    {
        return (1 - delta) * start + (delta) * end;
    }

    void FixedUpdate()
    {
        float motorLeft = 0;
        float motorRight = 0;

        if (this.left != null)
        {
            motorLeft = Lerp(this.left.MotorStart, this.left.MotorEnd, this.left.Delta());

            this.left += Time.fixedDeltaTime;
            if (this.left.Delta() >= 1.0f)
            {
                this.left = null;
            }
        }

        if (this.right != null)
        {
            motorRight = Lerp(this.right.MotorStart, this.right.MotorEnd, this.right.Delta());

            this.right += Time.fixedDeltaTime;
            if (this.right.Delta() >= 1.0f)
            {
                this.right = null;
            }
        }

        GamePad.SetVibration(PlayerIndex.One, motorLeft, motorRight);
    }

    void OnEntityHitByProjectile(GameEvent evt)
    {
        EventEntityShipHitByProjectile evtHit = (EventEntityShipHitByProjectile) evt;

        if (!(evtHit.Ship is EntityPlayerShip)) return;

        Transform target = evtHit.Ship.GetRender();
        Transform source = evtHit.Projectile.transform;

        Vector3 targetToSource = source.position - target.position;

        // determines orthogonality of
        // target's right to the vector from target to source
        float orthogonality = Vector3.Dot(target.right, targetToSource);

        // if orthogonality is < 0, then right side
        // if orthogonality is > 0, then left side
        if (orthogonality < 0)
            this.PulseOn(ref this.right, this.OnHitStrength, this.OnHitDuration);
        else if (orthogonality > 0)
            this.PulseOn(ref this.left, this.OnHitStrength, this.OnHitDuration);

    }

    void PulseOn(ref Pulse pulseRef, float strength, float duration)
    {
        pulseRef = new Pulse(strength, 0, duration);
    }

}
