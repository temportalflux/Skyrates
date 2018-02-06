﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ComponentType = ShipData.ComponentType;

[CreateAssetMenu(menuName = "Data/Ship/Builder")]
public partial class ShipBuilder : ScriptableObject
{
    public ShipComponentList shipComponentList;

    public ShipData shipData = new ShipData();

    public ShipHull GetHull(ShipData data = null)
    {
        if (data == null) data = this.shipData;
        return (ShipHull)this.GetShipComponent(ComponentType.Hull, data);
    }

    public ShipPropulsion GetPropulsion(ShipData data = null)
    {
        if (data == null) data = this.shipData;
        return (ShipPropulsion)this.GetShipComponent(ComponentType.Propulsion, data);
    }

    public ShipArtillery GetArtillery(ShipData data = null)
    {
        if (data == null) data = this.shipData;
        return (ShipArtillery)this.GetShipComponent(ComponentType.Artillery, data);
    }

    public ShipSail GetSail(ShipData data = null)
    {
        if (data == null) data = this.shipData;
        return (ShipSail)this.GetShipComponent(ComponentType.Sail, data);
    }

    public ShipFigurehead GetFigurehead(ShipData data = null)
    {
        if (data == null) data = this.shipData;
        return (ShipFigurehead)this.GetShipComponent(ComponentType.Figurehead, data);
    }

    public ShipNavigation GetNavigation(ShipData data = null)
    {
        if (data == null) data = this.shipData;
        return (ShipNavigation)this.GetShipComponent(ComponentType.Navigation, data);
    }

    /// <summary>
    /// Returns server-serializable _gameStateData representing the parts of the ship
    /// </summary>
    /// <returns></returns>
    public ShipData GetData()
    {
        return this.shipData;
    }

    public ShipComponent GetShipComponent(ComponentType type, ShipData data)
    {
        return this.shipComponentList.GetRawComponent(type, data[type]);
    }

    /// <summary>
    /// Builds the current _gameStateData as a GameObject
    /// </summary>
    /// <param name="root"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public ShipHull BuildTo(ref Transform root, ShipData data = null)
    {
        if (data == null) data = this.shipData;
        
        ShipHull hullPrefab = (ShipHull)this.GetShipComponent(ComponentType.Hull, data);
        ShipHull hullBuilt = Instantiate(hullPrefab.gameObject, root).GetComponent<ShipHull>();

        hullBuilt.ClearGeneratedComponents();

        foreach (ComponentType compType in ShipData.ComponentTypes)
        {
            if (compType == ComponentType.Hull) continue;

            ShipComponent component = this.GetShipComponent(compType, data);
            if (component != null)
            {
                GameObject prefab = component.gameObject;

                Transform[] targets = hullBuilt.GetRoots(compType);

                for (int iTarget = 0; iTarget < targets.Length; iTarget++)
                {
                    Transform target = targets[iTarget];
                    GameObject built = Instantiate(prefab, target.position, target.rotation, root);
                    hullBuilt.AddShipComponent(compType, iTarget, built.GetComponent<ShipComponent>());
                }
            }
            
        }

        return hullBuilt;
    }

}
