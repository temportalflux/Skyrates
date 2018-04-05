using Skyrates.AI.Steering.Basic;
using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.Steering.Delegated
{

    /// <summary>
    /// Aligns the rotation to the current velocity.
    /// 
    /// Derived from pg 72 of
    /// Artifical Intelligence for Games 2nd Edition
    /// Ian Millington & John Funge
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Steering/Basic/Face")]
    public class Face : Align
    {

        /// <inheritdoc />
        public override DataPersistent GetUpdate(ref PhysicsData physics, ref DataBehavioral behavioral, DataPersistent persistent, float deltaTime)
        {
            // Check for a zero direction, and make no change if so
            if (physics.LinearVelocity.sqrMagnitude <= 0 || physics.LinearVelocity == Vector3.zero) return persistent;

            // Put the target together
            behavioral.Target.RotationPosition = Quaternion.LookRotation(physics.LinearVelocity);

            // Delegate to align
            return base.GetUpdate(ref physics, ref behavioral, persistent, deltaTime);
        }
        
    }

}
