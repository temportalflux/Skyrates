using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.AI.State.Transition
{

    [CreateAssetMenu(menuName = "Data/AI/Composite/State/Transitions/Has Entity Nearby Formation")]
    public class TransitionDetectedNearbyTargetsFormation : TransitionDetectedNearbyTargets
    {

        protected override List<Behavior.DataBehavioral.NearbyTarget> GetNearbyFrom(Behavior.DataBehavioral data)
        {
            return data.NearbyFormationTargets;
        }

    }

}