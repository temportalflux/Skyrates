using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Common.AI
{

    public class PhysicsData
    {

        [BitSerialize(0)]
        public Vector3 LinearPosition = Vector3.zero;

        [BitSerialize(1)]
        public Vector3 LinearVelocity = Vector3.zero;

        [BitSerialize(2)]
        public Vector3 LinearAccelleration = Vector3.zero;

        [BitSerialize(3)]
        public Quaternion RotationPosition = Quaternion.identity;

        [BitSerialize(4)]
        public Quaternion RotationVelocity = Quaternion.identity;

        [BitSerialize(5)]
        public Quaternion RotationAccelleration = Quaternion.identity;

        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            this.LinearPosition = position;
            this.RotationPosition = rotation;
        }

    }

}
