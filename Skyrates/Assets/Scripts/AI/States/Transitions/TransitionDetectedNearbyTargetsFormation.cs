using UnityEngine;

namespace Skyrates.AI.State.Transition
{

    [CreateAssetMenu(menuName = "Data/AI/Composite/State/Transitions/Has Entity Nearby Formation")]
    public class TransitionDetectedNearbyTargetsFormation : TransitionDetectedNearbyTargets
    {

        protected override int GetNearbyCount(Behavior.DataBehavioral data)
        {
            return data.Formation.GetNearbyTargets().Count;
        }

    }

}