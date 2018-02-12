using System.Collections;
using Skyrates.Client.Ship;
using Skyrates.Common.Entity;
using Skyrates.Client.Loot;
using UnityEngine;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Common.Network;

namespace Skyrates.Client.Entity
{
    public class EntityShipNavy : EntityShip
    {

        public Shooter[] Shooters;

        private EntityDynamic _aiTarget;

        private Coroutine _findTarget;
        private Coroutine _shootAtTarget;

        protected override void Start()
        {
            base.Start();
            this._aiTarget = null;
            this._findTarget = null;
            this._shootAtTarget = StartCoroutine(this.ShootAtTarget());
        }

        protected override void FixedUpdate()
        {
            if (this._findTarget == null && this._aiTarget == null)
                this._findTarget = StartCoroutine(this.FindTarget());
            this.SteeringData.Target = this._aiTarget == null ? this.Physics : this._aiTarget.Physics;
            base.FixedUpdate();
        }

        protected override void OnDestroy()
        {
            StopCoroutine(this._shootAtTarget);
            this._shootAtTarget = null;
            base.OnDestroy();
        }

        IEnumerator FindTarget()
        {
            while (this._aiTarget == null)
            {
                yield return new WaitForSeconds(5);
                this._aiTarget = FindObjectOfType<EntityPlayerShip>();
            }

            this._findTarget = null;
        }

        IEnumerator ShootAtTarget()
        {
            while (true)
            {
                float wait = this._aiTarget == null ? 15 : 3;

                float sqrDist = 200;
                float maxDist = 10000;
                if (this._aiTarget != null)
                {
                    sqrDist = (this.transform.position - this._aiTarget.transform.position).sqrMagnitude;
                    if (sqrDist > maxDist)
                        wait = 0.5f;
                }
                yield return new WaitForSeconds(wait);
                if (sqrDist < maxDist)
                {
                    this.Shoot();
                }
            }
        }

        protected override Shooter[] GetArtilleryShooters()
        {
            return this.Shooters;
        }

        // TODO: Fix the steering to remove this
        protected override void IntegratePhysics(float deltaTime)
        {
            Quaternion rotation = this.Physics.RotationPosition;
            base.IntegratePhysics(deltaTime);
            this.transform.rotation = rotation;
        }

    }
}
