using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Util;
using UnityEngine;

using ComponentType = ShipData.ComponentType;

[CreateAssetMenu(menuName = "Data/Ship/List: Components")]
public class ShipComponentList : PrefabList
{

    void OnEnable()
    {
        this.Setup(ShipData.ComponentTypes, ShipData.ComponentClassTypes);
    }

    /// <inheritdoc />
    public override object GetKeyFrom(int index)
    {
        return (ComponentType) index;
    }

    /// <inheritdoc />
    public override int GetIndexFrom(object key)
    {
        return (int) key;
    }
    
    public ShipComponent GetRawComponent(ComponentType compType, int index)
    {
        ShipComponent component = null;
        this.TryGetValue(compType, index, out component);
        return component;
    }

    public T GetComponent<T>(ComponentType compType, int index) where T : ShipComponent
    {
        return (T)this.GetRawComponent(compType, index);
    }

}
