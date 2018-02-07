using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ComponentType = ShipData.ComponentType;

namespace Skyrates.Client.Ship
{
    
    /// <summary>
    /// Subclass of <see cref="ShipComponent"/> dedicated to
    /// components of type <see cref="ShipData.ComponentType.Hull"/>.
    /// </summary>
    public class ShipHull : ShipComponent
    {

        #region PREFAB ONLY

        /// <summary>
        /// A set of targets for a specfic <see cref="ComponentType"/>.
        /// </summary>
        [Serializable]
        public struct Mount
        {
            /// <summary>
            /// The pos/rot of the object that will be generated.
            /// </summary>
            [SerializeField]
            public Transform[] Roots;
        }
        
        /// <summary>
        /// List of transforms/objects for each ComponentType.
        /// </summary>
        [SerializeField]
        public Mount[] Mounts;

        #endregion

        /// <summary>
        /// The objects generated during <see cref="ShipBuilder.BuiltTo"/>.
        /// </summary>
        private readonly ShipComponent[][] GeneratedComponents = new ShipComponent[ShipData.NonHullComponents.Length][];

        private int GetComponentIndex(ComponentType type)
        {
            return ShipData.HulllessComponentIndex[(int) type];
        }

        /// <summary>
        /// Creates the array of components for some type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="count"></param>
        public void SetShipComponentCount(ComponentType type, int count)
        {
            this.GeneratedComponents[this.GetComponentIndex(type)] = new ShipComponent[count];
        }

        /// <summary>
        /// Sets the generated component of a type at the target index.
        /// </summary>
        /// <param name="iMount"></param>
        /// <param name="index"></param>
        /// <param name="comp"></param>
        public void AddShipComponent(Mount[] mounts, ComponentType compType, int index, ShipComponent comp)
        {
            // Set the generated component
            this.GeneratedComponents[this.GetComponentIndex(compType)][index] = comp;

            // Set the transform information on the component from the target
            Mount mount = mounts[this.GetComponentIndex(compType)];
            comp.gameObject.transform.SetPositionAndRotation(mount.Roots[index].position, mount.Roots[index].rotation);
        }

        /// <summary>
        /// Returns the generated component list for a specific type.
        /// </summary>
        /// <param name="compType"></param>
        /// <returns></returns>
        public ShipComponent[] GetGeneratedComponent(ComponentType compType)
        {
            Debug.Assert((int) ComponentType.Hull == 0);
            Debug.Assert(ComponentType.Hull != compType, "Cannot get hull from hull");
            // compType - 1 to account for Hull
            return this.GeneratedComponents[this.GetComponentIndex(compType)];
        }


    }

}
