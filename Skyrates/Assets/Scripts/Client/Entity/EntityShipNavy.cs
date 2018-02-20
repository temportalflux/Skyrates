using System.Collections;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Client.Mono;
using Skyrates.Common.Entity;
using UnityEngine;

namespace Skyrates.Client.Entity
{

    /// <summary>
    /// Special case of entity ship which is completely controlled by AI.
    /// </summary>
    public class EntityShipNavy : EntityShip
    {

        // TODO: Revamp this class with better AI

        public Shooter[] Shooters;

        private EntityShip _aiTarget;

        private Coroutine _findTarget;
        private Coroutine _shootAtTarget;
        public float maxDistShoot = 10000;
        public float maxDistFind = 10000;

        protected override void Start()
        {
            base.Start();
            this._aiTarget = null;
            this._findTarget = null;
            this._shootAtTarget = StartCoroutine(this.ShootAtTarget());
        }

        protected override void FixedUpdate()
        {
            if (this._aiTarget == null)
            {
                if (this._findTarget == null)
                    this._findTarget = StartCoroutine(this.FindTarget());
            }
            else if ((this._aiTarget.transform.position - this.transform.position).sqrMagnitude > this.maxDistFind)
            {
                GameManager.Events.Dispatch(EventEnemyTargetEngage.Disengage(this, this._aiTarget));
                this._aiTarget = null;
            }
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
                if (this._aiTarget != null)
                {
                    if ((this._aiTarget.transform.position - this.transform.position).sqrMagnitude > this.maxDistFind)
                    {
                        this._aiTarget = null;
                    }
                    else
                    {
                        //Debug.Log(this._aiTarget);
                    }
                }
            }
            
            GameManager.Events.Dispatch(EventEnemyTargetEngage.Engage(this, this._aiTarget));

            this._findTarget = null;
        }

        IEnumerator ShootAtTarget()
        {
            while (true)
            {
                float wait = this._aiTarget == null ? 15 : 3;

                float sqrDist = 200;
                if (this._aiTarget != null)
                {
                    sqrDist = (this.transform.position - this._aiTarget.transform.position).sqrMagnitude;
                    if (sqrDist > maxDistShoot)
                        wait = 0.5f;

                    if (sqrDist < maxDistShoot)
                    {
                        this.Shoot(ShipData.ComponentType.ArtilleryForward);
                    }
                }
                yield return new WaitForSeconds(wait);
            }
        }

        protected override Shooter[] GetArtilleryShooters(ShipData.ComponentType artillery)
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
