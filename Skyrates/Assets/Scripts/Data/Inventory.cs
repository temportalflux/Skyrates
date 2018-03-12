using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Client.Data
{
	[Serializable]
	public class Inventory
	{
		/// <summary>
		/// The amount of each item in the inventory.
		/// </summary>
		private uint[] _itemCounts = new uint[ShipData.BrokenComponentTypes.Length];
		private List<GameObject>[] _generatedLoot = new List<GameObject>[ShipData.BrokenComponentTypes.Length];

		public List<GameObject>[] GeneratedLoot
		{
			get
			{
				//Not yet initialized?
				if(_generatedLoot[0] == null)
				{
					for (uint i = 0; i < _generatedLoot.Length; ++i)
						_generatedLoot[i] = new List<GameObject>(); //Initialize all lists.
				}
				return _generatedLoot;
			}

			set
			{
				_generatedLoot = value;
			}
		}

		/// <summary>
		/// Adds an amount of items to the inventory.  Amount defaults to 1.
		/// </summary>
		/// <returns>Amount of items actually added.</returns>
		public uint Add(ShipData.BrokenComponentType brokenComponent, uint amount = 1)
		{
			_itemCounts[(uint)brokenComponent] += amount;
			GeneratedLoot[(uint)brokenComponent].Add(null);
			return amount;
		}

		/// <summary>
		/// Maps latest inventory space of any broken component type to generated loot.
		/// </summary>
		public void MapGeneratedLoot(ShipData.BrokenComponentType brokenComponent, GameObject generated, bool forced = false)
		{
			if(forced || GeneratedLoot[(uint)brokenComponent].Count == 0) GeneratedLoot[(uint)brokenComponent].Add(generated);
			else GeneratedLoot[(uint)brokenComponent][GeneratedLoot[(uint)brokenComponent].Count - 1] = generated;
		}

		/// <summary>
		/// Removes an amount of items from the inventory.  Amount defaults to 1.
		/// </summary>
		/// <returns>Amount of items actually removed.</returns>
		public uint Remove(ShipData.BrokenComponentType brokenComponent, uint amount = 1)
		{
			amount = Math.Min(amount, _itemCounts[(uint)brokenComponent]);
			_itemCounts[(uint)brokenComponent] -= amount;
			for (uint i = amount; i > 0; --i)
			{
				GameObject.Destroy(GeneratedLoot[(uint)brokenComponent][GeneratedLoot[(uint)brokenComponent].Count - 1]);
				GeneratedLoot[(uint)brokenComponent].RemoveAt(GeneratedLoot[(uint)brokenComponent].Count - 1);
			}
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
			foreach(List<GameObject> generatedList in GeneratedLoot)
			{
				foreach(GameObject obj in generatedList)
				{
					if(obj != null) GameObject.Destroy(obj);
				}
				generatedList.Clear();
			}
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

		/// <summary>
		/// Gets the name of a certain broken component type.
		/// </summary>
		/// <returns>The name of the broken component type.</returns>
		public static string GetName(ShipData.BrokenComponentType brokenComponent)
		{
			//We have to use this because Unity's .NET version is outdated and does not contain the Display attribute for enums.
			return brokenComponent == ShipData.BrokenComponentType.Hull ? "Hull Armor" : brokenComponent.ToString();
		}

	}
}