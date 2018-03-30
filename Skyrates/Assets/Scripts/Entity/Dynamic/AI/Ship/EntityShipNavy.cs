using System.Collections;
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
        public float ShootDelayBroadside = 2.0f;
        public float distToBroadside = 60.0f;


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

                float delay;
                if (direction.sqrMagnitude > distToBroadside * distToBroadside)
                {
                    artilleryDir += target.PhysicsData.LinearVelocity * 1.5f;
                    artilleryDir -= this.PhysicsData.LinearVelocity * 1.5f;
                    delay = this.ShootDelay;
                }
                else
                {
                    delay = this.ShootDelayBroadside;
                }
                this.Shoot(ShipData.ComponentType.ArtilleryForward, artilleryDir);

                yield return new WaitForSeconds(delay);

                direction = (target.transform.position - this.transform.position);
            }
            this._shootAt = null;
        }

    }
}
