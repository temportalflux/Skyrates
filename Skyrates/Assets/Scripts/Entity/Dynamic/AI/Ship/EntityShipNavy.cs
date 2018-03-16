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
    public class EntityShipNavy : EntityShipNPC
    {

        private Coroutine _shootAt;

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
                this.Shoot(ShipData.ComponentType.ArtilleryForward, direction + target.PhysicsData.LinearVelocity * 1.5f);
                yield return new WaitForSeconds(this.ShootDelay);
                direction = (target.transform.position - this.transform.position);
            }
            this._shootAt = null;
        }

    }
}
