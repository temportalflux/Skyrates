using Skyrates.Entity;
using UnityEngine;

using ComponentType = Skyrates.Ship.ShipData.ComponentType;

namespace Skyrates.Ship
{

    [CreateAssetMenu(menuName = "Data/Ship/Builder")]
    public class ShipRig : ScriptableObject
    {

        /// <summary>
        /// The list of all components in the game.
        /// </summary>
        public ShipComponentList ShipComponentList;

        /// <summary>
        /// The data references for every component this rig uses.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        public ShipData ShipData;

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
        public ShipArtillery GetArtilleryLeft(ShipData data = null)
        {
            if (data == null) data = this.ShipData;
            return (ShipArtillery)this.GetShipComponent(ComponentType.ArtilleryLeft, data);
        }

        /// <summary>
        /// Retrives the ship component <see cref="ShipArtillery"/> from passed or stored data.
        /// </summary>
        /// <param name="data">The data to pull from, stored data if null.</param>
        /// <returns></returns>
        public ShipArtillery GetArtilleryRight(ShipData data = null)
        {
            if (data == null) data = this.ShipData;
            return (ShipArtillery)this.GetShipComponent(ComponentType.ArtilleryRight, data);
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
        public ShipHull GetHullArmor(ShipData data = null)
        {
            if (data == null) data = this.ShipData;
            return (ShipHull) this.GetShipComponent(ComponentType.HullArmor, data);
        }

        /// <summary>
        /// Retrives the ship component <see cref="ShipNavigation"/> from passed or stored data.
        /// </summary>
        /// <param name="data">The data to pull from, stored data if null.</param>
        /// <returns></returns>
        public ShipNavigation GetNavigationLeft(ShipData data = null)
        {
            if (data == null)
                data = this.ShipData;
            return (ShipNavigation) this.GetShipComponent(ComponentType.NavigationLeft, data);
        }

        /// <summary>
        /// Retrives the ship component <see cref="ShipNavigation"/> from passed or stored data.
        /// </summary>
        /// <param name="data">The data to pull from, stored data if null.</param>
        /// <returns></returns>
        public ShipNavigation GetNavigationRight(ShipData data = null)
        {
            if (data == null)
                data = this.ShipData;
            return (ShipNavigation) this.GetShipComponent(ComponentType.NavigationRight, data);
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

        #endregion

        /// <summary>
        /// Builds the current <see cref="ShipData"/> as a GameObject.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="data">The data to use, stored if null</param>
        /// <returns></returns>
        public void BuildTo(EntityShip owner, ref Transform root, ShipData data = null)
        {
            if (data == null) data = this.ShipData;
            
            ShipHull hull = owner.GetHull();
            ShipHullGenerated hullGenerated = hull as ShipHullGenerated;
            if (hull == null || hullGenerated == null)
            {
                Debug.LogError(string.Format(
                    "Could not generate ship onto non-generatable hull. Hull is null or is not {0}.",
                    typeof(ShipHullGenerated).Name
                ));
                return;
            }
            
            // Create all the remaining components
            foreach (ComponentType compType in ShipData.ComponentTypes)
            {

                // Get the component for the type
                ShipComponent component = this.GetShipComponent(compType, data);

                // Skip if the component does not exist (component was empty in editor)
                if (component == null) continue;

                // Get the prefab object
                GameObject prefab = component.gameObject;

                // Get all the targets for the type of component (transforms on hull to generate at)
                Transform[] targets = hullGenerated.Mounts[(int)compType].Value;

                hullGenerated.SetShipComponentCount(compType, targets.Length);

                // Generate a component of the current type at each target
                for (int iTarget = 0; iTarget < targets.Length; iTarget++)
                {
                    //Transform target = targets[iTarget];
                    // Create the object
                    GameObject built = Instantiate(prefab, root);
                    built.transform.SetPositionAndRotation(hullGenerated.transform.position, hullGenerated.transform.rotation);
                    // Tell the built hull that it exists
                    // TODO: Optimize this function to just send in the transform
                    hullGenerated.AddShipComponent(hullGenerated.Mounts, compType, iTarget, built.GetComponent<ShipComponent>());
                }
                
            }
            
        }

        /* This is outdated and redundant. Use Destroy/Generate and Increment the ship data tier index via ShipData.Upgrade
		/// <summary>
		/// Upgrades the ship to the next tier.
		/// </summary>
		/// <param name="brokenComponentType">Broken</param>
		/// <param name="data">The data to use, stored if null</param>
		/// <returns>True if upgrade is successful, false otherwise.</returns>
		public bool UpgradeComponent(EntityPlayerShip player, ShipData.BrokenComponentType brokenComponentType, ShipData data = null)
		{
			if (data == null) data = this.ShipData;
			List<ShipData.ComponentType> componentTypes = new List<ComponentType>(3);
			switch(brokenComponentType)
			{
				case ShipData.BrokenComponentType.Artillery:
					componentTypes.Add(ComponentType.ArtilleryForward);
					componentTypes.Add(ComponentType.ArtilleryLeft);
					componentTypes.Add(ComponentType.ArtilleryRight);
					break;
				case ShipData.BrokenComponentType.Figurehead:
					componentTypes.Add(ComponentType.Figurehead);
					break;
				case ShipData.BrokenComponentType.Hull:
					componentTypes.Add(ComponentType.Hull);
					break;
				case ShipData.BrokenComponentType.Navigation:
					componentTypes.Add(ComponentType.NavigationLeft);
					componentTypes.Add(ComponentType.NavigationRight);
					break;
				case ShipData.BrokenComponentType.Propulsion:
					componentTypes.Add(ComponentType.Propulsion);
					break;
				default:
					return false;
			}

			// Create all the remaining components
			foreach (ComponentType componentType in componentTypes)
			{
				if (componentType == ShipData.ComponentType.Hull)
				{
					// Create the hull
					ShipComponent[] oldHullComponentsRef = player.GetShipComponentsOfType(ComponentType.Hull);
					ShipComponent[] oldHullComponents = new ShipComponent[oldHullComponentsRef.Length];
					oldHullComponentsRef.CopyTo(oldHullComponents, 0); //We want a copy so that we can keep the references to delete.
					ShipComponent oldHullComponent = oldHullComponents.Length > 0 ? oldHullComponents[0] : null;
					uint oldHullTierIndex = oldHullComponent ? oldHullComponent.TierIndex : 0;
					ShipHull hullPrefab = this.ShipComponentList.GetComponent<ShipHull>(ComponentType.Hull, (int)++oldHullTierIndex);
					ShipHull hullBuilt = Instantiate(hullPrefab.gameObject, player.ShipRoot.ComponentRoot).GetComponent<ShipHull>();
					hullBuilt.Ship = player;
					player.Health = player.StatBlock.MaxHealth;

					//Create new components
					//TODO: Attach the old components back on 
					foreach (ComponentType compType in ShipData.NonHullComponents)
					{
						ShipComponent[] oldComponentsRef = player.GetShipComponentsOfType(compType);
						ShipComponent[] oldComponents = oldComponentsRef != null && oldComponentsRef.Length > 0 ? new ShipComponent[oldComponentsRef.Length] : null;
						if(oldComponentsRef != null) oldComponentsRef.CopyTo(oldComponents, 0); //We want a copy so that we can keep the references to delete.
						ShipComponent oldComponent = oldComponents != null && oldComponents.Length > 0 ? oldComponents[0] : null;
						uint oldTierIndex = oldComponent ? oldComponent.TierIndex : 0;

						// Get the component for the type
						ShipComponent component = this.ShipComponentList.GetRawComponent(compType, (int)oldTierIndex);

						// Skip if the component does not exist (component was empty in editor)
						if (component == null) continue;

						// Get the prefab object
						GameObject prefab = component.gameObject;

						// Get all the targets for the type of component (transforms on hull to generate at)
						Transform[] targets = hullPrefab.Mounts[ShipData.HulllessComponentIndex[(int)compType]].Roots;

						hullBuilt.SetShipComponentCount(compType, targets.Length);

						// Generate a component of the current type at each target
						for (int iTarget = 0; iTarget < targets.Length; iTarget++)
						{
							//Transform target = targets[iTarget];
							// Create the object
							GameObject built = Instantiate(prefab, player.ShipRoot.ComponentRoot);
							built.transform.SetPositionAndRotation(hullBuilt.transform.position, hullBuilt.transform.rotation);
							// Tell the built hull that it exists
							// TODO: Optimize this function to just send in the transform
							hullBuilt.AddShipComponent(hullPrefab.Mounts, compType, iTarget, built.GetComponent<ShipComponent>());
						}
						if (oldComponents != null) //Should already be taken care of, but let's be explicit.
						{
							foreach (ShipComponent oldComp in oldComponents)
							{
								if (oldComp != null) GameObject.Destroy(oldComp.gameObject);
							}
						}
					}
					foreach (ShipComponent oldComp in oldHullComponents)
					{
						GameObject.Destroy(oldComp.gameObject);
					}
					if (player.PlayerData.Inventory.GeneratedLoot.Length > 0) //Destruction is delayed, so generated should still exist.
					{
						//We want to clone the pointers to the list in order to stay pointed to them while we repopulate the new hull.
						List<GameObject>[] generatedLoot = (List<GameObject>[])player.PlayerData.Inventory.GeneratedLoot.Clone(); //Forgot about this method...
						Array.Clear(player.PlayerData.Inventory.GeneratedLoot, 0, player.PlayerData.Inventory.GeneratedLoot.Length); //Clear so that we start anew
						for (uint i = 0; i < generatedLoot.Length; ++i)
						{
							ShipData.BrokenComponentType brokenComponent = (ShipData.BrokenComponentType)i;
							foreach (GameObject generated in generatedLoot[i])
							{
								if (generated == null) player.PlayerData.Inventory.GeneratedLoot[i].Add(null);
								else
								{
									hullBuilt.GenerateLoot(generated, brokenComponent, true);
									GameObject.Destroy(generated);  //Mark for destruction.  Not necessary, but safety first.
								}
							}
							generatedLoot[i].Clear(); //Clean up references just in case.
						}
					}
					player.ShipRoot.Hull = hullBuilt;
				}
				else
				{
					ShipHull hullBuilt = player.ShipRoot.Hull;

					ShipComponent[] oldComponentsRef = player.GetShipComponentsOfType(componentType);
					ShipComponent[] oldComponents = oldComponentsRef != null && oldComponentsRef.Length > 0 ? new ShipComponent[oldComponentsRef.Length] : null;
					if (oldComponentsRef != null) oldComponentsRef.CopyTo(oldComponents, 0); //We want a copy so that we can keep the references to delete.
					ShipComponent oldComponent = oldComponents != null && oldComponents.Length > 0 ? oldComponents[0] : null;
					uint oldTierIndex = oldComponent ? oldComponent.TierIndex : 0;

					// Get the component for the type
					ShipComponent component = this.ShipComponentList.GetRawComponent(componentType, (int)++oldTierIndex);

					// Skip if the component does not exist (component was empty in editor)
					if (component == null) continue;

					// Get the prefab object
					GameObject prefab = component.gameObject;

					// Get all the targets for the type of component (transforms on hull to generate at)
					Transform[] targets = hullBuilt.Mounts[ShipData.HulllessComponentIndex[(int)componentType]].Roots;

					hullBuilt.SetShipComponentCount(componentType, targets.Length); //Just in case.

					// Generate a component of the current type at each target
					for (int iTarget = 0; iTarget < targets.Length; iTarget++)
					{
						//Transform target = targets[iTarget];
						// Create the object
						GameObject built = Instantiate(prefab, player.ShipRoot.ComponentRoot);
						built.transform.SetPositionAndRotation(hullBuilt.transform.position, hullBuilt.transform.rotation);
						// Tell the built hull that it exists
						// TODO: Optimize this function to just send in the transform
						hullBuilt.AddShipComponent(hullBuilt.Mounts, componentType, iTarget, built.GetComponent<ShipComponent>());
					}
					if (oldComponents != null) //Explicit
					{
						foreach (ShipComponent oldComp in oldComponents)
						{
							if(oldComp != null) GameObject.Destroy(oldComp.gameObject);
						}
					}
				}
			}

			return true;
		}
        //*/

	}

}
