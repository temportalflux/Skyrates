using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Common.Entity
{

    public class EntityDynamic : Entity
    {

        [BitSerialize(2)]
        public PhysicsData Physics;

        /// <inheritdoc />
        public void IntegratePhysics()
        {
            // TODO: Use this after steering does its thing
            this.transform.SetPositionAndRotation(
                this.Physics.PositionLinear,
                Quaternion.Euler(this.Physics.VelocityRotationEuler)
            );
        }

    }

}
