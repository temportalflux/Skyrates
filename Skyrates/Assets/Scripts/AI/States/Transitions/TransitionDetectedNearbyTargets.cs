using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.State.Transition
{

    [CreateAssetMenu(menuName = "Data/AI/Composite/State/Transitions/Has Entity Nearby")]
    public class TransitionDetectedNearbyTargets : StateTransition
    {

        public bool HasMin = true;
        public int Min = 0;

        public bool HasMax = false;
        public int Max = 1;

        protected virtual int GetNearbyCount(Behavior.DataBehavioral data)
        {
            return data.NearbyTargets.Count;
        }

        public override bool CanEnter(Behavior.DataBehavioral behavioralData, PhysicsData physics, ref Behavior.DataPersistent persistent)
        {
            int numNearby = this.GetNearbyCount(behavioralData);
            return (!this.HasMin || numNearby >= this.Min) && (!this.HasMax || numNearby <= this.Max);
        }

    }

}
