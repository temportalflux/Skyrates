using System;
using Skyrates.Util.Extension;
using UnityEngine;

namespace Skyrates.Physics
{

    [Serializable]
    public class PhysicsData
    {

        public Vector3 Forward = Vector3.zero;
        public Vector3 Right = Vector3.zero;
        public Vector3 Up = Vector3.zero;


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

        public static PhysicsData From(Transform transform)
        {
            return new PhysicsData()
            {
                LinearPosition = transform.position,
                RotationPosition = transform.rotation,
                Forward = transform.forward,
                Right = transform.right,
                Up = transform.up,
            };
        }

        public PhysicsData Copy()
        {
            PhysicsData newInst = new PhysicsData();
            newInst.CopyFrom(this);
            return newInst;
        }

        public void CopyFrom(PhysicsData other)
        {
            this.Forward = other.Forward;
            this.Right = other.Right;
            this.Up = other.Up;
            this.LinearPosition = other.LinearPosition;
            this.LinearVelocity = other.LinearVelocity;
            this.LinearAccelleration = other.LinearAccelleration;
            this.RotationPosition = other.RotationPosition;
            this.RotationVelocity = other.RotationVelocity;
            this.RotationAccelleration = other.RotationAccelleration;
            this.RotationAesteticPosition = other.RotationAesteticPosition;
            this.RotationAesteticVelocity = other.RotationAesteticVelocity;
        }

        public static PhysicsData operator *(PhysicsData data, float weight)
        {
            return new PhysicsData
            {
                Forward = data.Forward,
                Right = data.Right,
                Up = data.Up,
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
                Forward = a.Forward,
                Right = a.Right,
                Up = a.Up,
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

        public void UpdatePositions(Transform t)
        {
            this.LinearPosition = t.position;
            this.RotationPosition = t.rotation;
        }

        public void UpdateDirections(Transform t)
        {
            this.Forward = t.forward;
            this.Right = t.right;
            this.Up = t.up;
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

#if UNITY_EDITOR
        public void DrawGizmos(float axisScale, float sphereScale, Color sphereColor)
        {
            if (Mathf.Abs(axisScale) > 0.0f)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(this.LinearPosition, this.Forward * axisScale);
                Gizmos.color = Color.red;
                Gizmos.DrawRay(this.LinearPosition, this.Right * axisScale);
                Gizmos.color = Color.green;
                Gizmos.DrawRay(this.LinearPosition, this.Up * axisScale);
            }
            if (Mathf.Abs(sphereScale) > 0.0f)
            {
                Gizmos.color = sphereColor;
                Gizmos.DrawWireSphere(this.LinearPosition, sphereScale);
            }
        }
#endif

    }

}
