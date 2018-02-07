
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

            physics.LinearAccelleration = data.Target.LinearPosition - physics.LinearPosition;
            physics.LinearAccelleration.Normalize();
            physics.LinearAccelleration *= this.AccellerationMax;

            physics.RotationAccelleration = Quaternion.identity;

        }

    }

}
