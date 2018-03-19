using Skyrates.Client.Data;
using Skyrates.Client.Entity;
using Skyrates.Client.Game;
using Skyrates.Client.Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Skyrates.Client.UI
{
	public class UpgradeButton : MonoBehaviour
	{
		//TODO: doxygen
		public PlayerData PlayerData;
		public ShipData.BrokenComponentType Type; //Type of the button, what item type it aligns to.
		private Button _button;
		private EntityPlayerShip _player;

		[HideInInspector]
		public Button Button
		{
			get
			{
				if(!this._button) this._button = GetComponent<Button>(); //Workaround: Since the parent game object is disabled at start, Awake is never called, so we must set the button explicitly.
				return this._button;
			}

			set
			{
				this._button = value;
			}
		}

		//Removes item from local data and upgrades the tier by 1.
		public void UpgradeItem()
		{
            // Get the list of all the actual components that this button is set to upgrade
            // 1 button can upgrade multiple components (i.e. upgrade all cannons at once, or both sides of navigation)
			List<ShipData.ComponentType> componentTypes = new List<ShipData.ComponentType>(3);
            // TODO: Move this to a dictionary lookup + addrange
            switch (Type)
			{
				case ShipData.BrokenComponentType.Artillery:
					componentTypes.Add(ShipData.ComponentType.ArtilleryForward);
					componentTypes.Add(ShipData.ComponentType.ArtilleryLeft);
					componentTypes.Add(ShipData.ComponentType.ArtilleryRight);
					break;
				case ShipData.BrokenComponentType.Figurehead:
					componentTypes.Add(ShipData.ComponentType.Figurehead);
					break;
				case ShipData.BrokenComponentType.Hull:
					componentTypes.Add(ShipData.ComponentType.Hull);
					break;
				case ShipData.BrokenComponentType.Navigation:
					componentTypes.Add(ShipData.ComponentType.NavigationLeft);
					componentTypes.Add(ShipData.ComponentType.NavigationRight);
					break;
				case ShipData.BrokenComponentType.Propulsion:
					componentTypes.Add(ShipData.ComponentType.Propulsion);
					break;
				default:
					break;
			}

            // Iterate over each component to determine if ALL components can be upgraded at least once
			bool isUpgradableFurther = true;
			foreach(ShipData.ComponentType type in componentTypes)
			{
                // Get the current list of components for each type
				ShipComponent[] oldComponents = this._player.GetShipComponentsOfType(type);
                // Get the first in the list
				ShipComponent oldComponent = oldComponents != null && oldComponents.Length > 0 ? oldComponents[0] : null;
                // Get the tier of the component list
                // TODO: Add getter to EntityShip indicating the current tier of each component type
				uint oldTierIndex = oldComponent != null ? oldComponent.TierIndex : 0;
                // Get the current rig and component list to check for max upgrades
			    ShipRig playerShipRig = this._player.ShipRoot.Blueprint;
			    ShipComponentList componentList = playerShipRig.ShipComponentList;
                // Check to see if the component can be upgraded any more
			    if (oldTierIndex + 1 >= componentList.Categories[componentList.GetIndexFrom(type)].Prefabs.Length)
			    {
			        isUpgradableFurther = false;
			        break;
			    }
			}
            // If all components have a next tier, and we have enough inventory
			if (isUpgradableFurther && this.PlayerData.Inventory.Remove(Type) != 0)
			{
                // Upgrade the component
			    this._player.ShipRoot.UpgradeComponents(componentTypes);

                // Regenerate the ship rig
			    this._player.ShipRoot.ReGenerate(this._player);
			}
		}

		public void AddUpgradeListener(EntityPlayerShip player)
		{
			this._player = player;
			this.Button.onClick.AddListener(UpgradeItem);
		}

		public void RemoveUpgradeListener()
		{
			this.Button.onClick.RemoveAllListeners();
		}
	}
}
