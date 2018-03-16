using Skyrates.Common.AI;
using UnityEngine;

namespace Skyrates.AI.State.Transition
{

    /// <summary>
    /// Transitions if the target exists.
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Control/Transitions/Has Target")]
    public class TransitionTargetExists : StateTransition
    {

        /// <inheritdoc />
        public override bool CanEnter(Behavior.DataBehavioral behavioralData, PhysicsData physics, ref Behavior.DataPersistent persistent)
        {
            return behavioralData.HasTarget;
        }

    }

}
