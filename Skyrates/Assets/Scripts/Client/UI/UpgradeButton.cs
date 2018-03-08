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
		public LocalData PlayerData;
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
			List<ShipData.ComponentType> componentTypes = new List<ShipData.ComponentType>(3);
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
			bool isUpgradableFurther = true;
			foreach(ShipData.ComponentType type in componentTypes)
			{
				ShipComponent[] oldComponents = this._player.GetShipComponentsOfType(type);
				ShipComponent oldComponent = oldComponents.Length > 0 ? oldComponents[0] : null;
				uint oldTierIndex = oldComponent ? oldComponent.TierIndex : 0;
				if(++oldTierIndex >= this._player.ShipRoot.Blueprint.ShipComponentList.Categories[this._player.ShipRoot.Blueprint.ShipComponentList.GetIndexFrom(type)].Prefabs.Length) { isUpgradableFurther = false; break; }
			}
			if (isUpgradableFurther && this.PlayerData.Inventory.Remove(Type) != 0);
			{
				this._player.ShipRoot.Blueprint.UpgradeComponent(this._player, Type);
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
