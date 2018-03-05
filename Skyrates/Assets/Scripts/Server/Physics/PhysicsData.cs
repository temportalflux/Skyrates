using Skyrates.Common.Network;
using System;
using UnityEngine;

namespace Skyrates.Common.AI
{

    [Serializable]
    public class PhysicsData
    {

        [BitSerialize(0)]
        [SerializeField]
        public Vector3 LinearPosition = Vector3.zero;

        [BitSerialize(1)]
        [SerializeField]
        public Vector3 LinearVelocity = Vector3.zero;

        [BitSerialize(2)]
        [SerializeField]
        public Vector3 LinearAccelleration = Vector3.zero;

        [BitSerialize(3)]
        [SerializeField]
        public Quaternion RotationPosition = Quaternion.identity;

        [BitSerialize(4)]
        [SerializeField]
        public Vector3 RotationVelocity = Vector3.zero;

        [BitSerialize(5)]
        [SerializeField]
        public Vector3 RotationAccelleration = Vector3.zero;

        public bool HasAesteticRotation = false;

        [SerializeField]
        public Quaternion RotationAesteticPosition = Quaternion.identity;

        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            this.LinearPosition = position;
            this.RotationPosition = rotation;
        }

        public static PhysicsData operator*(PhysicsData data, float weight)
        {
            return new PhysicsData
            {
                LinearPosition = data.LinearPosition,
                LinearVelocity = data.LinearVelocity * weight,
                LinearAccelleration = data.LinearAccelleration * weight,
                RotationPosition = data.RotationPosition,
                RotationVelocity = data.RotationVelocity * weight,
                RotationAccelleration = data.RotationAccelleration * weight,
            };
        }

        public static PhysicsData operator+(PhysicsData a, PhysicsData b)
        {
            return new PhysicsData
            {
                LinearPosition = a.LinearPosition,
                LinearVelocity = a.LinearVelocity + b.LinearVelocity,
                LinearAccelleration = a.LinearAccelleration + b.LinearAccelleration,
                RotationPosition = a.RotationPosition,
                RotationVelocity = a.RotationVelocity + b.RotationVelocity,
                RotationAccelleration = a.RotationAccelleration + b.RotationAccelleration,
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

        }
        
    }

}
