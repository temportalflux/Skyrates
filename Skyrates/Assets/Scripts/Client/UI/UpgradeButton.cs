using Skyrates.Client.Data;
using Skyrates.Client.Game;
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
			this.PlayerData.Inventory.Remove(Type);
		}

		public void AddUpgradeListener()
		{
			this.Button.onClick.AddListener(UpgradeItem);
		}

		public void RemoveUpgradeListener()
		{
			this.Button.onClick.RemoveAllListeners();
		}
	}
}
