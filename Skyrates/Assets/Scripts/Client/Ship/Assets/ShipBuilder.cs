using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ComponentType = ShipData.ComponentType;

namespace Skyrates.Client.Ship
{

    [CreateAssetMenu(menuName = "Data/Ship/Builder")]
    public class ShipBuilder : ScriptableObject
    {

        /// <summary>
        /// The list of all components in the game.
        /// </summary>
        public ShipComponentList ShipComponentList;

        /// <summary>
        /// The data references for every component this rig uses.
        /// </summary>
        [SerializeField] public ShipData ShipData;

        #region Get Components

        /// <summary>
        /// Return the ship component for the selected category.
        /// </summary>
        /// <param name="type">The type of component requested.</param>
        /// <param name="data">The ship data to reference.</param>
        /// <returns></returns>
        public ShipComponent GetShipComponent(ComponentType type, ShipData data)
        {
            return data.GetShipComponent(this.ShipComponentList, type);
        }

        /// <summary>
        /// Retrives the ship component <see cref="ShipArtillery"/> from passed or stored data.
        /// </summary>
        /// <param name="data">The data to pull from, stored data if null.</param>
        /// <returns></returns>
        public ShipArtillery GetArtillery(ShipData data = null)
        {
            if (data == null) data = this.ShipData;
            return (ShipArtillery) this.GetShipComponent(ComponentType.Artillery, data);
        }

        /// <summary>
        /// Retrives the ship component <see cref="ShipFigurehead"/> from passed or stored data.
        /// </summary>
        /// <param name="data">The data to pull from, stored data if null.</param>
        /// <returns></returns>
        public ShipFigurehead GetFigurehead(ShipData data = null)
        {
            if (data == null) data = this.ShipData;
            return (ShipFigurehead) this.GetShipComponent(ComponentType.Figurehead, data);
        }

        /// <summary>
        /// Retrives the ship component <see cref="ShipHull"/> from passed or stored data.
        /// </summary>
        /// <param name="data">The data to pull from, stored data if null.</param>
        /// <returns></returns>
        public ShipHull GetHull(ShipData data = null)
        {
            if (data == null) data = this.ShipData;
            return (ShipHull) this.GetShipComponent(ComponentType.Hull, data);
        }

        /// <summary>
        /// Retrives the ship component <see cref="ShipNavigation"/> from passed or stored data.
        /// </summary>
        /// <param name="data">The data to pull from, stored data if null.</param>
        /// <returns></returns>
        public ShipNavigation GetNavigation(ShipData data = null)
        {
            if (data == null) data = this.ShipData;
            return (ShipNavigation) this.GetShipComponent(ComponentType.Navigation, data);
        }

        /// <summary>
        /// Retrives the ship component <see cref="ShipPropulsion"/> from passed or stored data.
        /// </summary>
        /// <param name="data">The data to pull from, stored data if null.</param>
        /// <returns></returns>
        public ShipPropulsion GetPropulsion(ShipData data = null)
        {
            if (data == null) data = this.ShipData;
            return (ShipPropulsion) this.GetShipComponent(ComponentType.Propulsion, data);
        }

        /// <summary>
        /// Retrives the ship component <see cref="ShipSail"/> from passed or stored data.
        /// </summary>
        /// <param name="data">The data to pull from, stored data if null.</param>
        /// <returns></returns>
        public ShipSail GetSail(ShipData data = null)
        {
            if (data == null) data = this.ShipData;
            return (ShipSail) this.GetShipComponent(ComponentType.Sail, data);
        }

        #endregion

        /// <summary>
        /// Builds the current <see cref="ShipData"/> as a GameObject.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="data">The data to use, stored if null</param>
        /// <returns></returns>
        public ShipHull BuildTo(ref Transform root, ShipData data = null)
        {
            if (data == null) data = this.ShipData;

            // Create the hull
            ShipHull hullPrefab = (ShipHull) this.GetShipComponent(ComponentType.Hull, data);
            ShipHull hullBuilt = Instantiate(hullPrefab.gameObject, root).GetComponent<ShipHull>();

            int iMount = 0;
            // Create all the remaining components
            foreach (ComponentType compType in ShipData.ComponentTypes)
            {
                if (compType == ComponentType.Hull) continue;

                // Get the component for the type
                ShipComponent component = this.GetShipComponent(compType, data);

                // Skip if the component does not exist (component was empty in editor)
                if (component == null) continue;

                // Get the prefab object
                GameObject prefab = component.gameObject;

                // Get all the targets for the type of component (transforms on hull to generate at)
                Transform[] targets = hullBuilt.Mounts[iMount].Roots;

                hullBuilt.SetShipComponentCount(compType, targets.Length);

                // Generate a component of the current type at each target
                for (int iTarget = 0; iTarget < targets.Length; iTarget++)
                {
                    Transform target = targets[iTarget];
                    // Create the object
                    GameObject built = Instantiate(prefab, target.position, target.rotation, root);
                    // Tell the built hull that it exists
                    // TODO: Optimize this function to just send in the transform
                    hullBuilt.AddShipComponent(hullPrefab.Mounts, compType, iTarget, built.GetComponent<ShipComponent>());
                }

                iMount++;
            }

            return hullBuilt;
        }

    }

}
