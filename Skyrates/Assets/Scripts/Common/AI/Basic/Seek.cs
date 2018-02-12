
using Skyrates.Common.Entity;
using UnityEngine;

namespace Skyrates.Common.AI
{

    /// <summary>
    /// Full force steering - just go for it.
    /// Derived from
    /// Artifical Intelligence for Games 2nd Editin
    /// Ian Millington & John Funge
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Basic/Seek")]
    public class Seek : Steering
    {

        public float AccellerationMax;

        /// <inheritdoc />
        /// https://gamedev.stackexchange.com/questions/121469/unity3d-smooth-rotation-for-seek-steering-behavior
        public override void GetSteering(SteeringData data, ref PhysicsData physics)
        {

            Vector3 direction = (data.Target.LinearPosition - physics.LinearPosition).normalized;
            
            //physics.LinearAccelleration = direction * this.AccellerationMax;
            physics.LinearVelocity = direction * this.AccellerationMax;

            // Face
            Vector3 directionXZ = new Vector3(data.Target.LinearPosition.x, physics.LinearPosition.y, data.Target.LinearPosition.z) - physics.LinearPosition;
            if (directionXZ != Vector3.zero)
            {
                Quaternion towardsTarget = Quaternion.LookRotation(directionXZ);
                physics.RotationPosition = towardsTarget;
            }

        }

    }

}
