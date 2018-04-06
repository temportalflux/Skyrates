using System;
using Skyrates.Ship;
using UnityEngine;

namespace Skyrates.UI
{

    public class StatPanel : MonoBehaviour
    {

        public ShipData.ComponentType Type;

        public virtual void UpdateStats(ShipComponentList compList, ShipData rig)
        {
            
        }

        protected float GetStat<T>(ShipComponentList compList, ShipData rig,
            Func<T, float> predicate) where T : ShipComponent
        {
            return rig.GetStat(compList, this.Type, predicate);
        }

    }

}
