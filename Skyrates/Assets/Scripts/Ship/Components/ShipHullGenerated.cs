using System;
using Skyrates.Entity;
using UnityEngine;

using ComponentType = Skyrates.Ship.ShipData.ComponentType;

namespace Skyrates.Ship
{

    public class ShipHullGenerated : ShipHull
    {

        [Serializable]
        public class MountList
        {
            public Transform[] Value;
        }

        /// <summary>
        /// List of transforms/objects for each ComponentType.
        /// </summary>
        [SerializeField]
        public MountList[] Mounts = new MountList[ShipData.ComponentTypes.Length];
        
        /// <summary>
        /// Creates the array of components for some type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="count"></param>
        public void SetShipComponentCount(ComponentType type, int count)
        {
            this.Components[this.GetComponentIndex(type)].Value = new ShipComponent[count];
        }

        /// <summary>
        /// Sets the generated component of a type at the target index.
        /// </summary>
        /// <param name="mounts"></param>
        /// <param name="compType"></param>
        /// <param name="index"></param>
        /// <param name="comp"></param>
        public void AddShipComponent(MountList[] mounts, ComponentType compType, int index, ShipComponent comp)
        {
            // Set the generated component
            this.Components[this.GetComponentIndex(compType)].Value[index] = comp;

            // Set the transform information on the component from the target
            Transform[] roots = mounts[this.GetComponentIndex(compType)].Value;

            comp.transform.localPosition += roots[index].localPosition;
            comp.transform.localRotation = roots[index].localRotation;

            comp.Ship = this.Ship;
            //Special cases to set bonuses for navigation and propulsion components.
            EntityPlayerShip playerShip = comp.Ship as EntityPlayerShip;
            if (playerShip)
            {
                ShipNavigationLeft navigationComp = comp as ShipNavigationLeft; //Could also use ShipNavigation, but this lets us skip half of the adds, which is ideally 1.
                if (navigationComp)
                {
                    // TODO: Put this in the ship stat data, not player data
                    //playerShip.PlayerData.AdditionalTurnPercent = navigationComp.AdditionalTurnPercent;
                }
                else
                {
                    ShipPropulsion propulsionComp = comp as ShipPropulsion;
                    if (propulsionComp)
                    {
                        //playerShip.PlayerData.AdditionalMovePercent = propulsionComp.AdditionalMovePercent;
                    }
                }
            }
        }

    }

}
