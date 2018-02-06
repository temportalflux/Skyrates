using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Ship;
using Skyrates.Common.Network;
using UnityEngine;

/// <summary>
/// Data object used to send ship rig data over the network (and for tool usage in editor).
/// Stores references to components as integers for lookup in <see cref="ShipComponentList"/>.
/// </summary>
[Serializable]
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
    public static readonly ComponentType[] NonHullComponents = {
        ComponentType.Artillery,
        ComponentType.Figurehead,
        ComponentType.Navigation,
        ComponentType.Propulsion,
        ComponentType.Sail,
    };
    public static readonly int[] HulllessComponentIndex = { 0, 1, -1, 2, 3, 4 };

    public static readonly Type[] ComponentClassTypes =
    {
        typeof(ShipArtillery),
        typeof(ShipFigurehead),
        typeof(ShipHull),
        typeof(ShipNavigation),
        typeof(ShipPropulsion),
        typeof(ShipSail),
    };

    [BitSerialize(0)]
    [SerializeField]
    public int[] Components = new int[ComponentTypes.Length];

    public int this[ComponentType key]
    {
        get { return this.Components[(int)key]; }
        set { this.Components[(int)key] = value; }
    }

    /// <summary>
    /// Return the ship component for the selected category.
    /// </summary>
    /// <param name="list">The <see cref="ShipComponentList"/> reference.</param>
    /// <param name="type">The type of component requested.</param>
    /// <returns></returns>
    public ShipComponent GetShipComponent(ShipComponentList list, ComponentType type)
    {
        return list.GetRawComponent(type, this[type]);
    }

}
