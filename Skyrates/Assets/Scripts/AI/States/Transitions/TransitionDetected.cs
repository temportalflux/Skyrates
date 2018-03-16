using Skyrates.Common.AI;
using UnityEngine;

namespace Skyrates.AI.State.Transition
{

    public abstract class TransitionDetected<T> : StateTransition where T:MonoBehaviour
    {

        // TODO: Use the DistanceCollider check
        public float Distance;

        public override bool CanEnter(Behavior.DataBehavioral behavioralData, PhysicsData physics, ref Behavior.DataPersistent persistent)
        {
            // TODO: This is extremely expensive
            GameObject targetObj = GameObject.FindGameObjectWithTag("Player");
            T targetEntity = targetObj != null ? targetObj.GetComponent<T>() : null;
            if (targetEntity != null)
            {
                Vector3 diff = targetObj.transform.position - physics.LinearPosition;
                if (diff.sqrMagnitude <= this.Distance * this.Distance)
                {
                    return true;
                }
            }
            return false;
        }
        
    }

}
