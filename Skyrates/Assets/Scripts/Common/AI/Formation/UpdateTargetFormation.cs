using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Common.AI.Formation
{

    [CreateAssetMenu(menuName = "Data/AI/Formation/Update Target")]
    public class UpdateTargetFormation : Behavior
    {

        public override object GetUpdate(ref BehaviorData data, ref PhysicsData physics, float deltaTime, object persistent)
        {
            data.Target = data.FormationOwner == null ? physics : (data.FormationOwner.GetTarget(data.FormationSlot) ?? physics);
            return persistent;
        }

    }

}
