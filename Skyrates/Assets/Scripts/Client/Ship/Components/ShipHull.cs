using Skyrates.Client.Entity;
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

        [SerializeField]
        public Transform[] LootMounts;

        #endregion

        /// <summary>
        /// The objects generated during <see cref="ShipBuilder.BuiltTo"/>.
        /// </summary>
        private readonly ShipComponent[][] GeneratedComponents = new ShipComponent[ShipData.NonHullComponents.Length][];

        private readonly List<GameObject> GeneratedLoot = new List<GameObject>();

		[Tooltip("The base amount of damage subtracted from damage taken")]
		public float Defense;

		[Tooltip("The percentage of damage subtracted from damage taken")]
		public float Protection;

		/// <summary>
		/// Gets the amount of damage subtracted from damage taken.
		/// </summary>
		/// <returns>The amount of damage subtracted from damage taken</returns>
		public float GetDefense()
		{
			return this.Defense;
		}

		/// <summary>
		/// Gets the percentage of damage subtracted from damage taken.
		/// </summary>
		/// <returns>The percentage of damage subtracted from damage taken</returns>
		public float GetProtection()
		{
			return this.Protection;
		}

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
            comp.transform.position += mount.Roots[index].localPosition;
            comp.transform.rotation = mount.Roots[index].localRotation;

            comp.Ship = this.Ship;
			//Special cases to set bonuses for navigation and propulsion components.
			EntityPlayerShip playerShip = comp.Ship as EntityPlayerShip;
			if (playerShip)
			{
				ShipNavigationLeft navigationComp = comp as ShipNavigationLeft; //Could also use ShipNavigation, but this lets us skip half of the adds, which is ideally 1.
				if (navigationComp)
				{
					playerShip.PlayerData.InputData.AdditionalTurnPercent = navigationComp.AdditionalTurnPercent;
				}
				else
				{
					ShipPropulsion propulsionComp = comp as ShipPropulsion;
					if (propulsionComp)
					{
						playerShip.PlayerData.InputData.AdditionalMovePercent = propulsionComp.AdditionalMovePercent;
					}
				}
			}
        }

        /// <summary>
        /// Returns the generated component list for a specific type.
        /// </summary>
        /// <param name="compType"></param>
        /// <returns></returns>
        public ShipComponent[] GetGeneratedComponent(ComponentType compType)
        {
            Debug.Assert((int) ComponentType.Hull != 0);
            Debug.Assert(ComponentType.Hull != compType, "Cannot get hull from hull");
            // compType - 1 to account for Hull
            return this.GeneratedComponents[this.GetComponentIndex(compType)];
        }

        public void GenerateLoot(GameObject lootObjectPrefab)
        {
            int nextIndex = this.GeneratedLoot.Count;
            if (nextIndex < this.LootMounts.Length)
            {
                Transform mount = this.LootMounts[nextIndex];
                GameObject generated = Instantiate(lootObjectPrefab, this.transform);
                generated.transform.SetPositionAndRotation(mount.position, mount.rotation);
                generated.transform.localScale = mount.localScale;
                this.GeneratedLoot.Add(generated);
            }
        }

    }

}
