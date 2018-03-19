using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Util;
using UnityEngine;

using ComponentType = ShipData.ComponentType;

namespace Skyrates.Client.Ship
{

    /// <summary>
    /// The listings of all components used in ships.
    /// Referenced by <see cref="ShipRig"/> and <see cref="ShipData"/> for generating
    /// ship rigs and syncing them across the network.
    /// </summary>
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
    
        /// <summary>
        /// Returns the ship component of specified type and at a specfic index in the listing.
        /// </summary>
        /// <param name="compType"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public ShipComponent GetRawComponent(ComponentType compType, int index)
        {
            ShipComponent component = null;
            this.TryGetValue(compType, index, out component);
            return component;
        }

        /// <summary>
        /// Returns <see cref="GetRawComponent"/> casted to a subclass of <see cref="ShipComponent"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="compType"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public T GetComponent<T>(ComponentType compType, int index) where T : ShipComponent
        {
            return (T)this.GetRawComponent(compType, index);
        }

    }

}