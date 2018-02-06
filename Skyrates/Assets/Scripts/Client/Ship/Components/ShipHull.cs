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

        #region Generation

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

            /// <summary>
            /// The objects generated during <see cref="ShipBuilder.BuiltTo"/>.
            /// </summary>
            [SerializeField]
            public ShipComponent[] GeneratedComponents;

        }
        
        /// <summary>
        /// List of transforms/objects for each ComponentType.
        /// </summary>
        [SerializeField]
        public Mount[] Mounts;

        #endregion

        /// <summary>
        /// Returns an array of all the target transforms for a specificed type.
        /// </summary>
        /// <param name="compType"></param>
        /// <returns></returns>
        public Transform[] GetRoots(ComponentType compType)
        {
            Debug.Assert((int) ComponentType.Hull == 0);
            Debug.Assert(ComponentType.Hull != compType);

            // compType - 1 to account for Hull
            return this.Mounts[(int) compType - 1].Roots;
        }

        /// <summary>
        /// Clears any generated components from the list.
        /// <param name="doDestroy">If the objects should be destroyed via MonoBehavior.</param>
        /// </summary>
        public void ClearGeneratedComponents(bool doDestroy = false)
        {
            foreach (ComponentType key in ShipData.ComponentTypes)
            {
                if (key == ComponentType.Hull) continue;
                Mount t = this.Mounts[(int) key - 1];

                if (doDestroy)
                {
                    foreach (ShipComponent component in t.GeneratedComponents)
                    {
                        Destroy(component);
                    }
                }

                t.GeneratedComponents = new ShipComponent[t.Roots.Length];
                this.Mounts[(int) key - 1] = t;
            }
        }

        /// <summary>
        /// Sets the generated component of a type at the target index.
        /// </summary>
        /// <param name="compType"></param>
        /// <param name="index"></param>
        /// <param name="comp"></param>
        public void AddShipComponent(ComponentType compType, int index, ShipComponent comp)
        {
            Debug.Assert((int) ComponentType.Hull == 0);
            Debug.Assert(ComponentType.Hull != compType, "Cannot add hull to hull");

            // Set the generated component
            Mount t = this.Mounts[(int) compType - 1];
            t.GeneratedComponents[index] = comp;

            // Set the transform information on the component from the target
            comp.gameObject.transform.SetPositionAndRotation(t.Roots[index].position, t.Roots[index].rotation);

            // Update list
            this.Mounts[(int) compType - 1] = t;
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
            return this.Mounts[(int) compType - 1].GeneratedComponents;
        }

    }

}
