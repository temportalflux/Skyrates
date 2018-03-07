using Skyrates.Common.Network;
using System;
using UnityEngine;

namespace Skyrates.Common.AI
{

    [Serializable]
    public class PhysicsData
    {

        [Serializable]
        public class Integrals
        {

            [SerializeField]
            public Vector3 Velocity = Vector3.zero;

            [SerializeField]
            public Vector3 Acceleration = Vector3.zero;

            public virtual void Integrate(float deltaTime)
            {
                ExtensionMethods.Integrate(ref this.Velocity, this.Acceleration, deltaTime);
            }

        }

        [Serializable]
        public class LinearIntegrals : Integrals
        {

            [SerializeField]
            public Vector3 Position = Vector3.zero;

            public override void Integrate(float deltaTime)
            {
                base.Integrate(deltaTime);
                ExtensionMethods.Integrate(ref this.Position, this.Velocity, deltaTime);
            }

        }

        [Serializable]
        public class RotationalIntegrals : Integrals
        {

            [SerializeField]
            public Quaternion Position = Quaternion.identity;

            public override void Integrate(float deltaTime)
            {
                base.Integrate(deltaTime);
                ExtensionMethods.Integrate(ref this.Position, this.Velocity, deltaTime);
            }

        }

        [SerializeField]
        public LinearIntegrals Linear = new LinearIntegrals();

        [SerializeField]
        public RotationalIntegrals Rotation = new RotationalIntegrals();

        [SerializeField]
        public RotationalIntegrals RotationAestetic = new RotationalIntegrals();
        
        public Vector3 LinearPosition
        {
            get { return this.Linear.Position; }
            set { this.Linear.Position = value; }
        }
        
        public Vector3 LinearVelocity
        {
            get { return this.Linear.Velocity; }
            set { this.Linear.Velocity = value; }
        }
        
        public Vector3 LinearAccelleration
        {
            get { return this.Linear.Acceleration; }
            set { this.Linear.Acceleration = value; }
        }
        
        public Quaternion RotationPosition
        {
            get { return this.Rotation.Position; }
            set { this.Rotation.Position = value; }
        }

        public Vector3 RotationVelocity
        {
            get { return this.Rotation.Velocity; }
            set { this.Rotation.Velocity = value; }
        }

        public Vector3 RotationAccelleration
        {
            get { return this.Rotation.Velocity; }
            set { this.Rotation.Velocity = value; }
        }

        public bool HasAesteticRotation = false;

        public Quaternion RotationAesteticPosition
        {
            get { return this.RotationAestetic.Position; }
            set { this.RotationAestetic.Position = value; }
        }

        [SerializeField]
        public Vector3 RotationAesteticVelocity = Vector3.zero;

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
            };
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
            this.Linear.Integrate(deltaTime);
            this.Rotation.Integrate(deltaTime);
            this.RotationAestetic.Integrate(deltaTime);
        }
        
    }

}
