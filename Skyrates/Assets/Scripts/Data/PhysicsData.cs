using System;
using Skyrates.Util.Extension;
using UnityEngine;

namespace Skyrates.Physics
{

    [Serializable]
    public class PhysicsData
    {

        public Vector3 LinearPosition = Vector3.zero;
        
        public Vector3 LinearVelocity = Vector3.zero;
        
        public Vector3 LinearAccelleration = Vector3.zero;
        
        public Quaternion RotationPosition = Quaternion.identity;
        
        public Vector3 RotationVelocity = Vector3.zero;
        
        public Vector3 RotationAccelleration = Vector3.zero;

        public bool HasAesteticRotation = false;
        
        public Quaternion RotationAesteticPosition = Quaternion.identity;
        
        public Vector3 RotationAesteticVelocity = Vector3.zero;

        public Quaternion RotationPositionComposite = Quaternion.identity;

        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            this.LinearPosition = position;
            this.RotationPosition = rotation;
        }

        public PhysicsData Copy()
        {
            return new PhysicsData
            {
                LinearPosition = LinearPosition,
                LinearVelocity = LinearVelocity,
                LinearAccelleration = LinearAccelleration,
                RotationPosition = RotationPosition,
                RotationVelocity = RotationVelocity,
                RotationAccelleration = RotationAccelleration,
                RotationAesteticPosition = RotationAesteticPosition,
                RotationAesteticVelocity = RotationAesteticVelocity,
            };
        }

        public static PhysicsData operator *(PhysicsData data, float weight)
        {
            return new PhysicsData
            {
                LinearPosition = data.LinearPosition,
                LinearVelocity = data.LinearVelocity * weight,
                LinearAccelleration = data.LinearAccelleration * weight,
                RotationPosition = data.RotationPosition,
                RotationVelocity = data.RotationVelocity * weight,
                RotationAccelleration = data.RotationAccelleration * weight,
                RotationAesteticPosition = data.RotationAesteticPosition,
                RotationAesteticVelocity = data.RotationAesteticVelocity * weight,
            };
        }

        public static PhysicsData operator +(PhysicsData a, PhysicsData b)
        {
            return new PhysicsData
            {
                LinearPosition = a.LinearPosition,
                LinearVelocity = a.LinearVelocity + b.LinearVelocity,
                LinearAccelleration = a.LinearAccelleration + b.LinearAccelleration,
                RotationPosition = a.RotationPosition,
                RotationVelocity = a.RotationVelocity + b.RotationVelocity,
                RotationAccelleration = a.RotationAccelleration + b.RotationAccelleration,
                RotationAesteticPosition = a.RotationAesteticPosition,
                RotationAesteticVelocity = a.RotationAesteticVelocity + b.RotationAesteticVelocity,
            };
        }

        public void Integrate(float deltaTime)
        {
            // Update linear velocity
            ExtensionMethods.Integrate(ref this.LinearVelocity, this.LinearAccelleration, deltaTime);

            // Update linear position
            ExtensionMethods.Integrate(ref this.LinearPosition, this.LinearVelocity, deltaTime);

            // Update rotational velocity
            ExtensionMethods.Integrate(ref this.RotationVelocity, this.RotationAccelleration, deltaTime);

            // Update rotational position
            ExtensionMethods.Integrate(ref this.RotationPosition, this.RotationVelocity, deltaTime);

            // Update rotational position
            ExtensionMethods.Integrate(ref this.RotationAesteticPosition, this.RotationAesteticVelocity, deltaTime);

            // Update the composite rotation of aestetic and actual
            this.RotationPositionComposite = this.RotationPosition;
            ExtensionMethods.Integrate(ref this.RotationPositionComposite, this.RotationAesteticVelocity, deltaTime);

        }

    }

}
