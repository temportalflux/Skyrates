using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Common.AI
{

    [CreateAssetMenu(menuName = "Data/AI/Custom/Ship Rotation")]
    public class ShipRotation : Steering
    {

        public float AngleYaw;
        public float AngleSpeedYaw;
        public float AnglePitch;
        public float AngleSpeedPitch;

        public override object GetUpdate(ref BehaviorData data, ref PhysicsData physics,
            float deltaTime, object persistent)
        {
            physics.HasAesteticRotation = true;

            Vector3 movementVertical = Vector3.up * physics.LinearVelocity.y;
            float rotationY = physics.RotationVelocity.y;

            // banking
            float rotationZ = Mathf.Sign(-rotationY) * Mathf.Min(Mathf.Abs(rotationY), this.AngleYaw);

            // pitching (up/down rotation)
            float rotationX = -1 * (movementVertical.sqrMagnitude > 0 ? Mathf.Sign(movementVertical.y) : 0) * this.AnglePitch;

            Vector3 currentRotation = physics.RotationAesteticPosition.eulerAngles;

            LerpRotation(currentRotation.z, rotationZ,
                this.AngleSpeedYaw,
                ref physics.RotationAesteticVelocity.z
            );
            LerpRotation(currentRotation.x, rotationX,
                this.AngleSpeedPitch,
                ref physics.RotationAesteticVelocity.x
            );

            return persistent;
        }

        private void LerpRotation(float current, float target, float speed, ref float velocity)
        {
            float rotation = target - current;
            rotation = ((int)(Align.MapToRange(rotation) * 100)) * 0.01f;
            float rotationSize = Mathf.Abs(rotation);
            float dir = rotationSize > 0.0f ? rotation / rotationSize : 0.0f;
            velocity = speed * dir;
        }

    }

}
