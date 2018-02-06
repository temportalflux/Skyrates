using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.Network;
using UnityEngine;

public class ShipData
{

    public enum ComponentType
    {
        Artillery, Figurehead, Hull, Navigation, Propulsion, Sail, 
    }

    public static readonly object[] ComponentTypes =
    {
        ComponentType.Artillery,
        ComponentType.Figurehead,
        ComponentType.Hull,
        ComponentType.Navigation,
        ComponentType.Propulsion,
        ComponentType.Sail,
    };

    public static readonly Type[] ComponentClassTypes =
    {
        typeof(ShipArtillery),
        typeof(ShipFigurehead),
        typeof(ShipHull),
        typeof(ShipNavigation),
        typeof(ShipPropulsion),
        typeof(ShipSail),
    };

    [BitSerialize()]
    public int[] Components = new int[ComponentTypes.Length];

    public int this[ComponentType key]
    {
        get { return this.Components[(int) key]; }
        set { this.Components[(int) key] = value; }
    }

}
