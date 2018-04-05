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
            int tier = rig.ComponentTiers[(int)this.Type];
            T current = compList.GetComponent<T>(this.Type, tier);
            T max = compList.GetComponent<T>(this.Type, -1);
            return predicate(current) / predicate(max);
        }

    }

}
