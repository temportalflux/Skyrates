using UnityEngine;

namespace Skyrates.Common.AI
{

    /// <summary>
    /// Aligns the rotation to the current velocity.
    /// 
    /// Derived from pg 71 of
    /// Artifical Intelligence for Games 2nd Edition
    /// Ian Millington & John Funge
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Delegated/Face")]
    public class Face : Align
    {

        /// <inheritdoc />
        public override void GetSteering(SteeringData data, ref PhysicsData physics)
        {
            // Calculate the target to delegate to align

            // Work out direction to target
            Vector3 direction = data.Target.LinearPosition - physics.LinearPosition;

            // Check for a zero direction, and make no change if so
            if (direction.sqrMagnitude <= 0) return;

            // Put the target together
            data.Target.RotationPosition = Quaternion.LookRotation(physics.LinearVelocity);
            
            // Delegate to align
            base.GetSteering(data, ref physics);
        }

    }

}
