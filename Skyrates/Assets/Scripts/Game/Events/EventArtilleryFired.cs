using Skyrates.Entity;
using Skyrates.Mono;
using Skyrates.Ship;
using UnityEngine;

namespace Skyrates.Game.Event
{

    /// <summary>
    /// Event dispatched when artillery is fired
    /// </summary>
    public class EventArtilleryFired : EventEntity
    {

        // TODO: Rename Shooter to Artillery? (will there be non-artillery shooters like guns?)
        /// <summary>
        /// The artillery fired.
        /// </summary>
        public ShipArtillery[] Shooters;

        public ShipData.ComponentType ComponentType;

        public EventArtilleryFired(EntityShip ship, ShipArtillery[] shooters, ShipData.ComponentType type)
            : base(GameEventID.ArtilleryFired, ship)
        {
            this.Shooters = shooters;
            this.ComponentType = type;
        }

        /// <summary>
        /// Returns the average transform data for all the shooters/artillery
        /// </summary>
        /// <param name="averagePosition"></param>
        /// <param name="averageRotation"></param>
        /// <returns></returns>
        public int GetAverageTransform(out Vector3 averagePosition, out Quaternion averageRotation)
        {
            averagePosition = Vector3.zero;
            averageRotation = Quaternion.identity;

            int artilleryCount = this.Shooters.Length;
            if (artilleryCount <= 0) return 0;

            // Position average is easy
            // Quaternion average taken from https://answers.unity.com/questions/815266/find-and-average-rotations-together.html
            foreach (ShipArtillery shooter in this.Shooters)
            {
                averagePosition += shooter.Shooter.spawn.position;
                averageRotation = Quaternion.Slerp(averageRotation, shooter.Shooter.spawn.rotation, 1 / artilleryCount);
            }
            averagePosition /= artilleryCount;

            return artilleryCount;
        }

    }

}
