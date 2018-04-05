using UnityEngine;

namespace Skyrates.AI.State.Transition
{

    [CreateAssetMenu(menuName = "Data/AI/Composite/State/Transitions/Has Threat Nearby Formation")]
    public class TransitionDetectedNearbyThreatsFormation : TransitionDetectedNearbyTargets
    {

        protected override int GetNearbyCount(Behavior.DataBehavioral data)
        {
            return data.Formation.GetNearbyThreats().Count;
        }

    }

}