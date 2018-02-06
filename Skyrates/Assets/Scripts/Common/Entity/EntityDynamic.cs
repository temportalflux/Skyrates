using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Common.Entity
{

    public class EntityDynamic : Entity
    {

        [BitSerialize(1)]
        public PhysicsData Physics;

        /// <inheritdoc />
        public override void IntegratePhysics()
        {
            this.transform.SetPositionAndRotation(
                this.Physics.PositionLinear,
                Quaternion.Euler(this.Physics.VelocityRotationEuler)
            );
        }

    }

}
