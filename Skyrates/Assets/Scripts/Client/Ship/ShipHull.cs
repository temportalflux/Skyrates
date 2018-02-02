using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipHull : ShipComponent
{

    [Serializable]
    public struct Target
    {
        public Transform[] roots;
    }

    public Target[] targets;

    public Transform[] GetRoots(ShipBuilder.ComponentType compType)
    {
        return this.targets[(int) compType].roots;
    }

    public void AddShipComponent(ShipBuilder.ComponentType compType, ShipComponent comp)
    {
        // TODO: Save these some how for reference when player controls (i.e. cannons)
    }

}
