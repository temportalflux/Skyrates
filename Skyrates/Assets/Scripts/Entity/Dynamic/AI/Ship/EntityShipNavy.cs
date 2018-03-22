﻿using System.Collections;
using Skyrates.Ship;
using UnityEngine;

namespace Skyrates.Entity
{

    /// <summary>
    /// Special case of entity ship which is completely controlled by AI.
    /// </summary>
    public class EntityShipNavy : EntityShipNPC
    {

        private Coroutine _shootAt;

        [Header("Hostile: Shooting")]
        public float ShootDelay = 7.0f;

        protected override void StartShooting(EntityPlayerShip target, float maxDistance)
        {
            if (this._shootAt == null)
            {
                this._shootAt = StartCoroutine(this.ShootAtTarget(target, maxDistance));
            }
        }

        IEnumerator ShootAtTarget(EntityPlayerShip target, float maxDistance)
        {
            float maxDistSq = maxDistance * maxDistance;
            Vector3 direction = (target.transform.position - this.transform.position);
            while (target && gameObject && direction.sqrMagnitude <= maxDistSq)
            {
                Vector3 artilleryDir = direction;
                artilleryDir += target.PhysicsData.LinearVelocity * 1.5f;
                artilleryDir -= this.PhysicsData.LinearVelocity * 1.5f;
                this.Shoot(ShipData.ComponentType.ArtilleryForward, artilleryDir);
                yield return new WaitForSeconds(this.ShootDelay);
                direction = (target.transform.position - this.transform.position);
            }
            this._shootAt = null;
        }

    }
}
