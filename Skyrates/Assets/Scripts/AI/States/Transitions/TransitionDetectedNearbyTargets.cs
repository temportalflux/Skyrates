using System.Collections.Generic;
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

        protected virtual List<Behavior.DataBehavioral.NearbyTarget> GetNearbyFrom(Behavior.DataBehavioral data)
        {
            return data.NearbyTargets;
        }

        public override bool CanEnter(Behavior.DataBehavioral behavioralData, PhysicsData physics, ref Behavior.DataPersistent persistent)
        {
            int numNearby = this.GetNearbyFrom(behavioralData).Count;
            return (!this.HasMin || numNearby >= this.Min) && (!this.HasMax || numNearby <= this.Max);
        }

    }

}
