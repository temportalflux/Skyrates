using Skyrates.AI;
using UnityEngine;

namespace Skyrates.Common.AI
{

    /// <summary>
    /// Full force steering - just go for it.
    /// 
    /// Derived from pg 57 of
    /// Artifical Intelligence for Games 2nd Edition
    /// Ian Millington & John Funge
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Basic/Seek")]
    public class Seek : Steering
    {

        /// <summary>
        /// Acceleration coefficient in the target direction.
        /// </summary>
        public float MaxAcceleration;

        /// <summary>
        /// Multiplies the target direction (which is then normalized).
        /// This value will have no effect regardless of nominal value.
        /// However, setting this to a negative (-1) will turn seek into flee.
        /// </summary>
        public float SeekTargetMultiplier = 1;

        /// <inheritdoc />
        /// https://gamedev.stackexchange.com/questions/121469/unity3d-smooth-rotation-for-seek-steering-behavior
        public override object GetUpdate(ref BehaviorData data, ref PhysicsData physics, float deltaTime, object pData)
        {
            // Get direction from this unit to the target
            physics.LinearAccelleration = (data.Target.LinearPosition - physics.LinearPosition) * this.SeekTargetMultiplier;
            physics.LinearAccelleration.Normalize();
            // Set velocity to a scalar in the desired direction
            physics.LinearAccelleration *= this.MaxAcceleration;
            return pData;
        }

    }

}
