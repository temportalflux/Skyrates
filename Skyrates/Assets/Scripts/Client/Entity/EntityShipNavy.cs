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

        public CapsuleCollider rangeFind;
        public CapsuleCollider rangeShoot;

        private float maxDistFind;
        private float maxDistShoot;

        protected override void Start()
        {
            base.Start();

            this.maxDistFind = this.rangeFind != null ? this.rangeFind.radius : 0;
            this.maxDistFind *= this.maxDistFind;
            this.maxDistShoot = this.rangeShoot != null ? this.rangeShoot.radius : 1;
            this.maxDistShoot *= this.maxDistShoot;

            this._aiTarget = null;
            this._findTarget = null;
            this._shootAtTarget = null;
        }

        protected override void FixedUpdate()
        {
            if (this._aiTarget == null)
            {
                //if (this._findTarget == null && this.maxDistFind > 0)
                //    this._findTarget = StartCoroutine(this.FindTarget());
            }
            else if ((this._aiTarget.transform.position - this.transform.position).sqrMagnitude > this.maxDistFind)
            {
                //GameManager.Events.Dispatch(EventEnemyTargetEngage.Disengage(this, this._aiTarget));
                //this._aiTarget = null;
            }
            this.SteeringData.Target = this._aiTarget == null ? this.Physics : this._aiTarget.Physics;
            base.FixedUpdate();
        }

        protected override void OnDestroy()
        {
            if (this._shootAtTarget != null)
            {
                StopCoroutine(this._shootAtTarget);
                this._shootAtTarget = null;
            }
            base.OnDestroy();
        }

        public void OnTriggerEnter_AreaFind(Collider other)
        {
            if (this._aiTarget != null) return;

            EntityPlayerShip playerShip = other.GetComponent<EntityPlayerShip>();
            if (playerShip != null)
            {
                this._aiTarget = playerShip;
            }
        }

        public void OnTriggerExit_AreaFind(Collider other)
        {
            if (this._aiTarget == null) return;

            EntityPlayerShip playerShip = other.GetComponent<EntityPlayerShip>();
            if (playerShip != null && playerShip == this._aiTarget)
            {
                this._aiTarget = null;
            }
        }

        public void OnTriggerEnter_AreaShoot(Collider other)
        {
            if (this._aiTarget == null) return;

            EntityPlayerShip playerShip = other.GetComponent<EntityPlayerShip>();
            if (playerShip != null && playerShip == this._aiTarget)
            {
                this._shootAtTarget = StartCoroutine(this.ShootAtTarget());
            }
        }

        public void OnTriggerExit_AreaShoot(Collider other)
        {
            if (this._aiTarget == null) return;

            EntityPlayerShip playerShip = other.GetComponent<EntityPlayerShip>();
            if (playerShip != null && playerShip == this._aiTarget)
            {
                if (this._shootAtTarget != null)
                {
                    StopCoroutine(this._shootAtTarget);
                    this._shootAtTarget = null;
                }
            }
        }

        IEnumerator FindTarget()
        {
            if (this.maxDistFind <= 0) yield break;

            while (this._aiTarget == null)
            {
                yield return new WaitForSeconds(5);
                this._aiTarget = FindObjectOfType<EntityPlayerShip>();

                if (this._aiTarget == null) continue;

                float sqrDist = (this.transform.position - this._aiTarget.transform.position).sqrMagnitude;
                if (sqrDist > this.maxDistFind)
                {
                    this._aiTarget = null;
                }
                else
                {
                    //Debug.Log(this._aiTarget);
                }
            }
            
            GameManager.Events.Dispatch(EventEnemyTargetEngage.Engage(this, this._aiTarget));

            this._findTarget = null;
        }

        IEnumerator ShootAtTarget()
        {
            while (true)
            {
                float wait = 3;

                if (this._aiTarget != null)
                {
                    float sqrDist = (this.transform.position - this._aiTarget.transform.position).sqrMagnitude;
                    if (sqrDist > maxDistShoot)
                    {
                        wait = 0.5f;
                    }
                    else
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
