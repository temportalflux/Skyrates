using UnityEngine;

namespace Skyrates.Common.AI
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
        public override object GetUpdate(ref BehaviorData data, ref PhysicsData physics, float deltaTime, object pData)
        {
            // Work out the distance to the target
            Vector3 direction = data.Target.LinearPosition - physics.LinearPosition;
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
            data.Target.LinearPosition += data.Target.LinearVelocity * prediction;

            base.GetUpdate(ref data,ref  physics, deltaTime);
            return pData;
        }

    }

}
