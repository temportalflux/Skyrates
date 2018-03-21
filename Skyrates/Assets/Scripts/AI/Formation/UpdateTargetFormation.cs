using Skyrates.Common.AI;
using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.Formation
{

    /// <summary>
    /// Sets the target according to the location slot set in the owners behavior and what its formation owner is.
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Formation/Update Target")]
    public class UpdateTargetFormation : Behavior
    {

        /// <inheritdoc />
        public override DataPersistent GetUpdate(ref PhysicsData physics, ref DataBehavioral data, DataPersistent persistent, float deltaTime)
        {
            // Get the physics data of the slot the agent is assigned to
            if (data.Formation != null && data.Formation.HasFormation)
                data.Target = data.Formation.GetTarget();
            return persistent;
        }

    }

}
