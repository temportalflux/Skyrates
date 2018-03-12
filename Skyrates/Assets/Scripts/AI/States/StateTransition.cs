using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Common.AI
{

    [Serializable]
    public abstract class StateTransition : ScriptableObject
    {

        [HideInInspector]
        public int StateSource;

        [HideInInspector]
        public int StateDestination;

        public State GetStateSource(StateMachine machine)
        {
            return machine.GetState(this.StateSource);
        }

        public State GetStateDestination(StateMachine machine)
        {
            return machine.GetState(this.StateDestination);
        }

        public abstract bool CanEnter(BehaviorData behavioralData, PhysicsData physics);

    }

}
