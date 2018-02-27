using System;

namespace Skyrates.Client.Data
{
	public class Inventory
	{
		/// <summary>
		/// The amount of each item in the inventory.
		/// </summary>
		private uint[] ItemCounts = new uint[ShipData.ComponentTypes.Length];

		/// <summary>
		/// Adds an amount of items to the inventory.  Amount defaults to 1.
		/// </summary>
		/// <returns>Amount of items actually added.</returns>
		public uint Add(ShipData.ComponentType component, uint amount = 1)
		{
			ItemCounts[(uint)component] += amount;
			return amount;
		}

		/// <summary>
		/// Removes an amount of items from the inventory.  Amount defaults to 1.
		/// </summary>
		/// <returns>Amount of items actually removed.</returns>
		public uint Remove(ShipData.ComponentType component, uint amount = 1)
		{
			amount = Math.Min(amount, ItemCounts[(uint)component]);
			ItemCounts[(uint)component] -= amount;
			return amount;
		}

		/// <summary>
		/// Gets the amount of items of component type in inventory.
		/// </summary>
		/// <returns>Amount of items of component type in inventory.</returns>
		public uint GetAmount(ShipData.ComponentType component)
		{
			return ItemCounts[(uint)component];
		}

	}
}