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
    public class EntityShipNPC : EntityShip
    {

        public Shooter[] Shooters;

        protected override Shooter[] GetArtilleryShooters(ShipData.ComponentType artillery)
        {
            return this.Shooters;
        }

        /// <summary>
        /// When we enter the radius of some other entity
        /// </summary>
        /// <param name="other"></param>
        /// <param name="maxDistance"></param>
        public override void OnEnterEntityRadius(EntityAI other, float maxDistance)
        {
            if (other is EntityPlayerShip)
            {
                // TODO: Move to a behavior
                this.StartShooting(other as EntityPlayerShip, maxDistance);
            }
        }

        protected virtual void StartShooting(EntityPlayerShip target, float maxDistance)
        {
            
        }

    }
}
