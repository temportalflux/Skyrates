using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ComponentType = ShipData.ComponentType;

public class ShipHull : ShipComponent
{

    [Serializable]
    public class HullTargets
    {

        [Serializable]
        public struct Target
        {
            // The pos/rot of the object that will be generated
            public Transform[] roots;

            // The objects generated during ShipBuilder.BuiltTo
            public ShipComponent[] generatedComponents;

        }
        
        // List of transforms/objects for each ComponentType
        public Target[] list;

    }

    public HullTargets targets;
    
    public Transform[] GetRoots(ComponentType compType)
    {
        Debug.Assert((int)ComponentType.Hull == 0);
        // compType - 1 to account for Hull
        return this.targets.list[(int) compType - 1].roots;
    }

    public void ClearGeneratedComponents()
    {
        foreach (ComponentType key in ShipData.ComponentTypes)
        {
            HullTargets.Target t = this.targets.list[(int)key];
            t.generatedComponents = new ShipComponent[t.roots.Length];
        }
    }

    public void AddShipComponent(ComponentType compType, int index, ShipComponent comp)
    {
        this.targets.list[(int) compType].generatedComponents[index] = comp;
    }

}
