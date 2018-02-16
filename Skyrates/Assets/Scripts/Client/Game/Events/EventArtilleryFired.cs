using Skyrates.Client.Ship;
using Skyrates.Common.Entity;
using UnityEngine;

namespace Skyrates.Client.Game.Event
{

    public class EventArtilleryFired : EventEntity
    {

        public Shooter[] Shooters;

        public EventArtilleryFired(EntityShip ship, Shooter[] shooters)
            : base(GameEventID.ArtilleryFired, ship)
        {
            this.Shooters = shooters;
        }

        public int GetAverageTransform(out Vector3 averagePosition, out Quaternion averageRotation)
        {
            averagePosition = Vector3.zero;
            averageRotation = Quaternion.identity;

            int artilleryCount = this.Shooters.Length;
            if (artilleryCount <= 0) return 0;

            // Position average is easy
            // Quaternion average taken from https://answers.unity.com/questions/815266/find-and-average-rotations-together.html
            foreach (Shooter shooter in this.Shooters)
            {
                averagePosition += shooter.spawn.position;
                averageRotation = Quaternion.Slerp(averageRotation, shooter.spawn.rotation, 1 / artilleryCount);
            }
            averagePosition /= artilleryCount;

            return artilleryCount;
        }

    }

}
