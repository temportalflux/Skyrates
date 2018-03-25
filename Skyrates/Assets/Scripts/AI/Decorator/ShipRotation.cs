using Skyrates.AI.Steering.Basic;
using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.Custom
{

    /// <summary>
    /// Adds aestetic rotation according to the present movement of the ship (rotation on X and Z axes).
    /// Specifically, rotates around X axis (pitch) when moving up/down (along Z axis), and rotates around X axis (yaw/banking) when turning (along Z axis). 
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Decorator/Ship Rotation")]
    public class ShipRotation : Steering.Steering
    {

        /// <summary>
        /// The maximum absolute-value angle of banking.
        /// </summary>
        public float AngleYaw;

        /// <summary>
        /// The speed at which <see cref="AngleYaw"/> is achieved.
        /// </summary>
        public float AngleSpeedYaw;

        /// <summary>
        /// The maximum absolute-value angle when moving vertically.
        /// </summary>
        public float AnglePitch;

        /// <summary>
        /// The speed at which <see cref="AnglePitch"/> is achieved.
        /// </summary>
        public float AngleSpeedPitch;

        /// <inheritdoc />
        public override DataPersistent GetUpdate(ref PhysicsData physics, ref DataBehavioral behavioral, DataPersistent persistent, float deltaTime)
        {
            physics.HasAesteticRotation = true;

            float rotationY = physics.RotationVelocity.y;

            // banking
            float rotationZ = Mathf.Sign(-rotationY) * Mathf.Min(Mathf.Abs(rotationY), this.AngleYaw);

            // pitching (up/down rotation)
            float rotationX = -1 *
                (physics.LinearVelocity.y > 0 ? Mathf.Sign(physics.LinearVelocity.y) : 0)
                * this.AnglePitch;

            Vector3 currentRotation = physics.RotationAesteticPosition.eulerAngles;

            // HORIZONTAL ROTATION - BANKING
            // Lerp from the current rotation to the target rotation via some speed and output as velocity
            LerpRotation(currentRotation.x, rotationX,
                this.AngleSpeedPitch,
                out physics.RotationAesteticVelocity.x
            );

            // VERTICAL ROTATION
            // Lerp from the current rotation to the target rotation via some speed and output as velocity
            LerpRotation(currentRotation.z, rotationZ,
                this.AngleSpeedYaw,
                out physics.RotationAesteticVelocity.z
            );

            return persistent;
        }

        /// <summary>
        /// Essentially the align behavior
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="speed"></param>
        /// <param name="velocity"></param>
        private void LerpRotation(float current, float target, float speed, out float velocity)
        {
            float rotation = target - current;
            rotation = ((int)(Align.MapToRange(rotation) * 100)) * 0.01f;
            float rotationSize = Mathf.Abs(rotation);
            float dir = rotationSize > 0.0f ? rotation / rotationSize : 0.0f;
            velocity = speed * dir;
        }

    }

}
