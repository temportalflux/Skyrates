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
        
        IEnumerator ShootAtTarget()
        {
            while (true)
            {
                float wait = 3;

                //if (this._aiTarget != null)
                //{
                //    this.Shoot(ShipData.ComponentType.ArtilleryForward); 
                //}
                yield return new WaitForSeconds(wait);
            }
        }

        protected override Shooter[] GetArtilleryShooters(ShipData.ComponentType artillery)
        {
            return null;//this.Shooters;
        }

    }
}
