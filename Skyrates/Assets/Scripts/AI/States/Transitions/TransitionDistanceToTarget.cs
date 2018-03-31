using System.Collections;
using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.State.Transition
{

    /// <summary>
    /// Transitions if the target exists.
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Composite/State/Transitions/Distance to Target")]
    public class TransitionDistanceToTarget : StateTransition
    {

        public bool IsInsideDistance = true;
        public float Distance;
        private float _distanceSq;

        public override Behavior.DataPersistent CreatePersistentData()
        {
            this._distanceSq = this.Distance * this.Distance;
            return base.CreatePersistentData();
        }

        /// <inheritdoc />
        public override bool CanEnter(Behavior.DataBehavioral behavioralData, PhysicsData physics,
            ref Behavior.DataPersistent persistent)
        {
            float distSq = (physics.LinearPosition - behavioralData.Target.LinearPosition).sqrMagnitude;
            return this.IsInsideDistance ? distSq < this._distanceSq : distSq > this._distanceSq;
        }
        
    }

}
