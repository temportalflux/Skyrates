using Skyrates.Client.Data;
using Skyrates.Client.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Skyrates.Client.UI
{
	[RequireComponent(typeof(Text))]
	public class InventoryItemText : MonoBehaviour
	{
		/// <summary>
		/// The player's local data.  Used to access the player's inventory.
		/// </summary>
		public PlayerData PlayerData;
		/// <summary>
		/// The type of the item this script is linked to.
		/// </summary>
		public ShipData.BrokenComponentType Type; //Type of the button, shows what item type it aligns to.
		/// <summary>
		/// The text that shows the amount of the item you have. 
		/// </summary>
		private Text _text;

		void Start()
		{
			_text = GetComponent<Text>();
		}

		void Update()
		{
			_text.text = PlayerData.Inventory.GetAmount(Type).ToString(); //Show the correct number of items of this type currently in the inventory.
		}
	}
}
