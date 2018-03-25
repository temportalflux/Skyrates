using System.Collections.Generic;
using Skyrates.AI.Formation;
using Skyrates.Entity;
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

        /// <inheritdoc />
        public override DataPersistent GetUpdate(ref PhysicsData physics, ref DataBehavioral data, DataPersistent persistent, float deltaTime)
        {
            Vector3 directionFromNearbyToOwner = Vector3.zero;
            foreach (DataBehavioral.NearbyTarget target in data.NearbyFormationTargets)
            {
                Vector3 targetLocation = target.Target.LinearPosition;
                Vector3 diff = physics.LinearPosition - targetLocation;
                directionFromNearbyToOwner += diff;
            }
            directionFromNearbyToOwner.Normalize();

            directionFromNearbyToOwner *= this.OffsetFromNearby;

            data.Target.LinearPosition += directionFromNearbyToOwner;

            return persistent;
        }

    }

}