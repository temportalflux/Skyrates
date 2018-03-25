using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.Decorator
{

    /// <summary>
    /// Offsets the current target in some direction by some amount.
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Decorator/Offset From Entity")]
    public class FormationOffsetFromEntity : Behavior
    {
        
        public float OffsetFromNearby = 0.0f;

        public bool ScaleByDistance = false;

        /// <inheritdoc />
        public override DataPersistent GetUpdate(ref PhysicsData physics, ref DataBehavioral data, DataPersistent persistent, float deltaTime)
        {
            Vector3 directionFromNearbyToOwner = Vector3.zero;
            foreach (DataBehavioral.NearbyTarget target in data.Formation.GetNearbyTargets())
            {
                Vector3 targetLocation = target.Target.LinearPosition;
                Vector3 diff = physics.LinearPosition - targetLocation;

                if (this.ScaleByDistance)
                {
                    float scale = 1.0f - Mathf.Min(diff.sqrMagnitude, target.MaxDistanceSq) / target.MaxDistanceSq;
                    diff.Normalize();
                    diff *= scale;
                }

                directionFromNearbyToOwner += diff;
            }

            if (!this.ScaleByDistance)
            {
                directionFromNearbyToOwner.Normalize();
            }

            directionFromNearbyToOwner *= this.OffsetFromNearby;

            data.Target.LinearPosition += directionFromNearbyToOwner;

            return persistent;
        }

    }

}