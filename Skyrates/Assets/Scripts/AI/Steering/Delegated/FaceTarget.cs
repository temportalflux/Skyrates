using Skyrates.AI.Steering.Basic;
using Skyrates.Common.AI;
using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.Steering.Delegated
{

    /// <summary>
    /// Aligns the rotation to direction of the target.
    /// 
    /// Derived from pg 71 of
    /// Artifical Intelligence for Games 2nd Edition
    /// Ian Millington & John Funge
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Delegated/Face Target")]
    public class FaceTarget : Align
    {

        /// <inheritdoc />
        public override DataPersistent GetUpdate(ref PhysicsData physics, ref DataBehavioral behavioral, DataPersistent persistent, float deltaTime)
        {
            // Calculate the target to delegate to align

            // Work out direction to target
            Vector3 direction = behavioral.Target.LinearPosition - physics.LinearPosition;
            direction.y = 0;
            direction.Normalize();

            // Check for a zero direction, and make no change if so
            if (direction.sqrMagnitude <= 0)
                return persistent;

            // Put the target together
            behavioral.Target.RotationPosition = Quaternion.LookRotation(direction);

            // Delegate to align
            return base.GetUpdate(ref physics, ref behavioral, persistent, deltaTime);
        }

    }

}
