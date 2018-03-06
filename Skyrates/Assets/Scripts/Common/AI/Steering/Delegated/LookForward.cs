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
    [CreateAssetMenu(menuName = "Data/AI/Delegated/LookForward")]
    public class LookForward : Align
    {

        /// <inheritdoc />
        public override object GetUpdate(ref BehaviorData data, ref PhysicsData physics, float deltaTime, object pData)
        {
            // Check for a zero direction, and make no change if so
            if (physics.LinearVelocity.sqrMagnitude <= 0 || physics.LinearVelocity == Vector3.zero) return physics;

            // Put the target together
            data.Target.RotationPosition = Quaternion.LookRotation(physics.LinearVelocity);

            // Delegate to align
            return base.GetUpdate(ref data, ref physics, deltaTime, pData);
        }

    }

}
