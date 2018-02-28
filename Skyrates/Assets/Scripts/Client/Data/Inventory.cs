using System;

namespace Skyrates.Client.Data
{
	[Serializable]
	public class Inventory
	{
		/// <summary>
		/// The amount of each item in the inventory.
		/// </summary>
		private uint[] _itemCounts = new uint[ShipData.BrokenComponentTypes.Length];

		/// <summary>
		/// Adds an amount of items to the inventory.  Amount defaults to 1.
		/// </summary>
		/// <returns>Amount of items actually added.</returns>
		public uint Add(ShipData.BrokenComponentType brokenComponent, uint amount = 1)
		{
			_itemCounts[(uint)brokenComponent] += amount;
			return amount;
		}

		/// <summary>
		/// Removes an amount of items from the inventory.  Amount defaults to 1.
		/// </summary>
		/// <returns>Amount of items actually removed.</returns>
		public uint Remove(ShipData.BrokenComponentType brokenComponent, uint amount = 1)
		{
			amount = Math.Min(amount, _itemCounts[(uint)brokenComponent]);
			_itemCounts[(uint)brokenComponent] -= amount;
			return amount;
		}

		/// <summary>
		/// Clears all items from the inventory.
		/// </summary>
		/// <returns>Amount of items actually removed.</returns>
		public uint Clear()
		{
			uint amount = TotalAmount();
			Array.Clear(_itemCounts, 0, _itemCounts.Length);
			return amount;
		}

		/// <summary>
		/// Gets the amount of items of all component types in inventory.
		/// </summary>
		/// <returns>Amount of items of all component types in inventory..</returns>
		public uint TotalAmount()
		{
			return (uint)_itemCounts.Length;
		}

		/// <summary>
		/// Gets the amount of items of component type in inventory.
		/// </summary>
		/// <returns>Amount of items of component type in inventory.</returns>
		public uint GetAmount(ShipData.BrokenComponentType brokenComponent)
		{
			return _itemCounts[(uint)brokenComponent];
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