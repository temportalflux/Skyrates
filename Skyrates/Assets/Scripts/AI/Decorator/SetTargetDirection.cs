using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.Formation
{

    /// <summary>
    /// Offsets the current target in some direction by some amount.
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Decorator/Set Target Direction")]
    public class SetTargetDirection : Behavior
    {

        public Vector3 OffsetWorld = Vector3.zero;

        public Vector3 OffsetTargetLocal = Vector3.zero;

        public Vector3 OffsetSourceLocal = Vector3.zero;

        /// <inheritdoc />
        public override DataPersistent GetUpdate(ref PhysicsData physics, ref DataBehavioral data, DataPersistent persistent, float deltaTime)
        {
            Vector3 offset_world = this.OffsetWorld;
            
            // Scale each transform direction (forward, right, up) by each axis of the local scale (z, x, y)
            Vector3 offset_local_target =
                data.Target.Forward * this.OffsetTargetLocal.z
                + data.Target.Right * this.OffsetTargetLocal.x
                + data.Target.Up * this.OffsetTargetLocal.y;
            Vector3 offset_local_source =
                physics.Forward * this.OffsetSourceLocal.z
                + physics.Right * this.OffsetSourceLocal.x
                + physics.Up * this.OffsetSourceLocal.y;

            data.Target.LinearPosition += offset_world + offset_local_target + offset_local_source;

            return persistent;
        }

    }

}