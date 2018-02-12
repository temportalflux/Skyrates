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
public class ShipData : ISerializing
{

    public enum ComponentType
    {
       Artillery, Figurehead, Hull, NavigationLeft, NavigationRight, Propulsion, Sail,
    }

    public static readonly object[] ComponentTypes =
    {
        ComponentType.Artillery,
        ComponentType.Figurehead,
        ComponentType.Hull,
        ComponentType.NavigationLeft,
        ComponentType.NavigationRight,
        ComponentType.Propulsion,
        ComponentType.Sail,
    };
    public static readonly ComponentType[] NonHullComponents = {
        ComponentType.Artillery,
        ComponentType.Figurehead,
        ComponentType.NavigationLeft,
        ComponentType.NavigationRight,
        ComponentType.Propulsion,
        ComponentType.Sail,
    };
    public static readonly int[] HulllessComponentIndex = { 0, 1, -1, 2, 3, 4, 5 };

    public static readonly Type[] ComponentClassTypes =
    {
        typeof(ShipArtillery),
        typeof(ShipFigurehead),
        typeof(ShipHull),
        typeof(ShipNavigationLeft),
        typeof(ShipNavigationRight),
        typeof(ShipPropulsion),
        typeof(ShipSail),
    };
    
    [SerializeField]
    public int[] Components = new int[ComponentTypes.Length];

    [SerializeField]
    private bool _hasNewData = false;

    public bool MustBeRebuilt
    {
        get { return this._hasNewData; }
        set { this._hasNewData = value; }
    }

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


    public int GetSize()
    {
        return sizeof(int) + this.Components.Length * sizeof(int);
    }

    public void Serialize(ref byte[] data, ref int lastIndex)
    {
        data = BitSerializeAttribute.Serialize(this.Components, data, lastIndex);
        lastIndex += this.GetSize();
    }

    public void Deserialize(byte[] data, ref int lastIndex)
    {
        int[] deserializedComponents = (int[])BitSerializeAttribute.Deserialize(this.Components, data, ref lastIndex);
        if (deserializedComponents.Length != this.Components.Length)
        {
            this._hasNewData = true;
        }
        else
        {
            for (int iComponent = 0; iComponent < this.Components.Length; iComponent++)
            {
                if (this.Components[iComponent] != deserializedComponents[iComponent])
                {
                    this._hasNewData = true;
                    break;
                }
            }
        }

        if (this._hasNewData)
        {
            this.Components = deserializedComponents;
        }
    }

}
