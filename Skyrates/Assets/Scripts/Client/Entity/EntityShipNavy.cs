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
        
       // private Coroutine _shootAtTarget;

        protected override void Start()
        {
            base.Start();

            this._aiTarget = null;
            //this._shootAtTarget = null;
        }

        protected override void FixedUpdate()
        {
            this.SteeringData.HasTarget = this._aiTarget != null;
            this.SteeringData.Target = this.SteeringData.HasTarget ? this._aiTarget.Physics : this.Physics;
            base.FixedUpdate();
        }

        protected override void OnDestroy()
        {
        //    if (this._shootAtTarget != null)
        //    {
        //        StopCoroutine(this._shootAtTarget);
        //        this._shootAtTarget = null;
        //    }
            base.OnDestroy();
        }

        public void OnEnterAreaFind(TriggerArea area, Collider other)
        {
            if (this._aiTarget != null) return;

            EntityPlayerShip playerShip = other.GetComponent<EntityPlayerShip>();
            if (playerShip != null)
            {
                this._aiTarget = playerShip;
            }
        }

        public void OnExitAreaFind(TriggerArea area, Collider other)
        {
            if (this._aiTarget == null) return;

            EntityPlayerShip playerShip = other.GetComponent<EntityPlayerShip>();
            if (playerShip != null && playerShip.GetInstanceID() == this._aiTarget.GetInstanceID())
            {
                this._aiTarget = null;
            }
        }

        public void OnEnterAreaShoot(TriggerArea area, Collider other)
        {
            //if (this._aiTarget == null) return;

            //EntityPlayerShip playerShip = other.GetComponent<EntityPlayerShip>();
            //if (playerShip != null && playerShip.GetInstanceID() == this._aiTarget.GetInstanceID())
            //{
            //    this._shootAtTarget = StartCoroutine(this.ShootAtTarget());
            //}
        }

        public void OnExitAreaShoot(TriggerArea area, Collider other)
        {
            //if (this._aiTarget == null) return;

            //EntityPlayerShip playerShip = other.GetComponent<EntityPlayerShip>();
            //if (playerShip != null && playerShip.GetInstanceID() == this._aiTarget.GetInstanceID())
            //{
            //    if (this._shootAtTarget != null)
            //    {
            //        StopCoroutine(this._shootAtTarget);
            //        this._shootAtTarget = null;
            //    }
            //}
        }
        
        //IEnumerator ShootAtTarget()
        //{
        //    while (true)
        //    {
        //        float wait = 3;

        //        if (this._aiTarget != null)
        //        {
        //            this.Shoot(ShipData.ComponentType.ArtilleryForward); 
        //        }
        //        yield return new WaitForSeconds(wait);
        //    }
        //}

        protected override Shooter[] GetArtilleryShooters(ShipData.ComponentType artillery)
        {
            return this.Shooters;
        }

        // TODO: Fix the steering to remove this
        protected override void IntegratePhysics(float deltaTime)
        {
            //Quaternion rotation = this.Physics.RotationPosition;
            base.IntegratePhysics(deltaTime);
            //this.transform.rotation = rotation;
        }

    }
}
