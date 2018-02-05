using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.Network;
using UnityEngine;

public class ShipData
{

    public enum ComponentType
    {
        Hull, Propulsion, Artillery, Sail, Figurehead, Navigation,
    }

    public static readonly ComponentType[] ComponentTypes =
    {
        ComponentType.Hull,
        ComponentType.Propulsion,
        ComponentType.Artillery,
        ComponentType.Sail,
        ComponentType.Figurehead,
        ComponentType.Navigation,
    };

    public static readonly Type[] ComponentClassTypes =
    {
        typeof(ShipHull),
        typeof(ShipPropulsion),
        typeof(ShipArtillery),
        typeof(ShipSail),
        typeof(ShipFigurehead),
        typeof(ShipNavigation),
    };

    [BitSerialize()]
    public int[] components = new int[ComponentTypes.Length];

    public int this[ComponentType key]
    {
        get { return this.components[(int) key]; }
        set { this.components[(int) key] = value; }
    }

}
