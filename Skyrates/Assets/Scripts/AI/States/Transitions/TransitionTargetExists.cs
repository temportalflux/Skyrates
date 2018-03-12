using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.AI;
using UnityEngine;

namespace Skyrates.Common.AI
{

    [CreateAssetMenu(menuName = "Data/AI/State/Transitions/Has Target")]
    public class TransitionTargetExists : StateTransition
    {

        public override bool CanEnter(BehaviorData behavioralData, PhysicsData physics)
        {
            return behavioralData.HasTarget;
        }

    }

}
