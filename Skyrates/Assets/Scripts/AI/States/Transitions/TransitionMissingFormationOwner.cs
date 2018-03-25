using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.State.Transition
{

    [CreateAssetMenu(menuName = "Data/AI/Composite/State/Transitions/Missing Formation Owner")]
    public class TransitionMissingFormationOwner : StateTransition
    {

        public override bool CanEnter(Behavior.DataBehavioral behavioralData, PhysicsData physics,
            ref Behavior.DataPersistent persistent)
        {
            return behavioralData.Formation != null &&
                behavioralData.Formation.FormationOwner == null;
        }

    }

}