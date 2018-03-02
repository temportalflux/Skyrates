using UnityEngine;

namespace Skyrates.Common.AI
{

    /// <summary>
    /// Aligns the rotation to direction of the target.
    /// 
    /// Derived from pg 71 of
    /// Artifical Intelligence for Games 2nd Edition
    /// Ian Millington & John Funge
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Delegated/LookAt")]
    public class LookAt : Align
    {

        /// <inheritdoc />
        public override object GetUpdate(ref BehaviorData data, ref PhysicsData physics, float deltaTime, object pData)
        {
            // Calculate the target to delegate to align

            // Work out direction to target
            Vector3 direction = data.Target.LinearPosition - physics.LinearPosition;
            direction.y = 0;
            direction.Normalize();

            // Check for a zero direction, and make no change if so
            if (direction.sqrMagnitude <= 0)
                return physics;

            // Put the target together
            data.Target.RotationPosition = Quaternion.LookRotation(direction);

            // Delegate to align
            base.GetUpdate(ref data, ref physics, deltaTime);

            return pData;
        }

    }

}
