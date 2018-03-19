using Skyrates.AI.Steering.Basic;
using Skyrates.Common.AI;
using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.Steering.Delegated
{

    /// <summary>
    /// Alters the target's position to account for where they will be,
    /// then delegates steering to <see cref="Seek"/>.
    /// 
    /// Derived from 69 of
    /// Artifical Intelligence for Games 2nd Edition
    /// Ian Millington & John Funge
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Delegated/Pursue")]
    public class Pursue : Seek
    {

        /// <summary>
        /// The maximum prediction time.
        /// </summary>
        public float MaxPredication;

        /// <inheritdoc />
        /// https://gamedev.stackexchange.com/questions/121469/unity3d-smooth-rotation-for-seek-steering-behavior
        public override DataPersistent GetUpdate(ref PhysicsData physics, ref DataBehavioral behavioral, DataPersistent persistent, float deltaTime)
        {
            // Work out the distance to the target
            Vector3 direction = behavioral.Target.LinearPosition - physics.LinearPosition;
            float distance = direction.magnitude;

            // Work out our current speed
            float speed = physics.LinearVelocity.magnitude;

            float prediction;
            // Check if speed is too small to give a reasonable prediction time
            if (speed < distance / this.MaxPredication)
            {
                prediction = this.MaxPredication;
            }
            // Otherwise calculate the prediction time
            else
            {
                prediction = distance / speed;
            }

            // Put the target together
            behavioral.Target.LinearPosition += behavioral.Target.LinearVelocity * prediction;

            return base.GetUpdate(ref physics, ref behavioral, persistent, deltaTime);
        }

    }

}
