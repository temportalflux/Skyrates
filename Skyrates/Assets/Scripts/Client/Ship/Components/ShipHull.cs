using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipHull : ShipComponent
{

    [Serializable]
    public class HullTargets
    {

        [Serializable]
        public struct Target
        {
            public Transform[] roots;
        }
        
        public Target[] list;

    }

    public HullTargets targets;
    
    public Transform[] GetRoots(ShipBuilder.ComponentType compType)
    {
        Debug.Assert((int)ShipBuilder.ComponentType.Hull == 0);
        // compType - 1 to account for Hull
        return this.targets.list[(int) compType - 1].roots;
    }

    public void AddShipComponent(ShipBuilder.ComponentType compType, ShipComponent comp)
    {
        // TODO: Save these some how for reference when player controls (i.e. cannons)
    }

}
