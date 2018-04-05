using Skyrates.Data;
using Skyrates.Ship;
using UnityEngine;
using UnityEngine.UI;

namespace Skyrates.UI
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
		    uint countInInv = this.PlayerData.Inventory.GetAmount(Type);
            
		    _text.text = string.Format("{0} / {1}", countInInv, 0);
		}
	}
}
