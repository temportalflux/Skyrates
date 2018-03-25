using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.Decorator
{

    /// <summary>
    /// Offsets the current target in some direction by some amount.
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Decorator/Target Nearest Nearby")]
    public class TargetNearestNearby : Behavior
    {

        /// <inheritdoc />
        public override DataPersistent GetUpdate(ref PhysicsData physics, ref DataBehavioral data, DataPersistent persistent, float deltaTime)
        {
            PhysicsData nearest = physics;

            float smallestDistSq = Mathf.Infinity;
            foreach (DataBehavioral.NearbyTarget target in data.NearbyTargets)
            {
                if (target.MaxDistanceSq < smallestDistSq)
                {
                    nearest = target.Target;
                    smallestDistSq = target.MaxDistanceSq;
                }
            }

            data.Target = (nearest).Copy();
            return persistent;
        }

    }

}