using Skyrates.Client.Entity;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using UnityEngine;
using XInputDotNetPure;

namespace Skyrates.Client.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class XInputDispatcher : MonoBehaviour
    {

        /// <summary>
        /// 
        /// </summary>
        public class Pulse
        {
            /// <summary>
            /// 
            /// </summary>
            public float Duration; // in ms
            /// <summary>
            /// 
            /// </summary>
            public float MotorStart;
            /// <summary>
            /// 
            /// </summary>
            public float MotorEnd;

            /// <summary>
            /// 
            /// </summary>
            private readonly float _durationInv;
            /// <summary>
            /// 
            /// </summary>
            private float _timeElapsed; // in ms

            /// <summary>
            /// 
            /// </summary>
            /// <param name="start"></param>
            /// <param name="end"></param>
            /// <param name="duration"></param>
            public Pulse(float start, float end, float duration)
            {
                this.MotorStart = start;
                this.MotorEnd = end;
                this.Duration = duration;
                this._durationInv = 1 / this.Duration;
                this._timeElapsed = 0;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public float Delta()
            {
                return Mathf.Min(1.0f, this._timeElapsed * this._durationInv);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="p"></param>
            /// <param name="delta"></param>
            /// <returns></returns>
            public static Pulse operator +(Pulse p, float delta)
            {
                p._timeElapsed += delta;
                return p;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private Pulse _left;
        /// <summary>
        /// 
        /// </summary>
        private Pulse _right;

        /// <summary>
        /// 
        /// </summary>
        public float OnHitStrength;
        /// <summary>
        /// 
        /// </summary>
        public float OnHitDuration;

        /// <summary>
        /// 
        /// </summary>
        public float StrengthOnArtilleryFire;
        /// <summary>
        /// 
        /// </summary>
        public float DurationOnArtilleryFire;
        
        /// <summary>
        /// 
        /// </summary>
        public float StrengthOnRam;
        /// <summary>
        /// 
        /// </summary>
        public float DurationOnRam;

        /// <inheritdoc />
        private void Start()
        {
            this._left = null;
            this._right = null;
        }

        /// <inheritdoc />
        private void OnEnable()
        {
            GameManager.Events.EntityShipHitByProjectile += this.OnEntityHitByProjectile;
            GameManager.Events.EntityShipHitByRam += this.OnEntityShipHitByRam;
            GameManager.Events.ArtilleryFired += this.OnArtilleryFired;
        }

        /// <inheritdoc />
        private void OnDisable()
        {
            GameManager.Events.EntityShipHitByProjectile -= this.OnEntityHitByProjectile;
            GameManager.Events.EntityShipHitByRam -= this.OnEntityShipHitByRam;
            GameManager.Events.ArtilleryFired -= this.OnArtilleryFired;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        private static float Lerp(float start, float end, float delta)
        {
            return (1 - delta) * start + (delta) * end;
        }

        /// <inheritdoc />
        private void FixedUpdate()
        {
            float motorLeft = 0;
            float motorRight = 0;

            if (this._left != null)
            {
                motorLeft = Lerp(this._left.MotorStart, this._left.MotorEnd, this._left.Delta());

                this._left += Time.fixedDeltaTime;
                if (this._left.Delta() >= 1.0f)
                {
                    this._left = null;
                }
            }

            if (this._right != null)
            {
                motorRight = Lerp(this._right.MotorStart, this._right.MotorEnd, this._right.Delta());

                this._right += Time.fixedDeltaTime;
                if (this._right.Delta() >= 1.0f)
                {
                    this._right = null;
                }
            }

            GamePad.SetVibration(PlayerIndex.One, motorLeft, motorRight);
        }

        /// <inheritdoc />
        private void OnEntityHitByProjectile(GameEvent evt)
        {
            EventEntityShipHitByProjectile evtHit = (EventEntityShipHitByProjectile) evt;

            if (!(evtHit.Ship is EntityPlayerShip)) return;

            Transform target = evtHit.Ship.GetRender().transform;
            Transform source = evtHit.Projectile.transform;

            Vector3 targetToSource = source.position - target.position;

            // determines orthogonality of
            // target's right to the vector from target to source
            float orthogonality = Vector3.Dot(target.right, targetToSource);

            // if orthogonality is < 0, then right side
            // if orthogonality is > 0, then left side
            if (orthogonality < 0)
                this._right = new Pulse(this.OnHitStrength, 0, this.OnHitDuration);
            else if (orthogonality > 0)
                this._left = new Pulse(this.OnHitStrength, 0, this.OnHitDuration);

        }

        private void OnEntityShipHitByRam(GameEvent evt)
        {
            // TODO: Rename event for player only
            this._left = this._right = new Pulse(this.StrengthOnRam, 0, this.DurationOnRam);
        }

        private void OnArtilleryFired(GameEvent evt)
        {
            EventArtilleryFired evtFired = (EventArtilleryFired) evt;
            switch (evtFired.ComponentType)
            {
                case ShipData.ComponentType.ArtilleryRight:
                    this._right = new Pulse(this.StrengthOnArtilleryFire, 0, this.DurationOnArtilleryFire);
                    break;
                case ShipData.ComponentType.ArtilleryLeft:
                    this._left = new Pulse(this.StrengthOnArtilleryFire, 0, this.DurationOnArtilleryFire);
                    break;
                default:
                    break;
            }
        }

    }
}