using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Common.AI
{

    public class PhysicsData
    {

        [BitSerialize(0)]
        public Vector3 LinearPosition;

        [BitSerialize(1)]
        public Vector3 LinearVelocity;

        [BitSerialize(2)]
        public Vector3 LinearAccelleration;

        [BitSerialize(3)]
        public Quaternion RotationPosition;

        [BitSerialize(4)]
        public Quaternion RotationVelocity;

        [BitSerialize(5)]
        public Quaternion RotationAccelleration;

    }

}
