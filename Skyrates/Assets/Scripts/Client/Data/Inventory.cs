using System;

namespace Skyrates.Client.Data
{
	[Serializable]
	public class Inventory
	{
		/// <summary>
		/// The amount of each item in the inventory.
		/// </summary>
		private uint[] ItemCounts = new uint[ShipData.BrokenComponentTypes.Length];

		/// <summary>
		/// Adds an amount of items to the inventory.  Amount defaults to 1.
		/// </summary>
		/// <returns>Amount of items actually added.</returns>
		public uint Add(ShipData.BrokenComponentType brokenComponent, uint amount = 1)
		{
			ItemCounts[(uint)brokenComponent] += amount;
			return amount;
		}

		/// <summary>
		/// Removes an amount of items from the inventory.  Amount defaults to 1.
		/// </summary>
		/// <returns>Amount of items actually removed.</returns>
		public uint Remove(ShipData.BrokenComponentType brokenComponent, uint amount = 1)
		{
			amount = Math.Min(amount, ItemCounts[(uint)brokenComponent]);
			ItemCounts[(uint)brokenComponent] -= amount;
			return amount;
		}

		/// <summary>
		/// Gets the amount of items of component type in inventory.
		/// </summary>
		/// <returns>Amount of items of component type in inventory.</returns>
		public uint GetAmount(ShipData.BrokenComponentType brokenComponent)
		{
			return ItemCounts[(uint)brokenComponent];
		}

		/// <summary>
		/// Gets the amount of items of component type in inventory.
		/// </summary>
		/// <returns>Amount of items of component type in inventory.</returns>
		public static ShipData.BrokenComponentType ComponentToBrokenComponent(ShipData.ComponentType component)
		{
			return ShipData.ComponentTypeToBrokenComponentType[(uint)component];
		}

	}
}