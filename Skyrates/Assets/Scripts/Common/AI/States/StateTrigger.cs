using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Common.AI
{

    [CreateAssetMenu(menuName = "Data/AI/Control/Trigger")]
    public abstract class StateTrigger : Behavior
    {

        /// <summary>
        /// Returns if the state can enter.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="physics"></param>
        /// <returns></returns>
        public abstract bool CanEnter(BehaviorData data, PhysicsData physics);

        /// <summary>
        /// Returns if the state can exit
        /// </summary>
        /// <param name="data"></param>
        /// <param name="physics"></param>
        /// <returns></returns>
        public abstract bool CanExit(BehaviorData data, PhysicsData physics);

    }

}
